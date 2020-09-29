using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFML.Window;

namespace Weary
{
    public static class Input
    {
        private static bool[] prevKeys = new bool[(int)Keyboard.Key.KeyCount];
        private static bool[] currKeys = new bool[(int)Keyboard.Key.KeyCount];

        private static Dictionary<string, Keyboard.Key> keyMaps = new Dictionary<string, Keyboard.Key>();

        internal static void Init(bool addTransparentMaps)
        {
            keyMaps.Clear();
            for (int i = 0; i < prevKeys.Length; i++)
            {
                prevKeys[i] = currKeys[i] = false;
                if (addTransparentMaps)
                {
                    Keyboard.Key k = (Keyboard.Key)i;
                    keyMaps.Add(k.ToString(), k);
                }
            }
        }

        internal static void Update(DeltaTime delta)
        {
            currKeys.CopyTo(prevKeys, 0);
            for (int i = 0; i < currKeys.Length; i++)
            {
                currKeys[i] = Keyboard.IsKeyPressed((Keyboard.Key)i);
            }
        }

        public static void LoadConfig(string data)
        {
            keyMaps.Clear();
            using (StringReader reader = new StringReader(data))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || line.StartsWith("//"))
                        continue;
                    
                    string[] segments = line.Split('=');
                    if (segments.Length != 2)
                        continue; //skip current line if its not in the correct format
                    if (!Enum.TryParse<Keyboard.Key>(segments[1], true, out Keyboard.Key parsedValue))
                        continue;

                    keyMaps.Add(segments[0], parsedValue);
                }
            }
        }

        public static string SaveConfig()
        { 
            StringBuilder configBuilder = new StringBuilder();
            using (StringWriter writer = new StringWriter(configBuilder))
            {
                foreach (KeyValuePair<string, Keyboard.Key> pair in keyMaps)
                {
                    writer.Write(pair.Key);
                    writer.Write(" = ");
                    writer.WriteLine(pair.Value);
                }
            }

            return configBuilder.ToString();
        }

        public static bool IsKeyDown(string id)
        {
            if (!keyMaps.ContainsKey(id))
                return false;

            return currKeys[(int)keyMaps[id]];
        }

        public static bool IsKeyPressed(string id)
        {
            if (!keyMaps.ContainsKey(id))
                return false;

            Keyboard.Key key = keyMaps[id];
            return currKeys[(int)key] && !prevKeys[(int)key];
        }

        public static bool IsKeyReleased(string id)
        {
            if (!keyMaps.ContainsKey(id))
                return false;

            Keyboard.Key key = keyMaps[id];
            return !currKeys[(int)key] && prevKeys[(int)key];
        }

        public static void RegisterKeymap(string id, Keyboard.Key key)
        {
            if (!keyMaps.ContainsKey(id))
                keyMaps.Add(id, key);
            else
                Log.WriteError("Could not add keymap, mapping already exists with id=" + id);
        }

        public static void Unregisterkeymap(string id)
        {
            if (keyMaps.ContainsKey(id))
                keyMaps.Remove(id);
            else
                Log.WriteError("Could not remove keymap, nothing mapped with id=" + id);
        }
    }
}