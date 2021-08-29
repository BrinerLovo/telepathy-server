namespace Lovatto.Chicas
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class ChicasData : Dictionary<object, object>, ISerializable
    {
        public ChicasData()
        {
        }

        public ChicasData(int x) : base(x)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var item in this)
            {
                info.AddValue((string)item.Key, item.Value);
            }
        }

        // The special constructor is used to deserialize values.
        public ChicasData(SerializationInfo info, StreamingContext context)
        {
            foreach (var item in info)
            {
                this.Add(item.Name, item.Value);
            }
        }

        public object Clone()
        {
            return new Dictionary<object, object>(this);
        }

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