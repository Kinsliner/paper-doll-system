using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VAR.Tool
{
	public abstract class SerializeProcessor
	{
		public abstract bool CanProcess (Type type);

		public abstract string ToSerialize (object obj, Type type);

		public abstract object FromSerialize (string value, Type type);
	}
}