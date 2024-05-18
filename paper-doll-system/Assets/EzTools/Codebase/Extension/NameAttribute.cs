using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NameAttribute : Attribute
{
    public string Name { get; private set; }

    public NameAttribute(string name)
    {
        Name = name;
    }
}

public static class NameExtension
{
    public static string GetName(this Type type)
    {
        var nameAttribute = type.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
        if (nameAttribute != null)
        {
            return nameAttribute.Name;
        }
        return type.Name;
    }
}