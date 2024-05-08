using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAR.Tool
{
	/// <summary>
	/// .net內置型別
	/// </summary>
	public class PrimitiveProcessor : SerializeProcessor
	{
		public PrimitiveProcessor()
		{
            //統一用ToString, 還原才有分別
            processTables = new Dictionary<Type, Func<string, object>>
            {
                { typeof(bool), (string value) => Convert.ToBoolean(value) },
                { typeof(byte), (string value) => Convert.ToByte(value) },
                { typeof(sbyte), (string value) => Convert.ToSByte(value) },
                { typeof(char), (string value) => Convert.ToChar(value) },
                { typeof(decimal), (string value) => Convert.ToDecimal(value) },
                { typeof(double), (string value) => Convert.ToDouble(value) },
                { typeof(float), (string value) => Convert.ToSingle(value) },
                { typeof(int), (string value) => Convert.ToInt32(value) },
                { typeof(uint), (string value) => Convert.ToUInt32(value) },
                { typeof(long), (string value) => Convert.ToInt64(value) },
                { typeof(ulong), (string value) => Convert.ToUInt64(value) },
                { typeof(short), (string value) => Convert.ToInt16(value) },
                { typeof(ushort), (string value) => Convert.ToUInt16(value) },
                { typeof(string), (string value) => value }
            };
        }

		Dictionary<Type,Func<string,object>> processTables = new Dictionary<Type, Func<string,object>> ();

		public override bool CanProcess (Type type)
		{
			return processTables.ContainsKey (type);
		}

		public override string ToSerialize (object obj, Type type)
		{
			return obj.ToString ();
		}

		public override object FromSerialize (string value, Type type)
		{
			Func<string,object> fromSerlizeFunc;
			
			if(processTables.TryGetValue(type, out fromSerlizeFunc))
			{
				object obj = fromSerlizeFunc.Invoke (value);

				return obj;
			}
			else
			{
				Debug.LogError ("not support type -> " + type.ToString ());
				
				return null;
			}
		}
	}
}
