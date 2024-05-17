using System.Collections.Generic;
using System;
using UnityEngine;

namespace Ez
{
    [Serializable]
    public class SerializeDic<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            Clear();
            if (keys.Count != values.Count)
                throw new Exception(string.Format("There are {0} keys and {1} values after deserialization."
                    + "Make sure that both key and value types are serializable.", keys.Count, values.Count));

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public SerializeDic<TKey, TValue> Clone()
        {
            SerializeDic<TKey, TValue> clone = new SerializeDic<TKey, TValue>();
            foreach (var pair in this)
            {
                clone.Add(pair.Key, pair.Value);
            }
            return clone;
        }
    }
}

