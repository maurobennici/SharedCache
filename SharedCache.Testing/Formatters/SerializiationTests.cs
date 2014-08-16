using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NUnit.Framework;
using SharedCache.Testing.HelperObjects.SerializeAttribute;
using SharedCache.WinServiceCommon.Formatters;

namespace SharedCache.Testing.Formatters
{   

    [TestFixture]
    public class SerializiationTests : BaseTest
    {
        [Test]
        public void BinarySerializeTest()
        {
            var person1 = new HelperObjects.SerializeAttribute.Person()
            {
                Salutation = "MR",
                FirstName = "Bob",
                LastName = "Smith",
                Age = 28
            };

            var person1Bytes = Serialization.BinarySerialize(person1);

            Expect(person1Bytes, Is.Not.Null);
            Expect(person1Bytes.Length, Is.GreaterThan(0));

            var ms = new MemoryStream(person1Bytes);
            var bf = new BinaryFormatter();

            var person2 = bf.Deserialize(ms) as Person;

            Expect(person2, Is.Not.Null);
            Expect(person2.Salutation, Is.EqualTo(person1.Salutation));
            Expect(person2.FirstName, Is.EqualTo(person1.FirstName));
            Expect(person2.LastName, Is.EqualTo(person1.LastName));
            Expect(person2.Age, Is.EqualTo(person1.Age));
        }
    }
}
