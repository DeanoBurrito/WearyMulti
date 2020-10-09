using System;
using SFML.Window;
using SFML.Graphics;

namespace Weary
{
    public class MainLoop
    {
        private readonly float fixedUpdateStep = 1f / 60f;
        private bool keepRunning = true;
        private RenderWindow window;
        
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
                Draw(window);

                //TODO: hack, fix this
                System.Threading.Thread.Sleep((int)(fixedUpdateStep * 1000f));
            }

            DeinitInternal();
        }

        public void Exit()
        {
            keepRunning = false;
        }

        protected virtual void Init()
        {}

        private void InitInternal()
        {
            Log.WriteLine("--- Initializing main loop.");
            Input.Init(true);
            window = new RenderWindow(new VideoMode(1600, 900), "Weary", Styles.Close);
            window.Closed += HandleExitInternal;

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
            
            Update(delta);
        }

        protected virtual void FixedUpdate(DeltaTime delta)
        {}

        protected virtual void Draw(RenderWindow window)
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