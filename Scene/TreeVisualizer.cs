using System;
using System.Collections.Generic;
using System.Text;
using Weary.Rendering;
using Weary.Resources;

namespace Weary.Scene
{
    internal sealed class TreeVisualizer
    {
        private float vScrollOffset = 0f;
        private float hScrollOffset = 0f;
        private float indentAmount = 24f;
        private float lineHeight = 22f;
        private List<ulong> openNodes = new List<ulong>();
        private ulong selectedNode = 0;
        private SceneTree tree;

        private ResourceRef bodyFont;

        public TreeVisualizer(SceneTree tree)
        {
            bodyFont = ResourceManager.Global.GetRef("Fonts/NotoMono_Regular.ttf");

            SetTree(tree);
        }

        public void SetTree(SceneTree tree)
        {
            this.tree = tree;

            selectedNode = tree.root.uuid;
            openNodes = new List<ulong>();
        }

        public void Update(DeltaTime delta)
        {
            if (Input.IsKeyPressed(InputKey.ArrowUp))
                MoveUp();
            else if (Input.IsKeyPressed(InputKey.ArrowDown))
                MoveDown();
            else if (Input.IsKeyPressed(InputKey.ArrowLeft))
                MoveLeft();
            else if (Input.IsKeyPressed(InputKey.ArrowRight))
                MoveRight();
        }

        private void MoveUp()
        {
            SceneNode current = tree.GetNode(selectedNode);
            SceneNode parent = tree.GetParentOf(selectedNode);
            if (parent == null)
                return;

            List<SceneNode> siblings = parent.GetChildren();
            int index = siblings.IndexOf(current);
            index--;

            if (index < 0)
            {
                selectedNode = parent.uuid;
            }
            else
            {
                selectedNode = siblings[index].uuid;
            }
        }

        private void MoveDown()
        {
            SceneNode current = tree.GetNode(selectedNode);
            if (openNodes.Contains(selectedNode))
            {
                List<SceneNode> children = current.GetChildren();
                if (children.Count > 0)
                    selectedNode = children[0].uuid;
            }

            SceneNode parent = tree.GetParentOf(selectedNode);
            if (parent == null)
                return;

            List<SceneNode> siblings = parent.GetChildren();
            int index = siblings.IndexOf(current);
            index++;

            if (index >= siblings.Count)
            {
                selectedNode = parent.uuid;
                openNodes.Remove(parent.uuid);
                MoveDown();
                openNodes.Add(parent.uuid);
            }
            else
            {
                selectedNode = siblings[index].uuid;
            }
        }

        private void MoveLeft()
        {
            SceneNode current = tree.GetNode(selectedNode);
            SceneNode parent = current.GetParent();

            if (openNodes.Contains(selectedNode))
            {
                openNodes.Remove(selectedNode);
            }
            else if (parent != null)
            {
                selectedNode = parent.uuid;
            }
        }

        private void MoveRight()
        {
            SceneNode current = tree.GetNode(selectedNode);
            List<SceneNode> children = current.GetChildren();

            if (openNodes.Contains(selectedNode))
            {
                if (children.Count > 0)
                    selectedNode = children[0].uuid;
            }
            else
            {
                if (children.Count > 0)
                    openNodes.Add(selectedNode);
            }
        }

        public void Render(RenderTarget target)
        {
            if (tree == null)
            {
                RenderParams textParams = new RenderParams();
                textParams.tintColor = Color.Red;
                textParams.position = new Vector2f(50f, 50f);
                target.DrawText(bodyFont.Get<Font>(), "Tree is null, nothing to display", 16, textParams);

                return;
            }

            Vector2f refVector = Vector2f.Zero;
            RenderNode(target, tree.root, ref refVector);
        }

        private void RenderNode(RenderTarget target, SceneNode node, ref Vector2f cursorPosition)
        {
            List<SceneNode> children = node.GetChildren();
            StringBuilder textBuilder = new StringBuilder();

            textBuilder.Append("[" + node.name + ", " + children.Count + " children] ");
            textBuilder.Append("");

            RenderParams textParams = new RenderParams();
            textParams.position = new Vector2f(hScrollOffset, vScrollOffset) + cursorPosition;

            if (node.uuid == selectedNode)
                textParams.tintColor = new Color(0f, 0.5f, 1f);
            else
                textParams.tintColor = Color.White;

            target.DrawText(bodyFont.Get<Font>(), textBuilder.ToString(), 16, textParams);

            cursorPosition += new Vector2f(0f, lineHeight);
            if (openNodes.Contains(node.uuid))
            {
                if (children.Count > 0)
                {
                    cursorPosition += new Vector2f(indentAmount, 0f);
                    for (int i = 0; i < children.Count; i++)
                    {
                        RenderNode(target, children[i], ref cursorPosition);
                    }
                    cursorPosition -= new Vector2f(indentAmount, 0f);
                }
            }
        }
    }
}