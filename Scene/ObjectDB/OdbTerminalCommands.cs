using System;
using System.Collections.Generic;
using Weary.Debug;

namespace Weary.Scene.ObjectDB
{
    internal static class OdbTerminalCommands
    {
        [DebugCommandLoader]
        internal static void RegisterCommands()
        {
            DebugTerminal.RegisterCommand("lsdb", "Displays the current contents of the ObjectDB.", LsDb);
            DebugTerminal.RegisterCommand("lsobj", "Displays details on a specific OdbObject.", LsObj);
            DebugTerminal.RegisterCommand("get", "Gets a field value using it's name and instance reference on scenetree.", GetField);
            DebugTerminal.RegisterCommand("set", "Sets a field value using it's name, instance reference and new value.", SetField);
        }

        public static void LsDb(string[] args)
        {
            bool showDetails = false;
            foreach (string a in args)
            {
                if (a == "-l")
                    showDetails = true;
            }

            List<string> objNames = ObjectDatabase.Global.GetObjectNames();
            foreach (string objName in objNames)
            {
                OdbObject obj = ObjectDatabase.Global.GetObject(objName);
                if (obj == null)
                {
                    Log.WriteLine($"OdbObject {objName} had no actually object, despite being named.");
                    continue;
                }
                Log.WriteLine($"OdbObject {objName} has {obj.fields.Count} fields and {obj.methods.Count} methods.");
                if (showDetails)
                {
                    foreach (OdbField field in obj.fields.Values)
                    {
                        Log.WriteLine($"    | FIELD is={field.type} name={field.identifier}");
                    }

                    foreach (OdbMethod method in obj.methods.Values)
                    {
                        string argsStr = string.Join<Type>(',', method.arguments);
                        Log.WriteLine($"    | METHOD rtns={(method.returnsData ? method.returnType.ToString() : "null")} name={method.identifier} args=({argsStr})");
                    }
                }
            }
        }

        public static void LsObj(string[] args)
        {
            if (args.Length != 1)
            {
                Log.WriteError("Expected OdbObject name.");
                return;
            }

            OdbObject obj = ObjectDatabase.Global.GetObject(args[0]);
            if (obj == null)
            {
                Log.WriteError("No OdbObject with that name.");
                return;
            }

            Log.WriteLine($"OdbObject {obj.identifier} has {obj.fields.Count} fields and {obj.methods.Count} methods.");
            foreach (OdbField field in obj.fields.Values)
            {
                Log.WriteLine($"    | FIELD is={field.type} name={field.identifier}");
            }

            foreach (OdbMethod method in obj.methods.Values)
            {
                string argsStr = string.Join<Type>(',', method.arguments);
                Log.WriteLine($"    | METHOD rtns={(method.returnsData ? method.returnType.ToString() : "null")} name={method.identifier} args=({argsStr})");
            }
        }

        public static void GetField(string[] args)
        {
            if (args.Length != 2)
            {
                Log.WriteError("Expected field name, instance ref (uuid or path)");
                return;
            }

            object instanceRef;
            ulong id = ulong.MaxValue;
            if (ulong.TryParse(args[1], out id))
                instanceRef = SceneTree.GetCurrent().GetNode(id);
            else
                instanceRef = SceneTree.GetCurrent().GetNode(args[1]);
            if (instanceRef == null)
            {
                Log.WriteError("Could not find node by " + (id == ulong.MaxValue ? "name" : "id")+ " '" + args[1] + "'");
                return;
            }

            string objectName = instanceRef.GetType().Name;
            objectName = objectName.Remove(0, objectName.LastIndexOf('.') + 1);
            object fieldValue = ObjectDatabase.Global.GetField("SceneNode", args[0], instanceRef);

            string line = args[1] + "." + args[0] + ": value=";
            if (fieldValue == null)
                line +="null";
            else
                line += fieldValue.ToString();
            Log.WriteLine(line);
        }

        public static void SetField(string[] args)
        {
            if (args.Length != 3)
            {
                Log.WriteLine("Expected field name, instance ref (uuid or path), and value");
                return;
            }

            object instanceRef;
            ulong id = ulong.MaxValue;
            if (ulong.TryParse(args[1], out id))
                instanceRef = SceneTree.GetCurrent().GetNode(id);
            else
                instanceRef = SceneTree.GetCurrent().GetNode(args[1]);
            if (instanceRef == null)
            {
                Log.WriteError("Could not find node by " + (id == ulong.MaxValue ? "name" : "id")+ " '" + args[1] + "'");
                return;
            }

            string objectName = instanceRef.GetType().Name;
            objectName = objectName.Remove(0, objectName.LastIndexOf('.') + 1);
            ObjectDatabase.Global.SetField(objectName, args[0], instanceRef, args[2]);

            Log.WriteLine(args[1] + "." + args[0] + ": value=" + args[2]);
        }
    }
}