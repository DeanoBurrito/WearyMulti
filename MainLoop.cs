using System;
using SFML.Window;
using SFML.Graphics;
using Weary.Debug;

namespace Weary
{
    public class MainLoop
    {
        private readonly float fixedUpdateStep = 1f / 60f;
        private bool keepRunning = true;
        private RenderWindow window;
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
                FixedUpdate(delta);
                RenderInternal(window);

                //TODO: hack, fix this
                System.Threading.Thread.Sleep((int)(fixedUpdateStep * 1000f));
            }

            DeinitInternal();
        }

        public void Exit()
        {
            HandleExitInternal(window, EventArgs.Empty);
        }

        protected virtual void Init()
        {}

        private void InitInternal()
        {
            Log.WriteLine("--- Initializing main loop.");
            Input.Init(true);
            window = new RenderWindow(new VideoMode(1600, 900), "Weary", Styles.Close);
            window.Closed += HandleExitInternal;

            debugTerminal = new DebugTerminal();
            window.TextEntered += debugTerminal.HandleWindowTextEntered;

            Init();
        }

        protected virtual void Deinit()
        {}

        private void DeinitInternal()
        {
            Log.WriteLine("--- Deinitializing main loop.");

            Deinit();
        }

        protected virtual void Update(DeltaTime delta)
        {}

        private void UpdateInternal(DeltaTime delta)
        {
            window.DispatchEvents();
            Input.Update(delta);
            debugTerminal.Update(delta);
            
            Update(delta);
        }

        protected virtual void FixedUpdate(DeltaTime delta)
        {}

        private void RenderInternal(RenderWindow window)
        {   
            window.Clear(Color.Black);

            Render(window);
            debugTerminal.Render(window);

            window.Display();
        }

        protected virtual void Render(RenderWindow window)
        {}

        protected virtual void HandleExitRequest()
        {}

        private void HandleExitInternal(object sender, EventArgs e)
        {
            keepRunning = false;
            HandleExitRequest();
        }
    }
}