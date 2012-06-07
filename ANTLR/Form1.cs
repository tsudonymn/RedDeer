using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Misc;
using Antlr4.StringTemplate.Visualizer;
using Antlr4.StringTemplate.Visualizer.Extensions;

namespace ANTLR
{
    public partial class Form1 : Form
    {
        private string body = string.Empty;
        public Form1()
        {
            InitializeComponent();

            StringBuilder stringBuilder = new StringBuilder();
            Template namesTemplate = new Template("<phones>");
            string fromTheDB = "First Name: <firstName>, <if(LastName)>Last Name:<LastName><endif>";
            Template aNameTemplate = new Template(fromTheDB);

            Datagroup nameDatagroup = new Datagroup()
            {
                Attributes = new List<EntityAttribute>() {
                                          new EntityAttribute("firstName", "Bob"),
//                                          new EntityAttribute("LastName", "Jones")
                                          }
            };

            foreach (var entityAttribute in nameDatagroup.Attributes)
            {
                aNameTemplate.Add(entityAttribute.Name, entityAttribute.Value);
            }
//            stringBuilder.AppendLine(aNameTemplate.Render());

            Datagroup phoneDatagroup1 = new Datagroup()
            {
                Attributes = new List<EntityAttribute>() {
                                          new EntityAttribute("type", "cell"),
                                          new EntityAttribute("num", "123"),
                                          }
            };

            Datagroup phoneDatagroup2 = new Datagroup()
            {
                Attributes = new List<EntityAttribute>() {
                                          new EntityAttribute("type", "home"),
                                          new EntityAttribute("num", "456"),
                                          }
            };


            List<Datagroup> phones = new List<Datagroup>() {phoneDatagroup1, phoneDatagroup2};

//            foreach (var datagroup in phones)
//            {
//                Template aPhoneTemplate = new Template("Phone Number: <num> (<type>)");
//                datagroup.Attributes.ForEach(attribute => aPhoneTemplate.Add(attribute.Name, attribute.Value));
//                stringBuilder.AppendLine(aPhoneTemplate.Render());
//            }

            Template phonesTemplate = new Template("<phones:{ p | Phone:<p.num> (<p.type>)}>");

//            var phone = new { num = "789", type = "cell" };
            //dynamic phone1 = new DynamicRenderer(phoneDatagroup1);
            //dynamic phone2 = new DynamicRenderer(phoneDatagroup2);
//            stringBuilder.AppendLine(phone1.num);
//            stringBuilder.AppendLine(phone2.type);
//            var value = new {phone1, phone2};
            //phonesTemplate.Add("phones", new List<object>{phone1, phone2});
            //stringBuilder.AppendLine(phonesTemplate.Render());



            Template contactsTemplate = new Template("<contacts:{ c | contact:<c.num> (<c.type>)\n}>");

            Datagroup phonGroup=new Datagroup();
            
            dynamic attribute = new DynamicEntityAttribute();
            dynamic attribute2 = new DynamicEntityAttribute();
//            attribute.dictionary["number" ] = "878768687";
//            attribute.dictionary["type"] = "Cell";
            phoneDatagroup1.Attributes.ForEach(a => attribute.dictionary[a.Name] = a.Value);
            phoneDatagroup2.Attributes.ForEach(a => attribute2.dictionary[a.Name] = a.Value);


//            Dictionary<string,object> foo = new Dictionary<string, object>();
//            phoneDatagroup1.Attributes.ForEach(a => foo[a.Name] = a.Value);
            //            contactsTemplate.Add("contacts", attribute);
//            var value1 = new {attribute[], attribute.type};
//            var value2 = new {attribute2.num, attribute2.type};
            contactsTemplate.Add("contacts", new List<object>{ attribute.dictionary , attribute2.dictionary});
//            foreach (var VARIABLE in foo)
//            {
//                contactsTemplate.Add("contacts", VARIABLE.Value);
//            }


//            dynamic attribute2 = new DynamicEntityAttribute();
//            attribute2.dictionary["phone.home.number" + "phone.home.type"] = "321331313" + "Cell";
//
//            foreach (var VARIABLE in attribute2.dictionary)
//            {
//                contactsTemplate.Add("contacts", VARIABLE);
//            }

            stringBuilder.AppendLine(contactsTemplate.Render());
            
            textBox1.Text = stringBuilder.ToString();

        }

        public class Phone
        {
            public string num { get; set; }

            public string type { get; set; }
        }


        public void PrintChildren(CommonTree ast)
        {
            PrintChildren(ast, " ", true);
        }

        public void PrintChildren(CommonTree ast, string delim, bool final)
        {
            if (ast.Children == null)
            {
                return;
            }

            int num = ast.Children.Count;

            for (int i = 0; i < num; ++i)
            {
                CommonTree d = (CommonTree)(ast.Children[i]);
                Print(d);
                if (final || i < num - 1)
                {
                    body += delim;
                }
            }
        }

        public void Print(CommonTree ast)
        {
            switch (ast.Token.Text)
            {
                case "EXPR":
                    body += "Header";
                    PrintChildren(ast);
                    //body += footer;
                    break;
                case ":":
                    body += "\r\n\r\n// Attributes\r\n";
                    PrintChildren(ast);
                    break;
                case "PROP":
                    body += "Property ";
                    PrintChildren(ast);
                    body += ";\r\n";
                    break;
                case "SUBTEMPLATE":
                    body += "SUBS ";
                     PrintChildren(ast);
                    body += ";\r\n";
                    break;
            }
        }

    }
}
