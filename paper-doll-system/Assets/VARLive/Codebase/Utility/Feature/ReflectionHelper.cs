using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ReflectionHelper
{
    public static List<Type> FindClassesImplementingInterface(Type interfaceType)
    {
        var implementingClasses = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (interfaceType.IsAssignableFrom(type) && !type.IsInterface)
                    {
                        implementingClasses.Add(type);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException.Message);
                }
            }
        }

        return implementingClasses;
    }

    public static List<Type> FindSubClassTypes<T>()
    {
        List<Type> result = new List<Type>();

        // 獲取所有已載入的組件
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 遍歷所有組件，尋找 T 的子類別
        foreach (var assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            var subClasses = (from Type type in types where type.IsSubclassOf(typeof(T)) select type).ToList();
            if (subClasses.Count > 0)
            {
                result.AddRange(subClasses);
            }
        }
        return result;
    }
}
