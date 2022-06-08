namespace Lovatto.Chicas
{
    using Lovatto.Chicas.Internal;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class ChicasData : Dictionary<object, object>, ISerializable, IDictionary<object, object>, ICustomSerializable
    {
        public ChicasData() { }

        public ChicasData(IDictionary<object, object> source) : base(source)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var item in this)
            {
                info.AddValue((string)item.Key, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Dictionary<object, object>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> list = new List<string>();
            foreach (object obj2 in base.Keys)
            {
                if ((obj2 == null) || (this[obj2] == null))
                {
                    list.Add(obj2 + "=" + this[obj2]);
                }
                else
                {
                    list.Add(string.Concat(new object[] { "(", obj2.GetType(), ")", obj2, "=(", this[obj2].GetType(), ")", this[obj2] }));
                }
            }
            return string.Join(", ", list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void Read(EndianBinaryReader reader)
        {
            var dic = reader.ReadDictionary();
            foreach (var item in dic)
            {
                if (ContainsKey(item.Key))
                {
                    this[item.Key] = item.Value;
                }
                else
                {
                    this.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new public object this[object key]
        {
            get
            {
                object obj;
                TryGetValue(key, out obj);
                return obj;
            }
            set
            {
                base[key] = value;
            }
        }
    }
}