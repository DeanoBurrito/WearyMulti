using System.Diagnostics;
using Weary.Backends.SF;
using Weary.Rendering;
using Weary.Debug;
using Weary.Resources;

namespace Weary
{
    public class MainLoop
    {
        private readonly float fixedUpdateStep = 1f / 60f;
        private readonly float renderStepMin = 1f / 120f;
        private readonly int maxFixedPerCycle = 5;
        private bool keepRunning = true;
        private Stopwatch mainClock;
        private Window mainWindow;
        private RenderTarget mainRenderTarget;
        private DebugTerminal debugTerminal;

        public MainLoop()
        { }

        public void Run()
        {
            InitInternal();

            float fixedDelta = 0f;
            float lastRenderDelta = 0f;
            while (keepRunning)
            {
                float localDelta = (float)mainClock.Elapsed.TotalSeconds;
                fixedDelta += localDelta;
                mainClock.Restart();
                DeltaTime flexDelta = new DeltaTime(localDelta);
                UpdateInternal(flexDelta);

                int fixedStepsTaken = 0;
                while (fixedDelta >= fixedUpdateStep && fixedStepsTaken < maxFixedPerCycle)
                {
                    fixedDelta -= fixedUpdateStep;
                    if (fixedDelta < float.Epsilon * 2f)
                        fixedDelta = 0f; //if we're getting really small values left over, discard them. TODO: better solution to precision errors

                    DeltaTime fixedDt = new DeltaTime(fixedUpdateStep);
                    if (keepRunning)
                        FixedUpdateInternal(fixedDt);
                    fixedStepsTaken++;
                }

                lastRenderDelta += localDelta;
                if (lastRenderDelta >= renderStepMin)
                {
                    //do rendering
                    lastRenderDelta -= renderStepMin;
                    if (lastRenderDelta < float.Epsilon * 2f)
                        lastRenderDelta = 0f;

                    if (keepRunning)
                        RenderInternal(mainRenderTarget);
                }
            }

            DeinitInternal();
        }

        public void Exit()
        {
            HandleExitInternal(mainWindow);
        }

        protected virtual void Init()
        { }

        private void InitInternal()
        {
            Log.WriteLine("--- Initializing main loop.");

            ResourceManager.Init();
            new SFWindowServer().Init();
            new SFInputServer().Init();
            new SFRenderServer().Init();

            mainWindow = WindowServer.Global.CreateWindow(1600, 900, false, "Weary (now with servers!)");
            mainWindow.onDestroy += HandleExitInternal;

            mainRenderTarget = ResourceManager.Global.CreateResource<RenderTarget>("Runtime/WindowRenderTarget");
            RenderServer.Global.BindRenderTarget(mainRenderTarget, mainWindow);

            debugTerminal = new DebugTerminal();
            (WindowServer.Global as SFWindowServer).GetRenderWindow(mainWindow).TextEntered += debugTerminal.HandleWindowTextEntered;

            mainClock = new Stopwatch();

            Init();
        }

        protected virtual void Deinit()
        { }

        private void DeinitInternal()
        {
            Log.WriteLine("--- Deinitializing main loop.");

            Deinit();

            RenderServer.Global.Deinit();
            InputServer.Global.Deinit();
            WindowServer.Global.Deinit();
            ResourceManager.Global.Deinit();
        }

        protected virtual void Update(DeltaTime delta)
        { }

        private void UpdateInternal(DeltaTime delta)
        {
            WindowServer.Global.HandleEvents();
            InputServer.Global.HandleEvents();
            RenderServer.Global.HandleEvents();

            Input.Update(delta);
            debugTerminal.Update(delta);

            Update(delta);
        }

        protected virtual void FixedUpdate(DeltaTime delta)
        { }

        protected virtual void FixedUpdateInternal(DeltaTime delta)
        {
            FixedUpdate(delta);
        }

        protected virtual void Render(RenderTarget target)
        { }

        private void RenderInternal(RenderTarget target)
        {
            target.Clear(Color.Black);

            Render(target);
            debugTerminal.Render(target);

            target.Display();
        }

        protected virtual void HandleExitRequest()
        { }

        private void HandleExitInternal(object sender)
        {
            keepRunning = false;
            HandleExitRequest();
        }
    }
}