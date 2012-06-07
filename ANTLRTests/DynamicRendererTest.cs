using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using ANTLR;
using NUnit.Framework;
using Antlr4.StringTemplate;


namespace ANTLRTests
{
    [TestFixture]
    public class DynamicRendererTest
    {
        [Test]
        public void NestedObjectDictionaryWithStringTemplate()
        {
            var tmp = new { parent = new { child = new Dictionary<string, string> { { "foo", "I should get this" }, { "bar", "And This" } } } };

            Template template = new Template("<x.parent.child.foo>&<x.parent.child.bar>");

            template.Add("x", tmp);
            Assert.That(template.Render(), Is.EqualTo(tmp.parent.child["foo"] + "&" + tmp.parent.child["bar"]));
        }

        [Test]
        public void NestedObjectsWithStringTemplate()
        {
            var tmp = new { parent = new { child = new { foo = "I should get this"} } };

            Template template = new Template("<x.parent.child.foo>");

            template.Add("x", tmp);
            Assert.That(template.Render(), Is.EqualTo(tmp.parent.child.foo));
        }

        [Test]
        public void ObjectsWithStringTemplate()
        {
            dynamic tmp = new { foo = "I should get this" };
            Template template = new Template("<x.foo>");

            template.Add("x", tmp);
            Assert.That(template.Render(), Is.EqualTo(tmp.foo));
        }

        [Test]
        public void testDynamicObjectness()
        {
            Datagroup datagroup = new Datagroup();
            EntityAttribute attribute = new EntityAttribute("foo", "bar");
            datagroup.Attributes.Add(attribute);
            dynamic renderer = new DynamicRenderer(datagroup);
            Assert.That(renderer.foo, Is.EqualTo(attribute.Value));
        }

        [Test]
        public void DynamicObjectWithStringTemplateDoesNotAppearToWork()
        {
            Datagroup datagroup = new Datagroup();
            EntityAttribute entityAttribute = new EntityAttribute { Name = "foo", Value = "Why don't I get this?" };
            datagroup.Attributes.Add(entityAttribute);
            dynamic dynamicRenderer = new DynamicRenderer(datagroup);

            var expando = new ExpandoObject();
            var expandoDictionary = (IDictionary<string, object>)expando;
            expandoDictionary["foo"] = "something";
            Dictionary<string, string> dd = new Dictionary<string, string>();
            dd["foo"] = "something";
            Template template = new Template("<x.foo>");
            template.Add("x", dynamicRenderer);

            Assert.That(template.Render(), Is.EqualTo(entityAttribute.Value));
        }

        [Test]
        public void CustomDictionayImplementationWithStringTemplate()
        {
            Datagroup datagroup = new Datagroup();
            EntityAttribute entityAttribute = new EntityAttribute { Name = "foo", Value = "I hate VS" };
            datagroup.Attributes.Add(entityAttribute);
            var dictionary = new DatagroupAttributeDictionary(datagroup);

            var expando = new ExpandoObject();
            var expandoDictionary = (IDictionary<string, object>)expando;
            expandoDictionary["foo"] = "something";
            Dictionary<string, string> dd = new Dictionary<string, string>();
            dd["foo"] = "something";
            Template template = new Template("<x.foo>");
            template.Add("x", dictionary);

            Assert.That(template.Render(), Is.EqualTo(entityAttribute.Value));
        }

        [Test]
        public void testDynamicObjectnessPlusPlus()
        {
            Datagroup datagroup = new Datagroup();
            EntityAttribute attribute = new EntityAttribute("foo", "bar");
            EntityAttribute attribute2 = new EntityAttribute("baz", "qux");
            EntityAttribute attribute3 = new EntityAttribute("Duck", "Dodger");
            datagroup.Attributes.Add(attribute);
            datagroup.Attributes.Add(attribute2);
            datagroup.Attributes.Add(attribute3);
            dynamic renderer = new DynamicRenderer(datagroup);
            Assert.That(renderer.foo, Is.EqualTo(attribute.Value));
            Assert.That(renderer.baz, Is.EqualTo(attribute2.Value));
            Assert.That(renderer.Duck, Is.EqualTo(attribute3.Value));
        }

        [Test]
        public void testDynamicObjectnessWhenDoesNotExist()
        {
            Datagroup datagroup = new Datagroup();
            dynamic renderer = new DynamicRenderer(datagroup);
            Assert.AreEqual(0, datagroup.Attributes.Count);
            Assert.AreEqual(string.Empty, renderer.DoesNotExist);
        }

        [Test]
        public void testDynamicObjectness_Set()
        {
            Datagroup datagroup = new Datagroup();
            string expectedAttributeName = "foo";
            string originalValue = "bar";
            datagroup.Attributes.Add(new EntityAttribute(expectedAttributeName, originalValue));
            dynamic renderer = new DynamicRenderer(datagroup);
            Assert.AreEqual(originalValue, renderer.foo);
            string updatedValue = "duc";
            renderer.foo = updatedValue;
            Assert.AreEqual(updatedValue, datagroup.Attributes.Find(a => a.Name.Equals(expectedAttributeName)).Value);
            Assert.AreEqual(updatedValue, renderer.foo);
        }
    }

    public class DatagroupAttributeDictionary : IDictionary<string, string>
    {
        private readonly Datagroup datagroup;

        public DatagroupAttributeDictionary(Datagroup datagroup)
        {
            this.datagroup = datagroup;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public void Add(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out string value)
        {
            throw new System.NotImplementedException();
        }

        public string this[string key]
        {
            get { return checkForAttribute(key).ToString(); }
            set { throw new System.NotImplementedException(); }
        }

        public ICollection<string> Keys
        {
            get { throw new System.NotImplementedException(); }
        }

        public ICollection<string> Values
        {
            get { throw new System.NotImplementedException(); }
        }


        private object checkForAttribute(string methodNameThatWasCalled)
        {
            EntityAttribute entityAttribute = datagroup.Attributes.Find(a => a.Name.Equals(methodNameThatWasCalled));
            if (entityAttribute != null)
            {
                return entityAttribute.Value;
            }
            return String.Empty;
        }
    }
}
