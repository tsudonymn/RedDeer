using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using ANTLR;
using NUnit.Framework;
using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Misc;


namespace ANTLRTests
{
    [TestFixture]
    public class DynamicRendererTest
    {
        [Test]
        public void ObjectsWithStringTemplate()
        {
            dynamic tmp = new { foo = "I should get this" };
            Template template = new Template("<x.foo>");

            template.Add("x", tmp);
            Assert.That(template.Render(), Is.EqualTo(tmp.foo));
        }

        [Test]
        public void StandardAttributeWithLoop()
        {
            string ph1 = "123-555-1212";
            string ph2 = "214-652-8652";
            var lead1 = new { name = "Lead1Name", Phones = new List<dynamic> {new {num = ph1}, new {num = ph2}} };

            Template template = new Template("<lead.name>:\n" +
                                             "<lead.phones:{p|<p.num>\n}>");
            template.Add("lead", lead1);
            Assert.That(template.Render(), Is.EqualTo("Lead1Name:\r\n" +
                                                      string.Format("{0}\r\n", ph1) +
                                                      string.Format("{0}\r\n", ph2)));
        }

        [Test]
        public void ATemplateGroupWithMulipleProperties()
        {
            dynamic name1 = new { title = "Cptn", first = "James", middle = "T", last = "Kirk", suffix = "Sr." };

            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("nameLineFormat", "<name.title> <name.first> <name.middle> <name.last>, <name.suffix>");

            Template template = @group.GetInstanceOf("nameLineFormat");
            Assert.That(template, Is.Not.Null);
            
            template.Add("nameLineFormat", name1);

            Assert.That(template.Render(), Is.EqualTo("Cptn James T Kirk, Sr."));
        }

        [Test]
        public void ATemplateWithMultipleProperties()
        {
            dynamic name1 = new { title = "Cptn", first = "James", middle = "T", last = "Kirk", suffix = "Sr." };
            Template template = new Template("<name.title> <name.first> <name.middle> <name.last>, <name.suffix>\n");
            template.Add("name", name1);
            Assert.That(template.Render(), Is.EqualTo("Cptn James T Kirk, Sr."+ Environment.NewLine));
        }

        [Test]
        public void SimpleTemplateGroup()
        {
            dynamic name1 = new { title = "Cptn", first = "James", middle = "T", last = "Kirk", suffix = "Sr." };

            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("nameLineFormat", "<name:{n| n.title n.first n.middle n.last, n.suffix\n}>");
            group.DefineTemplate("averyLabel", "<nameLineFormat(theName=name1)>\n");
            Template template = @group.GetInstanceOf("averyLabel");
            template.Add("theName", name1);
            Assert.That(template.Render(), Is.EqualTo("Cptn James T Kirk, Sr."));
        }

        [Test]
        public void TemplateGroups()
        {
            string ph1 = "123-555-1212";
            string ph2 = "214-652-8652";

            var name1 = new { title = "Cptn", fn = "James", mi = "T", ln = "Kirk", suffix = "Sr." };

            var lead1 = new { name = name1, Phones = new List<dynamic> { new { num = ph1 }, new { num = ph2 } } };

            var address = new { line1 = "1 Infinite Loop", city = "aa", state = "mi" };

            TemplateGroup group = new TemplateGroup();
            group.DefineTemplate("leadBlockFormat", "<lead.name>:\n" +
                                             "<lead.phones:{p|<p.num>\n}>");
            group.DefineTemplate("addressBlockFormat", "<address.line1>\n" +
                                             "<address.city>, <address.state>");
            group.DefineTemplate("nameLineFormat", "<lead.name:{n| n.title n.fn n.mi n.ln, n.suffix\n}>");
            group.DefineTemplate("averyLabel", "<nameLineFormat(lead=theLead)>\n<addressBlockFormat(address=address)>\nleadBlockFormat(lead=theLead)\n");
            Template template = @group.GetInstanceOf("averyLabel");
            template.Add("theLead", lead1);
            Assert.That(template.Render(), Is.EqualTo("!"));
        }

        [Test]
        public void GroupTemplateWithNewlineAtTheEndAppearsToConsumeTheNewline()
        {
            string header = "This is the header";
            string body = "This is the body";
            TemplateGroup group = new TemplateGroup();
            string headerWithNewlineAtEnd = string.Format("{0}" + Environment.NewLine, header);
            group.DefineTemplate("header", headerWithNewlineAtEnd);
            group.DefineTemplate("body", string.Format("{0}\n", body));
            group.DefineTemplate("page", "<header()><body()>");
            Assert.That(group.GetInstanceOf("page").Render(), Is.EqualTo(string.Format("{0}\r\n{1}\r\n", header, body)));
        }

        [Test]
        public void GroupTemplateWithNewlineInTheMiddleWorks()
        {
            string header = "This is the header";
            string body = "This is the body";
            TemplateGroup group = new TemplateGroup();
            string headerWithNewlineInTheMiddle = string.Format("{0}\nNotNewline", header);
            group.DefineTemplate("header", headerWithNewlineInTheMiddle);
            group.DefineTemplate("body", string.Format("{0}", body));
            group.DefineTemplate("page", "<header()><body()>");
      
            Assert.That(group.GetInstanceOf("page").Render(), Is.EqualTo(string.Format("{0}\r\nNotNewline{1}", header, body)));
        }

        [Test]
        public void MultiplePropertiesFromACollection()
        {
            var address1 = new {line1 = "1 Infinite Loop", city = "Cupertino"};
            var address2 = new {line1 = "23 Harbor Dr", city = "East Tawas"};
            var tmp = new {addresses = new List<dynamic> {address1, address2}};

            Template template = new Template("<addresses:{address|<address.line1>, <address.city>\n}>");

            template.Add("addresses", tmp.addresses);
            Assert.That(template.Render(), Is.EqualTo("1 Infinite Loop, Cupertino\r\n23 Harbor Dr, East Tawas\r\n"));

        }

        [Test]
        public void CollectionWithASubProperty()
        {
            var address1 =
                new {Labels = new Dictionary<string, string> {{"order", "primary"}}};
            var addres2 = new {Labels = new Dictionary<string, string> {{"order", "not primary"}}};
            var tmp = new {addresses = new List<dynamic> {address1, addres2}};

            Template template = new Template("<addresses:{address|<address.Labels.order>\n}>");
            template.Add("addresses", tmp.addresses);
            Assert.That(template.Render(), Is.EqualTo("primary\r\nnot primary\r\n"));
        }

        [Test]
        public void ObjectCollectionsUsingLoops()
        {
            var aParent = new { foo = "I should get this"};
            var anotherParent = new { foo = "I should get that"};
            var tmp = new { parents = new List<dynamic> { aParent, anotherParent } };

            Template template = new Template("<parents:{parent|<parent.foo>\n}>");

            template.Add("parents", tmp.parents);
            Assert.That(template.Render(), Is.EqualTo("I should get this\r\nI should get that\r\n"));
        }

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

    public class Parent
    {
        public Dictionary<string, string> Child { get; set; }
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
