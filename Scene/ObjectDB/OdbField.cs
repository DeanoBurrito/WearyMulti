using System;
using System.Reflection;

namespace Weary.Scene.ObjectDB
{
    public sealed class OdbField
    {
        public readonly string identifier;
        public readonly FieldInfo info;
        public readonly Type type;

        public OdbField(FieldInfo fInfo)
        {
            info = fInfo;
            identifier = info.Name;
            type = info.FieldType;
        }

        public void SetValue(object instance, object value)
        {
            if (instance == null)
            {
                Log.WriteError("Cannot set field on null instance.");
                return;
            }

            try
            {
                info.SetValue(instance, value);
            }
            catch (Exception e)
            {
                Log.WriteError($"Could not set field ({identifier}) value, error: " + e.ToString());
            }
        }

        public object GetValue(object instance)
        {
            if (instance == null)
            {
                Log.WriteError("Cannot get field on null instance.");
                return null;
            }
            
            try
            {
                return info.GetValue(instance);
            }
            catch (Exception e)
            {
                Log.WriteError($"Could not get field ({identifier}) value, error: " + e.ToString());
            }
            return null;
        }
    }
}