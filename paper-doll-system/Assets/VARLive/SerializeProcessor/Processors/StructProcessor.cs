using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAR.Tool
{
    public class StructProcessor : SerializeProcessor
    {
		public override bool CanProcess (Type type)
		{
			return type.IsStruct();
		}

		public override object FromSerialize(string value, Type type)
		{
			return JsonUtility.FromJson(value, type);
		}

		public override string ToSerialize(object obj, Type type)
		{
			return JsonUtility.ToJson(obj);
		}
	}
}