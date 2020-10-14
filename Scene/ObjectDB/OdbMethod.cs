using System;
using System.Collections.Generic;
using System.Reflection;

namespace Weary.Scene.ObjectDB
{
    public sealed class OdbMethod
    {
        public readonly string identifier;
        public readonly MethodInfo info;
        public readonly Type[] arguments;
        public readonly bool returnsData;
        public readonly Type returnType;

        public OdbMethod(MethodInfo mInfo)
        {
            info = mInfo;
            identifier = info.Name;
            List<Type> prams = new List<Type>();
            foreach (ParameterInfo pram in info.GetParameters())
            {
                prams.Add(pram.ParameterType);
            }
            arguments = prams.ToArray();

            if (mInfo.ReturnType == null)
                returnsData = false;
            else
            {
                returnsData = true;
                returnType = mInfo.ReturnType;
            }
        }

        public object CallMethod(object instance, object[] args)
        {
            if (instance == null)
            {
                Log.WriteError("Cannot call method on null instance.");
                return null;
            }
            
            object rtnObj = null;
            try
            {
                rtnObj = info.Invoke(instance, args);
            }
            catch (Exception e)
            {
                Log.WriteError($"Could not call method ({identifier}), error: " + e.ToString());
            }
            return rtnObj;
        }
    }
}