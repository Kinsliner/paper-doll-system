using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAR.Tool
{
	/// <summary>
	/// .net內置型別
	/// </summary>
	public class GenericProcessor : SerializeProcessor
	{
		public override bool CanProcess (Type type)
		{
			return type.IsArrayOrList();
		}

		public override string ToSerialize (object obj, Type type)
		{
			//內部每個物件的type
			Type elementType = type.GetArrayOrListElementType();

			//統一存成List<string>
			List<string> values = new List<string> ();

			IEnumerable iter = (IEnumerable)obj;

			foreach (var itemValue in iter) 
			{
				//讓mgr去把單1物件轉成string再拿回來
				string stringValue = SerializeManager.ToSerialize (itemValue, elementType);

				values.Add (stringValue);
			}

			ObjData objData = new ObjData (){ values = values };

			//unity json不能直接存list
			return JsonUtility.ToJson (objData);
		}

		public override object FromSerialize (string value, Type type)
		{
			ObjData objData = JsonUtility.FromJson<ObjData>(value);

			//統一存成List<string>
			List<string> values = objData.values;

			List<object> objs;

			
			//內部每個物件的type
			Type elementType;

			//array不是泛型, 所以要先判斷
			if (type.IsArray) 
			{
				elementType = type.GetElementType ();
				objs = values.ConvertAll (elementValue => SerializeManager.FromSerialize (elementValue, elementType));

				ArrayList arrayList = new ArrayList ();
				objs.ForEach (obj => arrayList.Add (obj));
				Array stronglyArr = arrayList.ToArray (elementType);

				return stronglyArr;

			} 
			else if (type.IsGenericType && type.GetGenericTypeDefinition () == typeof(List<>))
			{
				//是個list
				elementType = type.GetGenericArguments()[0];
				objs = values.ConvertAll (elementValue => SerializeManager.FromSerialize (elementValue, elementType));

				Type listType = typeof(List<>).MakeGenericType (elementType);
				IList iList = (IList)Activator.CreateInstance (listType);

				objs.ForEach (obj => iList.Add (obj));

				return iList;
			}

			return null;
		}

		enum GenericType
		{
			Array,List
		}

		[Serializable]
		class ObjData
		{
			public List<string> values;
		}
	}
}
