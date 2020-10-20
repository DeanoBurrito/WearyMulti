using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Weary.Scene
{
    public class SceneNode
    {
        public readonly ulong uuid;
        internal readonly SceneTree tree;
        public string name = "";
        public NodeUpdatePolicy updatePolicy = NodeUpdatePolicy.FollowParent;
        public readonly bool isComponent;

        internal List<ulong> children = new List<ulong>();
        internal ulong parent = 0;

        internal SceneNode(SceneTree tree = null, bool isComp = false)
        {
            this.tree = (tree == null ? SceneTree.GetCurrent() : tree);
            this.uuid = this.tree.uuidManager.GenerateId();
            this.tree.allNodes.Add(uuid, this);

            this.isComponent = isComp;
        }

        internal SceneNode(SceneTree tree, ulong id, bool isComp = false)
        {
            isComponent = isComp;
            uuid = id;
            this.tree = tree;
            this.tree.allNodes.Add(uuid, this);
        }

        public virtual void FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }
        
        public virtual byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public virtual void FromJson(JsonElement data)
        {
            if (data.TryGetProperty("Parent", out JsonElement parentProp))
            {
                parent = parentProp.GetUInt64();
            }
            if (data.TryGetProperty("Name", out JsonElement nameProp))
            {
                name = nameProp.GetString();
            }
            if (data.TryGetProperty("UpdatePolicy", out JsonElement updatePolicyProp))
            {
                updatePolicy = (NodeUpdatePolicy)updatePolicyProp.GetByte();
            }
        }

        public virtual void ToJson(Utf8JsonWriter writer)
        {
            writer.WriteNumber("UUID", uuid);
            writer.WriteBoolean("IsComponent", isComponent);
            writer.WriteNumber("Parent", parent);
            writer.WriteString("Name", name);
            writer.WriteNumber("UpdatePolicy", (byte)updatePolicy);
        }

        public List<SceneNode> GetChildren()
        {
            return tree.GetChildrenOf(uuid);
        }

        public void AddChild(SceneNode node)
        {
            if (isComponent)
                return;
            if (node == null)
            {
                Log.WriteError("Cannot add child node is null.");
                return;
            }
            if (!tree.allNodes.ContainsKey(node.uuid))
            {
                Log.WriteError("Cannot add a child node from a different tree (incompatable IDs).");
                return;
            }
            if (children.Contains(node.uuid))
            {
                Log.WriteError("Node is already a child.");
                return;
            }

            children.Add(node.uuid);
            node.parent = uuid;
        }

        public void RemoveChild(SceneNode node)
        {
            if (isComponent)
                return;
            if (node == null)
            {
                Log.WriteError("Cannot remove a null child.");
                return;
            }
            if (!tree.allNodes.ContainsKey(node.uuid))
            {
                Log.WriteError("Cannot remove a child that is not part of the current tree.");
                return;
            }
            if (!children.Contains(node.uuid))
            {
                Log.WriteError("Cannot remove a node that is not a child.");
                return;
            }

            children.Remove(node.uuid);
            node.parent = 0;
        }

        public SceneNode GetParent()
        {
            return tree.GetParentOf(uuid);
        }

        public SceneNode GetNode(string friendlyName)
        {
            if (isComponent)
                return null;

            bool isAbsolute = friendlyName.StartsWith('/');
            if (isAbsolute)
            {
                friendlyName = friendlyName.Remove(0, 1);
                return tree.root.GetNode(friendlyName);
            }

            string[] pathParts = friendlyName.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length < 1)
                return null; //empty path, cant search on that
            
            List<SceneNode> children = GetChildren();
            
            foreach (SceneNode child in children)
            {
                if (child.name == pathParts[0])
                {
                    if (pathParts.Length == 1)
                        return child;
                    else
                        return child.GetNode(string.Join('/', pathParts, 1, pathParts.Length - 1));
                }
            }
            return null;
        }

        public void Free()
        {
            tree.FreeNode(uuid);
        }

        public void QueueFree()
        {
            tree.QueueFreeNode(uuid);
        }

        public virtual void Update(DeltaTime delta)
        {}

        public virtual void FixedUpdate(DeltaTime delta)
        {}
    }
}