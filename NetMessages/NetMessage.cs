using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetMessages
{
    [Serializable]
    public abstract class NetMessage
    {
        public static byte[] SerializeToBytes<TData>(TData messageObject)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, messageObject);
                return stream.ToArray();
            }
        }

        public static TData DeserializeFromBytes<TData>(byte[] messageBytes)
        {            
            using (var stream = new MemoryStream(messageBytes))
            {
                var formatter = new BinaryFormatter();                
                return (TData)formatter.Deserialize(stream);
            }
        }
    }
}
