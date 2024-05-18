using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Tool
{
    public class Serializer
	{
		private List<SerializeProcessor> processors = new List<SerializeProcessor>();

		public void Add(SerializeProcessor processor)
		{
			if (processors.Contains(processor) == false)
			{
				processors.Add(processor);
			}
		}

		public void Remove(SerializeProcessor processor)
		{
			if (processors.Contains(processor))
			{
				processors.Remove(processor);
			}
		}

		public string ToSerialize (object obj, Type type)
		{
			var findProcessor = processors.Find (processor => processor.CanProcess (type));
			if (findProcessor != null) 
			{
				return findProcessor.ToSerialize (obj, type);
			}
			else
			{
				Debug.LogError ("不支援的 type, -> " + type.ToString ());
				return "";
			}
		}

		public object FromSerialize (string value, Type type)
		{
			var findProcessor = processors.Find (processor => processor.CanProcess (type));
			if (findProcessor != null) 
			{
				return findProcessor.FromSerialize (value, type);
			}
			else
			{
				Debug.LogError ("不支援的 type, -> " + type.ToString ());
				return null;
			}
		}
	}

	public static class SerializeExtension
	{
		public static bool IsArrayOrList(this Type type)
		{
			if (type.IsArray)
			{
				//是個array
				return true;
			}
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				//是個list
				return true;
			}
			return false;
		}

		public static bool IsStruct(this Type type)
        {
			if (type.IsValueType == false)
				return false;

			if (type.IsPrimitive)
				return false;

			if (type.IsEnum)
				return false;

			return true;
		}

		public static Type GetArrayOrListElementType(this Type arrayOrList)
		{
			//內部每個物件的type
			Type elementType = default(Type);

			//array不是泛型, 所以要先判斷
			if (arrayOrList.IsArray)
			{
				elementType = arrayOrList.GetElementType();
			}
			else if (arrayOrList.IsGenericType && arrayOrList.GetGenericTypeDefinition() == typeof(List<>))
			{
				//是個list
				elementType = arrayOrList.GetGenericArguments()[0];
			}

			return elementType;
		}
	}
}