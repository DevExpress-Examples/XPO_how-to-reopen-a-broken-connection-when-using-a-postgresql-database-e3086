using DevExpress.Xpo;

namespace B190497
{
    public class Person : XPObject
    {
        public Person(Session session) : base(session) { }

        string fName;
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }
    }
}