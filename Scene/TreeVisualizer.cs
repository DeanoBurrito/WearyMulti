using System;
using System.Collections.Generic;
using System.Text;
using SFML.System;
using SFML.Graphics;

namespace Weary.Scene
{
    internal sealed class TreeVisualizer
    {
        private float vScrollOffset = 0f;
        private float hScrollOffset = 0f;
        private float indentAmount = 24f;
        private float lineHeight = 24f;
        private List<ulong> openNodes = new List<ulong>();
        private ulong selectedNode = 0;
        private SceneTree tree;

        private Font bodyFont;

        public TreeVisualizer(SceneTree tree)
        {
            bodyFont = new Font("_Data/Fonts/NotoSans_Regular.ttf");

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
            if (Input.IsKeyPressed("Up"))
                MoveUp();
            else if (Input.IsKeyPressed("Down"))
                MoveDown();
            else if (Input.IsKeyPressed("Left"))
                MoveLeft();
            else if (Input.IsKeyPressed("Right"))
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
                Text noDisplayText = new Text("Tree is null, nothing to display.", bodyFont);
                noDisplayText.Position = new Vector2f(50f, 50f);
                noDisplayText.FillColor = Color.Red;
                target.Draw(noDisplayText);

                //this is great for resource usage I'm sure
                noDisplayText.Dispose();
                return;
            }

            Vector2f refVector = new Vector2f(0f, 0f);
            RenderNode(target, tree.root, ref refVector);
        }

        private void RenderNode(RenderTarget target, SceneNode node, ref Vector2f cursorPosition)
        {
            List<SceneNode> children = node.GetChildren();
            StringBuilder textBuilder = new StringBuilder();

            textBuilder.Append("[" + node.name + ", " + children.Count + " children] ");
            textBuilder.Append("");

            Text nodeText = new Text(textBuilder.ToString(), bodyFont);
            nodeText.Position = new Vector2f(hScrollOffset, vScrollOffset) + cursorPosition;
            nodeText.CharacterSize = 16;

            if (node.uuid == selectedNode)
                nodeText.FillColor = new Color(0, 170, 255);
            else
                nodeText.FillColor = Color.White;

            target.Draw(nodeText);

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