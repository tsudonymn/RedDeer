using System.Collections.Generic;

namespace ANTLR
{
    public class Datagroup
    {
        public List<EntityAttribute> Attributes { get; set; }

        public Datagroup()
        {
            Attributes = new List<EntityAttribute>();
        }
    }
}