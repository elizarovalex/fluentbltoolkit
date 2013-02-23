﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using BLToolkit.Mapping;

namespace BLToolkit.Fluent.MemberMappers
{
    public class BinarySerialisationMapper : MemberMapper
    {
        public override void SetValue(object o, object value)
        {
            if (value != null) this.MemberAccessor.SetValue(o, this.binarydeserialize((byte[])o));                                
        }

        public override object GetValue(object o)
        {
            return this.binaryserialize(this.MemberAccessor.GetValue(o));                        
        }        

        private byte[] binaryserialize(object obj)
        {
            if (obj == null) return null;
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            memoryStream.Flush();
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        private object binarydeserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
}
