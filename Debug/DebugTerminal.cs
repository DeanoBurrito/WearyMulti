using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace Weary.Debug
{
    public sealed class DebugTerminal
    {
        private static Dictionary<string, DebugCommand> commands = new Dictionary<string, DebugCommand>();

        public static void RegisterCommand(string identifier, string helpText, Action<string[]> callback)
        {
            identifier = identifier.ToLower();
            commands.TryAdd(identifier, new DebugCommand(identifier, helpText, callback));
        }

        private Font textFont;
        private StringBuilder currentLine = new StringBuilder();
        private bool isVisible = false;

        private ConcurrentQueue<string> unreadLogOutput = new ConcurrentQueue<string>();
        private List<string> terminalHistory = new List<string>();
        private int maxHistoryLength = 2048;

        private float lineHeight = 18f;
        private uint textFontSize = 14;

        private int maxLogDequeueChunk = 100; //max number of log messages to read in a single frame, creates logging incoherency (only in terminal) but prevents infinite loops
        private int scrollOffset = 0;

        private float cursorHeight = 18f;
        private bool cursorVisible = false;
        private int cursorFlashMs = 333;
        private int cursorFlashTimer = 0;

        internal DebugTerminal()
        {
            BuiltInCommands.Init();
            textFont = new Font("_Data/Fonts/NotoMono_Regular.ttf");

            Log.OnWriteLine += (string msg) => { unreadLogOutput.Enqueue("[LOG] " + msg); };
            Log.OnWriteError += (string msg) => { unreadLogOutput.Enqueue("[ERROR] " + msg); };
        }

        public void Update(DeltaTime delta)
        {
            UpdateTerminalHistory();

            if (Input.IsKeyReleased("F10"))
                isVisible = !isVisible;

            if (!isVisible)
                return;

            if (Input.IsKeyReleased("Return"))
            {
                terminalHistory.Add(">" + currentLine.ToString());
                HandleCommand(currentLine.ToString());
                currentLine.Clear();
            }
            else if (Input.IsKeyReleased("Backspace"))
            {
                if (currentLine.Length > 0)
                    currentLine = currentLine.Remove(currentLine.Length - 1, 1);
            }

            int scrollMod = Input.IsKeyDown("LShift") ? 5 : 1;
            if (Input.IsKeyReleased("Down"))
            {
                scrollOffset -= scrollMod;
                if (scrollOffset < 0)
                    scrollOffset = 0;
            }
            else if (Input.IsKeyReleased("Up"))
            {
                scrollOffset += scrollMod;
                if (scrollOffset >= terminalHistory.Count)
                    scrollOffset = terminalHistory.Count - 1;
            }

            cursorFlashTimer -= delta.milliseconds;
            if (cursorFlashTimer <= 0)
            {
                cursorFlashTimer = cursorFlashMs;
                cursorVisible = !cursorVisible;
            }
        }

        public void Render(RenderTarget target)
        {
            if (!isVisible)
                return;

            RectangleShape background = new RectangleShape();
            background.FillColor = new Color(20, 20, 20);
            background.Position = new Vector2f(0f, 0f);
            background.Size = new Vector2f(target.Size.X, target.Size.Y / 2f);
            target.Draw(background);

            float historyDrawableHeight = background.Size.Y - (lineHeight * 2f);
            int historyShowStart = terminalHistory.Count - (int)(historyDrawableHeight / lineHeight) - scrollOffset;
            if (historyShowStart < 0)
                historyShowStart = 0;

            int historyShowEnd = historyShowStart + (int)(historyDrawableHeight / lineHeight);
            if (historyShowEnd > terminalHistory.Count)
                historyShowEnd = terminalHistory.Count;

            Text textLine = new Text("", textFont, textFontSize);
            for (int i = historyShowStart; i < historyShowEnd; i++)
            {
                textLine.DisplayedString = terminalHistory[i];
                textLine.Position = new Vector2f(2f, (i - historyShowStart) * lineHeight);

                if (i == historyShowStart && historyShowStart > 0)
                    textLine.FillColor = new Color(180, 180, 180, 180);
                else if (terminalHistory[i].Contains("[ERROR]"))
                    textLine.FillColor = Color.Red;
                else
                    textLine.FillColor = Color.White;
                    
                target.Draw(textLine);
            }

            float currentLineY = background.Size.Y - (lineHeight * 1.5f);
            textLine.FillColor = Color.White;
            textLine.Position = new Vector2f(2f, currentLineY);
            textLine.DisplayedString = ">>> " + currentLine.ToString();
            target.Draw(textLine);

            if (cursorVisible)
            {
                RectangleShape cursorRect = new RectangleShape();
                cursorRect.FillColor = Color.White;
                cursorRect.Position = new Vector2f(textLine.GetLocalBounds().Width + 4f, currentLineY + (lineHeight - cursorHeight) / 2f);
                cursorRect.Size = new Vector2f(2f, cursorHeight);
                target.Draw(cursorRect);
            }
        }

        internal void HandleWindowTextEntered(object sender, TextEventArgs e)
        {
            if (!isVisible)
                return;
            if (e.Unicode.Length != 1)
                return;

            char character = e.Unicode[0];
            if (char.IsLetterOrDigit(character) || character == '_'
                || character == '.' || character == '-'
                || character == '/' || character == ' ')
                currentLine.Append(character);
        }

        private void UpdateTerminalHistory()
        {
            int currIndex = 0;
            while (currIndex < maxLogDequeueChunk && unreadLogOutput.TryDequeue(out string readLog))
            {
                terminalHistory.Add(readLog);
                currIndex++;
            }

            if (terminalHistory.Count > maxHistoryLength)
                terminalHistory.RemoveRange(0, terminalHistory.Count - maxHistoryLength);
        }

        private void HandleCommand(string input)
        {
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
                return;

            parts[0] = parts[0].ToLower();
            if (commands.ContainsKey(parts[0]))
            {
                string[] args = new string[parts.Length - 1];
                Array.Copy(parts, 1, args, 0, args.Length);
                commands[parts[0]].callback.Invoke(args);
            }
            else
            {
                Log.WriteError("Command not found: " + parts[0]);
            }
        }

        private static class BuiltInCommands
        {
            public static void Init()
            {
                RegisterCommand("help", "Displays this output.", Help);
                RegisterCommand("echo", "Echoes a line back to the terminal.", Echo);
                RegisterCommand("crash", "Causes an immediate crash and writes the reason to a crashlog in the executable directory.", Crash);
            }

            public static void Help(string[] args)
            {
                Log.WriteLine(commands.Count + " commands available. Listed below: ");
                foreach (string s in commands.Keys)
                {
                    Log.WriteLine(s.PadRight(19) + " - " + commands[s].help);
                }
            }

            public static void Echo(string[] args)
            {
                string result = string.Join(' ', args);
                Log.WriteLine(result);
            }

            public static void Crash(string[] args)
            {
                string reason = string.Join(' ', args);
                Log.FatalError("Crash() terminal command run, reason: " + reason);
            }
        }
    }
}