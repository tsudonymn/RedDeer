using ANTLR;
using NUnit.Framework;
using Antlr4.StringTemplate;


namespace ANTLRTests
{
    [TestFixture]
    public class DynamicRendererTest
    {

        [Test]
        public void ObjectsWithStringTemplate()
        {
            dynamic tmp = new {foo = "VS Sux!"};
            Template template = new Template("<x.foo>");

            template.Add("x", tmp);
            Assert.That(template.Render(), Is.EqualTo(tmp.foo) );
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
        public void DynamicObjectWithStringTemplate()
        {
            Datagroup datagroup = new Datagroup();
            EntityAttribute entityAttribute = new EntityAttribute { Name = "foo", Value = "I hate VS" };
            datagroup.Attributes.Add(entityAttribute);
            dynamic dynamicRenderer = new DynamicRenderer(datagroup);

            Template template = new Template("<x.foo>");
            template.Add("x", dynamicRenderer);

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
}
