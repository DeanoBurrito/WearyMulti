using System;
using System.Collections.Immutable;
using System.Reflection;

namespace Weary.Scene.ObjectDB
{
    public sealed class OdbObject
    {
        public readonly string identifier;
        public readonly ImmutableDictionary<string, OdbField> fields;
        public readonly ImmutableDictionary<string, OdbMethod> methods;

        public OdbObject(Type objType)
        {
            identifier = objType.Name;
            
            ImmutableDictionary<string, OdbField>.Builder fieldsBuilder = ImmutableDictionary.CreateBuilder<string, OdbField>();
            foreach (FieldInfo f in objType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                OdbField field = new OdbField(f);
                fieldsBuilder.Add(field.identifier, field);
            }
            fields = fieldsBuilder.ToImmutable();

            ImmutableDictionary<string, OdbMethod>.Builder methodsBuilder = ImmutableDictionary.CreateBuilder<string, OdbMethod>();
            foreach (MethodInfo m in objType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                OdbMethod method = new OdbMethod(m);
                methodsBuilder.Add(method.identifier, method);
            }
            methods = methodsBuilder.ToImmutable();
            
            Log.WriteLine($"Object '{identifier}' has {fields.Count} mapped fields, {methods.Count} mapped methods.");
        }

        public object CallMethod(string methodName, object instance, object[] args)
        {
            if (instance == null)
            {
                Log.WriteError("Cannot call method on null instance.");
                return null;
            }
            
            if (methods.ContainsKey(methodName))
                return methods[methodName].CallMethod(instance, args);
            
            Log.WriteError("ObjectDB error, unable to call method: " + methodName);
            return null;
        }

        public void SetField(string fieldName, object instance, object value)
        {
            if (instance == null)
            {
                Log.WriteError("Cannot set field on null instance.");
                return;
            }

            if (fields.ContainsKey(fieldName))
                fields[fieldName].SetValue(instance, value);
        }

        public object GetField(string fieldName, object instance) => GetField<object>(fieldName, instance);

        public T GetField<T>(string fieldName, object instance)
        {
            if (instance == null)
            {
                Log.WriteError("Cannot get field on null instance.");
                return default(T);
            }

            if (fields.ContainsKey(fieldName) && typeof(T).IsAssignableFrom(fields[fieldName].type))
                return (T)fields[fieldName].GetValue(instance);
            
            Log.WriteError("ObjectDB error, unable to get field value: " + fieldName);
            return default(T);
        }
    }
}