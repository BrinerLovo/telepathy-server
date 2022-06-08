using Lovatto.Chicas.Internal;

namespace Lovatto.Chicas
{
    /// <summary>
    /// Inherit this to implement a custom serialiable class.
    /// </summary>
    public interface ICustomSerializable
    {
        /// <summary>
        /// Use to write your serialized class.
        /// </summary>
        /// <param name="writer"></param>
        void Write(EndianBinaryWriter writer);

        /// <summary>
        /// Use to read your class from the binary data.
        /// </summary>
        /// <param name="reader"></param>
        void Read(EndianBinaryReader reader);
    }
}
