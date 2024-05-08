using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VAR.Tool;

public static class SerializeManager
{
    private static readonly Serializer serializer = new Serializer();

	public static void SetupDefaultProcessors()
	{
		serializer.Add(new EnumProcessor());
		serializer.Add(new PrimitiveProcessor());
		serializer.Add(new UnityStructProcessor());
		serializer.Add(new StructProcessor());
		serializer.Add(new GenericProcessor());
		serializer.Add(new JsonProcessor());
	}

	public static void Add(SerializeProcessor processor)
	{
		serializer.Add(processor);
	}

	public static void Remove(SerializeProcessor processor)
	{
		serializer.Remove(processor);
	}

	public static string ToSerialize(object obj, Type type)
	{
		return serializer.ToSerialize(obj, type);
	}

	public static object FromSerialize(string value, Type type)
	{
		return serializer.FromSerialize(value, type);
	}
}
