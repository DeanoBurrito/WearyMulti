using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace Weary.Scene.ObjectDB
{
    public sealed class ObjectDatabase
    {
        public static ObjectDatabase Global
        { get; private set; }

        internal static void ScanAndPopulate()
        {
            OdbTerminalCommands.RegisterCommands();
            Global = new ObjectDatabase(typeof(SceneNode));
        }

        private readonly ImmutableDictionary<string, OdbObject> objects;

        private ObjectDatabase(Type scanType)
        {
            List<Type> scanningTypes = new List<Type>();
            scanningTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes());

            ImmutableDictionary<string, OdbObject>.Builder objsBuilder = ImmutableDictionary.CreateBuilder<string, OdbObject>();
            foreach (Type t in scanningTypes)
            {
                if (scanType.IsAssignableFrom(t))
                {
                    OdbObject obj = new OdbObject(t);
                    objsBuilder.Add(obj.identifier, obj);
                }
            }

            objects = objsBuilder.ToImmutable();
            Log.WriteLine("ObjectDB found " + objects.Count + " usable objects within codebase.");
        }

        public bool ObjectExists(string name)
        {
            return objects.ContainsKey(name);
        }

        public bool ObjectHasField(string objName, string fieldName)
        {
            return objects.ContainsKey(objName) && objects[objName].fields.ContainsKey(fieldName);
        }

        public bool ObjectHasMethod(string objName, string methodName)
        { 
            return objects.ContainsKey(objName) && objects[objName].methods.ContainsKey(methodName);
        }

        public List<string> GetObjectNames()
        {
            return new List<string>(objects.Keys);
        }

        public OdbObject GetObject(string name)
        {
            if (objects.ContainsKey(name))
                return objects[name];
            Log.WriteError("ObjectDB error, unable to get object with name: " + name);
            return null;
        }

        public void SetField(string objName, string fieldName, object instance, object value)
        {
            if (!ObjectHasField(objName, fieldName))
            {
                Log.WriteError($"Cannot set field {objName}.{fieldName}, nothing with that signature exists.");
                return;
            }

            GetObject(objName).SetField(fieldName, instance, value);
        }

        public object GetField(string objName, string fieldName, object instance)
        { 
            if (!ObjectHasField(objName, fieldName))
            {
                Log.WriteError($"Cannot get field {objName}.{fieldName}, nothing with that signature exists.");
                return null;
            }

            return GetObject(objName).GetField(fieldName, instance);
        }

        public object CallMethod(string objName, string methodName, object instance, object[] args)
        {
            if (!ObjectHasMethod(objName, methodName))
            {
                Log.WriteError($"Cannot call method {objName}.{methodName}(), nothing with that signature exists.");
                return null;
            }

            return GetObject(objName).CallMethod(methodName, instance, args);
        }
    }
}