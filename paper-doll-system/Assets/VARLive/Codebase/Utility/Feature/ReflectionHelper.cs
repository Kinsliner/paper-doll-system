using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ReflectionHelper
{
    /// <summary>
    /// 取得所有繼承自指定介面的類別
    /// </summary>
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

    /// <summary>
    /// 取得所有繼承自指定類別的子類別
    /// </summary>
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

    /// <summary>
    /// 取得所有標記了指定屬性的類別
    /// </summary>
    public static List<Type> FindClassesWithAttribute<T>() where T : Attribute
    {
        var types = new List<Type>();

        // 獲取所有已加載的程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 遍歷每個程序集中的類型
        foreach (var assembly in assemblies)
        {
            Type[] assemblyTypes;
            try
            {
                // 在一些情況下，對於匿名程序集，可能會抛出異常
                assemblyTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 如果有異常，繼續處理下一個程序集
                assemblyTypes = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (var type in assemblyTypes)
            {
                // 檢查類型是否標記了特定的屬性
                if (type.GetCustomAttributes(typeof(T), true).Any())
                {
                    types.Add(type);
                }
            }
        }

        return types;
    }

    public static Dictionary<Type, List<Attribute>> FindClassesWithAttributes<T>() where T : Attribute
    {
        var typesWithAttributes = new Dictionary<Type, List<Attribute>>();

        // 獲取所有已加載的程序集
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 遍歷每個程序集中的類型
        foreach (var assembly in assemblies)
        {
            Type[] assemblyTypes;
            try
            {
                // 在一些情況下，對於匿名程序集，可能會抛出異常
                assemblyTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 如果有異常，繼續處理下一個程序集
                assemblyTypes = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (var type in assemblyTypes)
            {
                // 檢查類型是否標記了特定的屬性
                var attributes = type.GetCustomAttributes(typeof(T), true);
                if (attributes.Any())
                {
                    if (!typesWithAttributes.ContainsKey(type))
                    {
                        typesWithAttributes.Add(type, new List<Attribute>());
                    }

                    typesWithAttributes[type].AddRange(attributes.Cast<Attribute>());
                }
            }
        }

        return typesWithAttributes;
    }
}
