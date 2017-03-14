using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Persistity.Mappings;
using UnityEngine;

namespace Persistity.Serialization
{
    public class XmlDeserializer : IDeserializer<string>
    {
        private object DeserializePrimitive(Type type, XElement element)
        {
            if (type == typeof(byte)) { return byte.Parse(element.Value); }
            if (type == typeof(short)) { return short.Parse(element.Value); }
            if (type == typeof(int)) { return int.Parse(element.Value); }
            if (type == typeof(long)) { return long.Parse(element.Value); }
            if (type == typeof(bool)) { return bool.Parse(element.Value); }
            if (type == typeof(float)) { return float.Parse(element.Value); }
            if (type == typeof(double)) { return double.Parse(element.Value); }
            if (type == typeof(decimal)) { return decimal.Parse(element.Value); }
            if (type == typeof(Vector2))
            {
                var x = float.Parse(element.Element("x").Value);
                var y = float.Parse(element.Element("y").Value);
                return new Vector2(x, y);
            }
            if (type == typeof(Vector3))
            {
                var x = float.Parse(element.Element("x").Value);
                var y = float.Parse(element.Element("y").Value);
                var z = float.Parse(element.Element("z").Value);
                return new Vector3(x, y, z);
            }
            if (type == typeof(Vector4))
            {
                var x = float.Parse(element.Element("x").Value);
                var y = float.Parse(element.Element("y").Value);
                var z = float.Parse(element.Element("z").Value);
                var w = float.Parse(element.Element("w").Value);
                return new Vector4(x, y, z, w);
            }
            if (type == typeof(Quaternion))
            {
                var x = float.Parse(element.Element("x").Value);
                var y = float.Parse(element.Element("y").Value);
                var z = float.Parse(element.Element("z").Value);
                var w = float.Parse(element.Element("w").Value);
                return new Quaternion(x, y, z, w);
            }
            if (type == typeof(Guid))
            {
                return new Guid(element.Value);
            }
            if (type == typeof(DateTime))
            {
                var binaryTime = long.Parse(element.Value);
                return DateTime.FromBinary(binaryTime);
            }
            
            return element.Value;
        }

        public T DeserializeData<T>(TypeMapping typeMapping, string data) where T : new()
        {
            var xDoc = XDocument.Parse(data);
            var containerElement = xDoc.Element("Container");
            var instance = new T();
            Deserialize(typeMapping.InternalMappings, instance, containerElement);
            return instance;
        }

        private void DeserializeProperty<T>(PropertyMapping propertyMapping, T instance, XElement element)
        {
            var underlyingValue = DeserializePrimitive(propertyMapping.Type, element);
            propertyMapping.SetValue(instance, underlyingValue);
        }

        private void DeserializeNestedObject<T>(NestedMapping nestedMapping, T instance, XElement element)
        { Deserialize(nestedMapping.InternalMappings, instance, element); }

        private void DeserializeCollection(CollectionPropertyMapping collectionMapping, IList collectionInstance, int arrayCount, XElement element)
        {
            for (var i = 0; i < arrayCount; i++)
            {
                var collectionElement = element.Elements("CollectionElement").ElementAt(i);
                if (collectionMapping.InternalMappings.Count > 0)
                {
                    var elementInstance = Activator.CreateInstance(collectionMapping.CollectionType);
                    Deserialize(collectionMapping.InternalMappings, elementInstance, collectionElement);

                    if (collectionInstance.IsFixedSize)
                    { collectionInstance[i] = elementInstance; }
                    else
                    { collectionInstance.Insert(i, elementInstance); }
                }
                else
                {
                    var value = DeserializePrimitive(collectionMapping.CollectionType, collectionElement);
                    if (collectionInstance.IsFixedSize)
                    { collectionInstance[i] = value; }
                    else
                    { collectionInstance.Insert(i, value); }
                }
            }
        }

        private void Deserialize<T>(IEnumerable<Mapping> mappings, T instance, XElement element)
        {
            foreach (var mapping in mappings)
            {
                var currentElement = element.Element(mapping.LocalName);
                if (mapping is PropertyMapping)
                { DeserializeProperty((mapping as PropertyMapping), instance, currentElement); }
                else if (mapping is NestedMapping)
                {
                    var nestedMapping = (mapping as NestedMapping);
                    var childInstance = Activator.CreateInstance(nestedMapping.Type);
                    DeserializeNestedObject(nestedMapping, childInstance, currentElement);
                    nestedMapping.SetValue(instance, childInstance);
                }
                else
                {
                    var collectionMapping = (mapping as CollectionPropertyMapping);
                    var arrayCount = int.Parse(currentElement.Attribute("Count").Value);

                    if (collectionMapping.IsArray)
                    {
                        var arrayInstance = (IList) Activator.CreateInstance(collectionMapping.Type, arrayCount);
                        DeserializeCollection(collectionMapping, arrayInstance, arrayCount, currentElement);
                        collectionMapping.SetValue(instance, arrayInstance);
                    }
                    else
                    {
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(collectionMapping.CollectionType);
                        var listInstance = (IList)Activator.CreateInstance(constructedListType);
                        DeserializeCollection(collectionMapping, listInstance, arrayCount, currentElement);
                        collectionMapping.SetValue(instance, listInstance);
                    }
                }
            }
        }
    }
}