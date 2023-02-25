using System;

namespace GeNSIS.Core.Serialization
{
    public class DeSerializationProvider
    {
        public IDeSerializer GetDeSerializerByExtension(string pExtension)
        {
            switch(pExtension.ToLower())
            {
                case ".json": return new JsonDeSerializer();
                case ".xml": return new XmlDeSerializer();
            }

            throw new ArgumentException($"Extension '{pExtension}' is unknown, illegal or no {nameof(IDeSerializer)} has been implemented for it yet!", nameof(pExtension));
        }

        public IDeSerializer GetDeSerializerByEnum(EDeSerializers pDeSerializerEnum)
        {
            switch (pDeSerializerEnum)
            {
                case EDeSerializers.JSON: return new JsonDeSerializer();
                case EDeSerializers.XML: return new XmlDeSerializer();
            }

            throw new NotImplementedException($"{nameof(IDeSerializer)} for {nameof(EDeSerializers)}.{pDeSerializerEnum} is not implemented!");
        }
    }
}
