using System;
using System.Collections.Generic;

namespace Weary.Scene
{
    public abstract class SceneNode
    {
        public readonly ulong uuid;
        internal readonly SceneTree tree;
        public string name = "";
        public NodeUpdatePolicy updatePolicy = NodeUpdatePolicy.FollowParent;

        internal List<ulong> children = new List<ulong>();
        internal ulong parent = 0;

        internal SceneNode(SceneTree tree = null)
        {
            this.tree = (tree == null ? SceneTree.GetCurrent() : tree);
            this.uuid = this.tree.GenerateUuid();
            this.tree.allNodes.Add(uuid, this);
        }

        public abstract void FromBytes(byte[] data);
        public abstract byte[] ToBytes();
        public abstract void FromJson(string data);
        public abstract string ToJson();

        public List<SceneNode> GetChildren()
        {
            return tree.GetChildrenOf(uuid);
        }

        public void AddChild(SceneNode node)
        {
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

        public void Free()
        {
            tree.FreeNode(uuid);
        }

        public void QueueFree()
        {
            tree.QueueFreeNode(uuid);
        }

        internal void Update(DeltaTime delta)
        {}

        internal void FixedUpdate(DeltaTime delta)
        {}
    }
}