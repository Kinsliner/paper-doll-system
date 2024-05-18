using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Tool
{
    public class EnumProcessor : SerializeProcessor
    {
		public override bool CanProcess (Type type)
		{
			return type.IsEnum;
		}

		public override object FromSerialize (string value, Type type)
		{
			return Enum.Parse (type, value);
		}

		public override string ToSerialize (object obj, Type type)
		{
			return obj.ToString ();
		}
	}
}