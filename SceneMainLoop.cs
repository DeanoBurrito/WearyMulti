using System;
using SFML.Graphics;
using Weary.Scene;

namespace Weary
{
    public class SceneMainLoop : MainLoop
    {
        private bool renderVisualizer = false;
        private TreeVisualizer visualizer;
        private SceneTree tree;
        
        protected override void Init()
        {
            tree = new SceneTree();
            visualizer = new TreeVisualizer(null);
        }

        protected override void Deinit()
        {}

        protected override void Update(DeltaTime delta)
        {
            if (Input.IsKeyReleased("F1"))
                renderVisualizer = !renderVisualizer;
            
            if (renderVisualizer)
                visualizer.Update(delta);
        }

        protected override void Draw(RenderWindow window)
        {
            window.Clear();

            if (renderVisualizer)
                visualizer.Render(window);
            
            window.Display();
        }

        protected override void HandleExitRequest()
        {}
    }
}