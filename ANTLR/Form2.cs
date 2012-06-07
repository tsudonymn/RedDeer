using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Antlr4.StringTemplate;

namespace ANTLR
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();

            StringBuilder stringBuilder = new StringBuilder();

            //Names
            string nameFromDB = "<names:{ n | Name:<n.firstName> <n.lastName>\n}>";
            Template aNameTemplate = new Template(nameFromDB);
            Datagroup nameDatagroup = new Datagroup()
                                          {
                                              Attributes = new List<EntityAttribute>()
                                                               {
                                                                   new EntityAttribute("firstName", "Bob"),
                                                                   new EntityAttribute("lastName", "Jones")
                                                               }
                                              
                                          };
            dynamic name = new DynamicEntityAttribute();
            nameDatagroup.Attributes.ForEach(a => name.dictionary[a.Name] = a.Value);
            aNameTemplate.Add("names", new List<object> { name.dictionary});
            stringBuilder.AppendLine(aNameTemplate.Render());




            //Contacts
            var contactsFromDB = "<contacts:{ c | Contact:<c.num> (<c.type>)\n}>";
            Template contactsTemplate = new Template(contactsFromDB);
            Datagroup phoneDatagroup1 = new Datagroup()
                                            {
                                                Attributes = new List<EntityAttribute>()
                                                                 {
                                                                     new EntityAttribute("type", "cell"),
                                                                     new EntityAttribute("num", "123"),
                                                                 }
                                            };
            Datagroup phoneDatagroup2 = new Datagroup()
                                            {
                                                Attributes = new List<EntityAttribute>()
                                                                 {
                                                                     new EntityAttribute("type", "home"),
                                                                     new EntityAttribute("num", "456"),
                                                                 }
                                            };
            
            dynamic phone1 = new DynamicEntityAttribute();
            dynamic phone2 = new DynamicEntityAttribute();
            phoneDatagroup1.Attributes.ForEach(a => phone1.dictionary[a.Name] = a.Value);
            phoneDatagroup2.Attributes.ForEach(a => phone2.dictionary[a.Name] = a.Value);
            contactsTemplate.Add("contacts", new List<object> {phone1.dictionary, phone2.dictionary});
            stringBuilder.AppendLine(contactsTemplate.Render());



            //Addresses
            var addressesFromDB = "<addresses:{ a | Address:<a.line1>\n<a.city> <a.state>, <a.zip>\n}>";
            Template addressesTemplate = new Template(addressesFromDB);
            Datagroup addressDatagroup = new Datagroup()
            {
                Attributes = new List<EntityAttribute>()
                                                                 {
                                                                     new EntityAttribute("line1", "330 Packard street"),
                                                                     new EntityAttribute("city", "Ann Arbor"),
                                                                     new EntityAttribute("state", "MI"),
                                                                     new EntityAttribute("zip", "48656"),
                                                                 }
            };

            dynamic address = new DynamicEntityAttribute();
            addressDatagroup.Attributes.ForEach(a => address.dictionary[a.Name] = a.Value);
            addressesTemplate.Add("addresses", new List<object> { address.dictionary});
            stringBuilder.AppendLine(addressesTemplate.Render());

            textBox1.Text = stringBuilder.ToString();
        }
    }
}
