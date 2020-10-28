using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Weary
{
    public static class Input
    {
        private static InputState latest;
        private static Dictionary<string, InputKey> keyMaps = new Dictionary<string, InputKey>();

        public static void LoadConfig(string jsonData)
        {
            try
            {
                JsonDocument jdoc = JsonDocument.Parse(jsonData);
                JsonElement jroot = jdoc.RootElement;
                JsonElement mapsRoot = jroot.GetProperty("KeyMaps");

                int newCount = 0;
                foreach (JsonElement mapping in mapsRoot.EnumerateArray())
                {
                    if (!mapping.TryGetProperty("Id", out JsonElement mapKey) || !mapping.TryGetProperty("Key", out JsonElement mapVal))
                        continue;
                    if (!Enum.TryParse<InputKey>(mapVal.GetString(), true, out InputKey castKey))
                        continue;
                    
                    RegisterKeyMap(mapKey.GetString(), castKey);
                    newCount++;
                }

                Log.WriteLine(newCount + " new inputs mapped to keys.");
            }
            catch (Exception e)
            {
                Log.WriteError("Error occured trying to process input key map file: " + e.ToString());
            }
        }

        public static string SaveConfig()
        {
            try
            {
                string finalStr;
                using (MemoryStream mem = new MemoryStream())
                {
                    using (Utf8JsonWriter writer = new Utf8JsonWriter(mem))
                    {
                        writer.WriteStartObject();
                        writer.WriteStartArray("KeyMaps");
                        
                        foreach (KeyValuePair<string, InputKey> pair in keyMaps)
                        {
                            writer.WriteStartObject();

                            writer.WriteString("Id", pair.Key);
                            writer.WriteString("Key", pair.Value.ToString());

                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                    
                    finalStr = System.Text.Encoding.UTF8.GetString(mem.ToArray());
                }
                return finalStr;
            }
            catch (Exception e)
            {
                Log.WriteError("Error occured trying to save input key mapping to file: " + e.ToString());
                return string.Empty;
            }
        }

        public static void Update(DeltaTime delta)
        {
            latest = InputServer.Global.GetInputState();
        }

        public static bool IsKeyDown(InputKey key)
        {
            return latest.IsKeyDown(key);
        }

        public static bool IsKeyDown(string id)
        {
            if (keyMaps.TryGetValue(id, out InputKey key))
                return latest.IsKeyDown(key);

            return false;
        }

        public static bool IsKeyPressed(InputKey key)
        {
            return latest.IsKeyPressed(key);
        }

        public static bool IsKeyPressed(string id)
        {
            if (keyMaps.TryGetValue(id, out InputKey key))
                return latest.IsKeyPressed(key);
            return false;
        }

        public static bool IsKeyReleased(InputKey key)
        {
            return latest.IsKeyReleased(key);
        }

        public static bool IsKeyReleased(string id)
        {
            if (keyMaps.TryGetValue(id, out InputKey key))
                return latest.IsKeyReleased(key);
            return false;
        }

        public static void RegisterKeyMap(string id, InputKey key)
        {
            if (!keyMaps.ContainsKey(id))
                keyMaps.Add(id, key);
        }

        public static void UnregisterKeyMap(string id)
        { 
            if (keyMaps.ContainsKey(id))
                keyMaps.Remove(id);
        }

        public static InputKey KeyMapExists(string id)
        {
            if (keyMaps.TryGetValue(id, out InputKey key))
                return key;
            return InputKey.InvalidKey;
        }

        public static Dictionary<string, InputKey> GetKeyMaps()
        {
            return new Dictionary<string, InputKey>(keyMaps);
        }
    }
}