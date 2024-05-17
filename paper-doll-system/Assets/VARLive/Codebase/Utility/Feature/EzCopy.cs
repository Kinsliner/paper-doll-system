using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ez
{
    public  static class EzCopy
    {
        public static T Copy<T>(T source)
        {
            if(typeof(T).IsSerializable == false)
            {
                throw new ArgumentException($"{nameof(EzCopy)} => 此物件無法複製");
            }

            if(ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using(stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
