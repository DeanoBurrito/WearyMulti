using System;
using System.IO;
using Weary.Rendering;
using Weary.Resources;
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
            
            string jsonData = ResourceManager.Global.GetRef("Scenes/ExampleScene.json").Get<TextResource>().resource;
            tree = SceneTree.LoadFromJson(jsonData);
            visualizer = new TreeVisualizer(tree);
        }

        protected override void Deinit()
        {}

        protected override void Update(DeltaTime delta)
        {
            if (Input.IsKeyReleased(InputKey.F1))
                renderVisualizer = !renderVisualizer;
            if (Input.IsKeyReleased(InputKey.F2))
            {
                string jsonData = SceneTree.SaveToJson(tree);
                File.WriteAllText("_Data/Scenes/SavedDebugScene.json", jsonData);
            }
            
            if (renderVisualizer)
                visualizer.Update(delta);
        }

        protected override void Render(RenderTarget target)
        {
            if (renderVisualizer)
                visualizer.Render(target);
        }

        protected override void HandleExitRequest()
        {}
    }
}