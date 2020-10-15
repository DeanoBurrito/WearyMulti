using System;
using System.IO;
using SFML.Graphics;
using Weary.Scene;
using Weary.Scene.ObjectDB;

namespace Weary.Scene
{
    public class SceneMainLoop : MainLoop
    {
        private bool renderVisualizer = false;
        private TreeVisualizer visualizer;
        private SceneTree tree;
        
        protected override void Init()
        {
            ObjectDatabase.InitGlobal();
            
            string jsonData = File.ReadAllText("_Data/Scenes/ExampleScene.json");
            tree = SceneTree.LoadFromJson(jsonData);
            visualizer = new TreeVisualizer(tree);
        }

        protected override void Deinit()
        {}

        protected override void Update(DeltaTime delta)
        {
            if (Input.IsKeyReleased("F1"))
                renderVisualizer = !renderVisualizer;
            if (Input.IsKeyReleased("F2"))
            {
                string jsonData = SceneTree.SaveToJson(tree);
                File.WriteAllText("_Data/Scenes/SavedDebugScene.json", jsonData);
            }
            
            if (renderVisualizer)
                visualizer.Update(delta);
        }

        protected override void Render(RenderWindow window)
        {
            if (renderVisualizer)
                visualizer.Render(window);
        }

        protected override void HandleExitRequest()
        {}
    }
}