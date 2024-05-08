using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAR.Tool
{
	/// <summary>
	/// unity內置型別
	/// </summary>
	public class UnityStructProcessor : SerializeProcessor
	{
		public UnityStructProcessor()
		{
			handleTypes = new List<Type>()
			{
				typeof(Vector3),
				typeof(Quaternion),
				typeof(Rect)
			};
		}

		List<Type> handleTypes;
		public override bool CanProcess(Type type)
		{
			return handleTypes.Contains(type);
		}

		public override string ToSerialize (object obj, Type type)
		{
			if (type == typeof(Quaternion)) 
			{
				//轉成euler增加可讀性
				Vector3 euler = ((Quaternion)obj).eulerAngles;

				return ToJson (euler);
			} 
			else if (type == typeof(Rect))
			{
				return ToSerialize_Rect (obj);
			}
			else
			{
				return ToJson (obj);
			}
		}

		public override object FromSerialize (string value, Type type)
		{
			object obj = FromJson (value, typeof(Vector3));
			
			if (type == typeof(Quaternion)) 
			{
				//當初用euler存的,要轉回來
				Vector3 euler = (Vector3)obj;

				Quaternion quaternion = Quaternion.Euler (euler);

				return quaternion;
			}
			else if (type == typeof(Rect))
			{
				return FromSerialize_Rect (value);
			}
			else
			{
				return obj;
			}
		}

        /// <summary>
        /// Rect不能ToJson
        /// </summary>
        /// <returns>The serialize rect.</returns>
        /// <param name="obj">Object.</param>
        string ToSerialize_Rect (object obj)
		{
			Rect rect = (Rect)obj;

			List<float> values = new List<float> (){ rect.position.x, rect.position.y, rect.size.x, rect.size.y };

			//回去mgr用List<float>存
			return SerializeManager.ToSerialize (values, typeof(List<float>));
		}

        /// <summary>
        /// Rect是用List<float>存
        /// </summary>
        /// <returns>The serialize rect.</returns>
        /// <param name="value">Value.</param>
        object FromSerialize_Rect (string value)
		{
			List<float> values = (List<float>)SerializeManager.FromSerialize (value, typeof(List<float>));

			if (values.Count == 4) 
			{
				Vector2 position = new Vector2 (values [0], values [1]);

				Vector2 size = new Vector2 (values [2], values [3]);

				return new Rect (position, size);
			}
			else
			{
				//存取得長度不對
				StringBuilder builder = new StringBuilder();

				for (int i = 0; i < values.Count; i++) 
				{
					if (i != 0) 
					{
						builder.Append (",");
					}

					builder.Append (values [i].ToString ());
				}

				Debug.LogError ("values -> " + builder.ToString());

				return new Rect ();
			}
		}

		protected string ToJson(object obj)
		{
			return JsonUtility.ToJson(obj);
		}

		protected object FromJson(string value, Type type)
		{
			return JsonUtility.FromJson(value, type);
		}
	}
}
