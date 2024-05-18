using System;
using UnityEngine;

namespace Ez.Tool
{
    public class JsonProcessor : SerializeProcessor
    {
		public override bool CanProcess(Type type)
        {
			return true;
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