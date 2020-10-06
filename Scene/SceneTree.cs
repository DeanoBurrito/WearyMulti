using System;
using System.Collections.Generic;

namespace Weary.Scene
{
    public sealed class SceneTree
    {
        private static SceneTree current;

        public readonly SceneNode root;
        internal Dictionary<ulong, SceneNode> allNodes = new Dictionary<ulong, SceneNode>();

        private ulong highestUuid = 1; //0 will always be free, and mean null
        private Queue<ulong> freeUuids = new Queue<ulong>();
        private List<ulong> nodeFreeQueue = new List<ulong>();

        public static SceneTree LoadJson(string jsonFile)
        {
            throw new NotImplementedException();
        }

        public static string SaveJson(SceneTree instance)
        {
            throw new NotImplementedException();
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

            root = new ExampleNode();
            root.name = "Root";
            root.AddChild(new ExampleNode() { name = "Child1" });
            root.AddChild(new ExampleNode() { name = "Child2" });
            root.GetChildren()[0].AddChild(new ExampleNode() { name = "SubChild1" });
        }

        public void Update(DeltaTime delta)
        {
            //update root node, this will walk the tree and update anything that needs to be, while respect update policies.
            UpdateNode(delta, false, root.uuid);
            
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

        public SceneNode GetNode(ulong uuid)
        {
            if (!allNodes.ContainsKey(uuid))
                return null;
            return allNodes[uuid];
        }

        internal ulong GenerateUuid()
        {
            ulong rtnId = highestUuid + 1;
            if (freeUuids.Count > 0)
            {
                rtnId = freeUuids.Dequeue();
            }
            else
            {
                highestUuid++;
            }
            return rtnId;
        }

        internal void FreeUuid(ulong uuid)
        {
            if (uuid == highestUuid)
            {
                highestUuid--;
                return;
            }
            else
            {
                freeUuids.Enqueue(uuid);
            }
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