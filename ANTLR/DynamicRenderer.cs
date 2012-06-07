using System;
using System.Dynamic;
using System.Windows.Forms;

namespace ANTLR
{
    public class DynamicRenderer : DynamicObject
    {
        private readonly Datagroup datagroup;

        public DynamicRenderer(Datagroup datagroup)
        {
            this.datagroup = datagroup;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            MessageBox.Show("Here");
            return base.TryGetIndex(binder, indexes, out result);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            MessageBox.Show("Here");
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            MessageBox.Show("Here");
            return base.TryConvert(binder, out result);
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            MessageBox.Show("Here");
            return base.TryCreateInstance(binder, args, out result);
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            MessageBox.Show("Here");
            return base.TryDeleteIndex(binder, indexes);
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            MessageBox.Show("Here");
            return base.TryDeleteMember(binder);
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            MessageBox.Show("Here");
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            MessageBox.Show("Here");
            return base.TrySetIndex(binder, indexes, value);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            MessageBox.Show("Here");
            return base.TryUnaryOperation(binder, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string methodNameThatWasCalled = binder.Name;
            result = checkForAttribute(methodNameThatWasCalled);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            string methodNameThatWasCalled = binder.Name;
            result = checkForAttribute(methodNameThatWasCalled);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string methodNameThatWasCalled = binder.Name;
            EntityAttribute entityAttribute = datagroup.Attributes.Find(a => a.Name.Equals(methodNameThatWasCalled));
            if (entityAttribute != null)
            {
                entityAttribute.Value = value.ToString();
            }
            return true;
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