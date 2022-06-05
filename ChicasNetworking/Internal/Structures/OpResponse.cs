using System;
using Telepathy;

namespace Lovatto.Chicas
{
    [Serializable]
    public class OpResponse
    {
        public short Code;
        public string Message = "1";
        public string Error = "1";
        public object[] Params;

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="obj"></param>
        public void AddParam(object obj)
        {
            if (Params == null)
            {
                Params = new object[1];
                Params[0] = obj;
                return;
            }

            Params.Append(obj);
        }

        /// <summary>
        /// Get parameter from list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetParameter(int index)
        {
            if (index < 0 || Params == null) return null;
            if (index > Params.Length - 1)
            {
                Log.Error("Parameter index out of range.");
                return null;
            }
            return Params[index];
        }
    }
}