using System;
using System.Dynamic;

namespace ANTLR
{
    public class DynamicRenderer : DynamicObject
    {
        private readonly Datagroup datagroup;

        public DynamicRenderer(Datagroup datagroup)
        {
            this.datagroup = datagroup;
        }
        
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string methodNameThatWasCalled = binder.Name;
            EntityAttribute entityAttribute = datagroup.Attributes.Find(a => a.Name.Equals(methodNameThatWasCalled));
            if (entityAttribute != null)
            {
                result = entityAttribute.Value;
                return true;
            }
            result = String.Empty;
            return true;
        }
//        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
//        {
//            string name = binder.Name;
//            EntityAttribute entityAttribute = datagroup.Attributes.Find(a => a.Name.Equals(name));
//            if (entityAttribute != null)
//            {
//                result = entityAttribute.Value;
//                return true;
//            }
//            result = String.Empty;
//            return false;
//        }
//
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            string name = binder.Name;
            EntityAttribute entityAttribute = datagroup.Attributes.Find(a => a.Name.Equals(name));
            if (entityAttribute != null)
            {
                entityAttribute.Value = value.ToString();
            }
            return true;
        }
    }
}