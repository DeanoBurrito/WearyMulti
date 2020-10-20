using System;
using Weary.Backends.SF;
using Weary.Rendering;
using Weary.Debug;
using Weary.Resources;

namespace Weary
{
    public class MainLoop
    {
        private readonly float fixedUpdateStep = 1f / 60f;
        private bool keepRunning = true;
        private Window mainWindow;
        private DebugTerminal debugTerminal;

        public MainLoop()
        {}

        public void Run()
        {
            InitInternal();
            
            while (keepRunning)
            {
                DeltaTime delta = new DeltaTime(fixedUpdateStep);

                UpdateInternal(delta);
                if (keepRunning)
                    FixedUpdate(delta);
                if (keepRunning)
                    RenderInternal(mainWindow);

                //TODO: hack, fix this
                System.Threading.Thread.Sleep((int)(fixedUpdateStep * 1000f));
            }

            DeinitInternal();
        }

        public void Exit()
        {
            HandleExitInternal(mainWindow);
        }

        protected virtual void Init()
        {}

        private void InitInternal()
        {
            Log.WriteLine("--- Initializing main loop.");

            ResourceManager.Init();
            Input.Init(true);
            new SFWindowServer().Init();

            mainWindow = WindowServer.Global.CreateWindow(1600, 900, false, "Weary (now with servers!)");
            mainWindow.onDestroy += HandleExitInternal;

            debugTerminal = new DebugTerminal();
            (WindowServer.Global as SFWindowServer).GetRenderWindow(mainWindow).TextEntered += debugTerminal.HandleWindowTextEntered;

            Init();
        }

        protected virtual void Deinit()
        {}

        private void DeinitInternal()
        {
            Log.WriteLine("--- Deinitializing main loop.");

            Deinit();

            WindowServer.Global.Deinit();
        }

        protected virtual void Update(DeltaTime delta)
        {}

        private void UpdateInternal(DeltaTime delta)
        {
            WindowServer.Global.HandleEvents();

            Input.Update(delta);
            debugTerminal.Update(delta);
            
            Update(delta);
        }

        protected virtual void FixedUpdate(DeltaTime delta)
        {}

        private void RenderInternal(Window window)
        {   
            SFML.Graphics.RenderWindow sfWindow = (WindowServer.Global as SFWindowServer).GetRenderWindow(mainWindow);
            sfWindow.Clear(SFML.Graphics.Color.Black);

            Render(window);
            debugTerminal.Render(sfWindow);

            sfWindow.Display();
        }

        protected virtual void Render(Window window)
        {}

        protected virtual void HandleExitRequest()
        {}

        private void HandleExitInternal(object sender)
        {
            keepRunning = false;
            HandleExitRequest();
        }
    }
}