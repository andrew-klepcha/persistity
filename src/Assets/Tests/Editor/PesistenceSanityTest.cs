﻿using System;
using NUnit.Framework;
using Persistity.Mappings;
using Persistity.Serialization;
using Tests.Editor.Models;

namespace Assets.Tests.Editor.Persistence
{
    [TestFixture]
    public class PesistenceSanityTest
    {
        private TypeMapper _typeMapper = new TypeMapper();

        private A GenerateDummyData()
        {
            var a = new A();
            a.TestValue = "WOW";
            a.Stuff.Add("woop");
            a.Stuff.Add("poow");
            a.NestedValue = new B();
            a.NestedValue.IntValue = 10;
            a.NestedValue.StringValue = "Hello";
            a.NestedValue.NestedArray = new[] { new C { FloatValue = 2.43f } };
            a.NestedArray = new B[2];
            a.NestedArray[0] = new B
            {
                IntValue = 20,
                StringValue = "There",
                NestedArray = new[] { new C { FloatValue = 3.5f } }
            };
            a.NestedArray[1] = new B
            {
                IntValue = 30,
                StringValue = "Sir",
                NestedArray = new[]
                {
                     new C { FloatValue = 4.1f },
                     new C { FloatValue = 5.2f }
                }
            };

            return a;
        }

        private void AssertionOnDummyData(A result)
        {
            Assert.That(result.TestValue, Is.EqualTo("WOW"));
            Assert.That(result.NestedValue, Is.Not.Null);
            Assert.That(result.NestedValue.IntValue, Is.EqualTo(10));
            Assert.That(result.NestedValue.StringValue, Is.EqualTo("Hello"));
            Assert.That(result.NestedValue.NestedArray, Is.Not.Null);
            Assert.That(result.NestedValue.NestedArray.Length, Is.EqualTo(1));
            Assert.That(result.NestedValue.NestedArray[0].FloatValue, Is.EqualTo(2.43f));
            Assert.That(result.NestedArray, Is.Not.Null);
            Assert.That(result.NestedArray.Length, Is.EqualTo(2));
            Assert.That(result.NestedArray[0].IntValue, Is.EqualTo(20));
            Assert.That(result.NestedArray[0].StringValue, Is.EqualTo("There"));
            Assert.That(result.NestedArray[0].NestedArray, Is.Not.Null);
            Assert.That(result.NestedArray[0].NestedArray.Length, Is.EqualTo(1));
            Assert.That(result.NestedArray[0].NestedArray[0].FloatValue, Is.EqualTo(3.5f));
            Assert.That(result.NestedArray[1].IntValue, Is.EqualTo(30));
            Assert.That(result.NestedArray[1].StringValue, Is.EqualTo("Sir"));
            Assert.That(result.NestedArray[1].NestedArray, Is.Not.Null);
            Assert.That(result.NestedArray[1].NestedArray.Length, Is.EqualTo(2));
            Assert.That(result.NestedArray[1].NestedArray[0].FloatValue, Is.EqualTo(4.1f));
            Assert.That(result.NestedArray[1].NestedArray[1].FloatValue, Is.EqualTo(5.2f));
        }

        [Test]
        public void should_serialize_with_debug_serializer()
        {
            var a = GenerateDummyData();
            var typeStuff = _typeMapper.GetTypeMappingsFor(typeof(A));

            var serializer = new DebugSerializer();

            var output = serializer.SerializeData(typeStuff, a);
            Console.WriteLine(output);
        }

        [Test]
        public void should_correctly_serialize_with_json()
        {
            var a = GenerateDummyData();
            var typeStuff = _typeMapper.GetTypeMappingsFor(typeof(A));

            var serializer = new JsonSerializer();
            var jsonOutput = serializer.SerializeData(typeStuff, a);
            Console.WriteLine(jsonOutput);

            var deserializer = new JsonDeserializer();
            var result = deserializer.DeserializeData<A>(typeStuff, jsonOutput);

            AssertionOnDummyData(result);
        }
        
        [Test]
        public void should_correctly_serialize_with_binary()
        {
            var a = GenerateDummyData();
            var typeStuff = _typeMapper.GetTypeMappingsFor(typeof(A));

            var serializer = new BinarySerializer();
            var binaryOutput = serializer.SerializeData(typeStuff, a);
            Console.WriteLine(BitConverter.ToString(binaryOutput));

            var deserializer = new BinaryDeserializer();
            var result = deserializer.DeserializeData<A>(typeStuff, binaryOutput);

            AssertionOnDummyData(result);
        }
    }
}