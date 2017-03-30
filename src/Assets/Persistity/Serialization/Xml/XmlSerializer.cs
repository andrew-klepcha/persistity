using System;
using System.Xml.Linq;
using Persistity.Extensions;
using Persistity.Registries;
using Persistity.Serialization.Binary;
using UnityEngine;

namespace Persistity.Serialization.Xml
{
    public class XmlSerializer : GenericSerializer<XElement, XElement>, IXmlSerializer
    {
        public XmlSerializer(IMappingRegistry mappingRegistry, XmlConfiguration configuration = null) : base(mappingRegistry)
        {
            Configuration = configuration ?? XmlConfiguration.Default;
        }

        private readonly Type[] CatchmentTypes =
        {
            typeof(string), typeof(bool), typeof(byte), typeof(short), typeof(int),
            typeof(long), typeof(Guid), typeof(float), typeof(double), typeof(decimal)
        };

        public override void HandleNullData(XElement state)
        { state.Add(new XAttribute("IsNull", true)); }

        public override void HandleNullObject(XElement state)
        { state.Add(new XAttribute("IsNull", true)); }

        public override void AddCountToState(XElement state, int count)
        { state.Add(new XAttribute("Count", count)); }
        
        public override void SerializeDefaultPrimitive(object value, Type type, XElement element)
        {
            if (type == typeof(Vector2))
            {
                var typedObject = (Vector2)value;
                element.Add(new XElement("x", typedObject.x));
                element.Add(new XElement("y", typedObject.y));
                return;
            }
            if (type == typeof(Vector3))
            {
                var typedObject = (Vector3)value;
                element.Add(new XElement("x", typedObject.x));
                element.Add(new XElement("y", typedObject.y));
                element.Add(new XElement("z", typedObject.z));
                return;
            }
            if (type == typeof(Vector4))
            {
                var typedObject = (Vector4)value;
                element.Add(new XElement("x", typedObject.x));
                element.Add(new XElement("y", typedObject.y));
                element.Add(new XElement("z", typedObject.z));
                element.Add(new XElement("w", typedObject.w));
                return;
            }
            if (type == typeof(Quaternion))
            {
                var typedObject = (Quaternion)value;
                element.Add(new XElement("x", typedObject.x));
                element.Add(new XElement("y", typedObject.y));
                element.Add(new XElement("z", typedObject.z));
                element.Add(new XElement("w", typedObject.w));
                return;
            }
            if (type == typeof(DateTime))
            {
                var typedValue = (DateTime)value;
                var stringValue = typedValue.ToBinary().ToString();
                element.Value = stringValue;
                return;
            }

            if (type.IsTypeOf(CatchmentTypes) || type.IsEnum)
            {
                element.Value = value.ToString();
                return;
            }
        }

        public override DataObject Serialize(object data)
        {
            var element = new XElement("Container");
            var dataType = data.GetType();
            var typeMapping = MappingRegistry.GetMappingFor(dataType);
            Serialize(typeMapping.InternalMappings, data, element);

            var typeElement = new XElement("Type", dataType.GetPersistableName());
            element.Add(typeElement);
            
            var xmlString = element.ToString();
            return new DataObject(xmlString);
        }
    }
}