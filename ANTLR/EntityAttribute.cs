namespace ANTLR
{
    public class EntityAttribute
    {
        public string Value { get; set; }
        public string Name { get; set; }

        public EntityAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public EntityAttribute()
        {
        }
    }
}