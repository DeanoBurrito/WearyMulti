using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Weary.Scene
{
    public sealed class SceneTree
    {
        private static SceneTree current;

        public readonly SceneNode root;
        internal Dictionary<ulong, SceneNode> allNodes = new Dictionary<ulong, SceneNode>();

        internal UuidManager uuidManager = new UuidManager();
        private List<ulong> nodeFreeQueue = new List<ulong>();

        public static SceneTree LoadFromJson(string jsonFile)
        {  
            SceneTree localTree = new SceneTree();
            JsonDocument jdoc = JsonDocument.Parse(jsonFile);
            JsonElement jdocRoot = jdoc.RootElement;

            ulong? highestId = null;
            if (jdocRoot.TryGetProperty("HighestUuid", out JsonElement newHighestProperty) 
                && newHighestProperty.TryGetUInt64(out ulong newHighestUuid))
            {
                
                highestId = newHighestUuid;
            }

            Queue<ulong> newFreeds = new Queue<ulong>();
            if (jdocRoot.TryGetProperty("FreeUuids", out JsonElement newFreedUuidsProperty) 
                && newFreedUuidsProperty.GetArrayLength() > 0)
            {
                foreach (JsonElement freedProp in newFreedUuidsProperty.EnumerateArray())
                {
                    newFreeds.Enqueue(freedProp.GetUInt64());
                }
            }
            localTree.uuidManager = new UuidManager(highestId, newFreeds);

            if (jdocRoot.TryGetProperty("Nodes", out JsonElement nodesProperty))
            {
                foreach (JsonElement element in nodesProperty.EnumerateArray())
                {
                    LoadNodeJson(localTree, element);
                }
            }

            foreach (SceneNode node in localTree.allNodes.Values)
            {
                if (node == localTree.root)
                    continue;
                
                if (!localTree.allNodes.ContainsKey(node.parent))
                {
                    Log.WriteError("Could not parent SceneNode, parent id not found in tree, removing node.");
                    localTree.FreeNode(node.uuid);
                    continue;
                }

                SceneNode parent = localTree.allNodes[node.parent];
                parent.children.Add(node.uuid);
            }

            return localTree;
        }

        private static void LoadNodeJson(SceneTree tree, JsonElement nodeElement)
        {   
            ulong uuid = 0;
            bool isComponent = false;
            if (nodeElement.TryGetProperty("UUID", out JsonElement idElement) &&
                nodeElement.TryGetProperty("IsComponent", out JsonElement isCompElement))
            {
                uuid = idElement.GetUInt64();
                isComponent = isCompElement.GetBoolean();
            }
            else
            {
                Log.WriteError("Could not loaded SceneNode, UUID/IsComponent missing. Skipping node (note this may cause further failures in tree).");
                return;
            }
            
            SceneNode node = new SceneNode(tree, uuid, isComponent);
            node.FromJson(nodeElement);
        }

        public static string SaveToJson(SceneTree instance)
        {
            string rtnData;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(memoryStream))
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("HighestUuid", instance.uuidManager.GetInternals().highest);
                    
                    writer.WriteStartArray("FreedUuids");
                    foreach (ulong freeId in instance.uuidManager.GetInternals().freed)
                        writer.WriteNumberValue(freeId);
                    writer.WriteEndArray();

                    writer.WriteStartArray("Nodes");
                    foreach (SceneNode node in instance.allNodes.Values)
                    {
                        writer.WriteStartObject();
                        node.ToJson(writer);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();

                    writer.WriteEndObject();
                    writer.Flush();
                }
                rtnData = System.Text.Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
            
            return rtnData;
        }

        internal static SceneTree GetCurrent()
        {
            if (current == null)
                current = new SceneTree();
            return current;
        }

        public SceneTree()
        {
            current = this;

            root = new SceneNode(this);
            root.name = "Root";
        }

        public void Update(DeltaTime delta)
        {
            //update root node, this will walk the tree and update anything that needs to be, while respect update policies.
            UpdateNode(delta, false, root.uuid);

            //TODO: implement fixed/non-fixed update cycles
            UpdateNode(delta, true, root.uuid);
            
            HandleFreeRequests();
        }

        public List<SceneNode> GetChildren(SceneNode node)
        {
            return GetChildrenOf(node.uuid);
        }

        public List<SceneNode> GetChildrenOf(ulong uuid)
        {
            if (!allNodes.ContainsKey(uuid))
                return null;
            if (allNodes[uuid].isComponent)
                return null;

            List<SceneNode> children = new List<SceneNode>();
            SceneNode node = allNodes[uuid];
            foreach (ulong childId in node.children)
            {
                if (allNodes.ContainsKey(childId))
                    children.Add(allNodes[childId]);
            }
            return children;
        }

        public SceneNode GetParent(SceneNode node)
        {
            return GetParentOf(node.uuid);
        }

        public SceneNode GetParentOf(ulong uuid)
        {
            if (!allNodes.ContainsKey(uuid))
                return null;

            SceneNode node = allNodes[uuid];
            if (allNodes.ContainsKey(node.parent))
                return allNodes[node.parent];
            return null;
        }

        public SceneNode GetNode(string path)
        {
            return root.GetNode(path);
        }

        public SceneNode GetNode(ulong uuid)
        {
            if (!allNodes.ContainsKey(uuid))
                return null;
            return allNodes[uuid];
        }

        internal void QueueFreeNode(ulong uuid)
        {
            if (!nodeFreeQueue.Contains(uuid))
                nodeFreeQueue.Add(uuid);
        }

        internal void FreeNode(ulong uuid)
        {
            if (!allNodes.ContainsKey(uuid))
            {
                Log.WriteError("Could not free node with id=" + uuid + ", node does not exist in sceneTree.");
                return;
            }

            SceneNode node = allNodes[uuid];
            allNodes.Remove(uuid);

            foreach (ulong childId in node.children)
            {
                FreeNode(childId);
            }

            if (node.parent != 0 && allNodes.ContainsKey(node.parent))
            {
                allNodes[node.parent].children.Remove(uuid);
            }
            
            uuidManager.FreeId(uuid);
        }

        private void HandleFreeRequests()
        {
            foreach (ulong id in nodeFreeQueue)
            {
                FreeNode(id);
            }
        }

        private void UpdateNode(DeltaTime deltaTime, bool isFixedUpdate, ulong id, bool parentExecuting = true)
        {
            SceneNode node = allNodes[id];
            if (node.updatePolicy == NodeUpdatePolicy.AlwaysExecute ||
                (node.updatePolicy == NodeUpdatePolicy.FollowParent && parentExecuting))
            {
                if (isFixedUpdate)
                    node.FixedUpdate(deltaTime);
                else
                    node.Update(deltaTime);
                parentExecuting = true;
            }

            foreach (ulong childId in node.children)
            {
                UpdateNode(deltaTime, isFixedUpdate, childId, parentExecuting);
            }
        }
    }
}