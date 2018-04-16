using System;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace B190497 {
    class Program {
        static void Main(string[] args) {
            SafePostgreSqlConnectionProvider.Register();
            SafePostgreSqlConnectionProvider dataStore = new SafePostgreSqlConnectionProvider(
                "user id=postgres;password=admin;server=donchakDBFarm;database=XpoUnitTests;port=5434",
                AutoCreateOption.DatabaseAndSchema
            );
            IDataLayer dal = new SimpleDataLayer(dataStore);
            int id = CreateData(dal);
            Console.WriteLine("restart the database, and press any key to continue ..");
            Console.ReadKey();
            new Session(dal).GetObjectByKey<Person>(id);
            ((IDisposable)dataStore).Dispose();
            dal.Dispose();
            Console.WriteLine("done\npress any key to exit ..");
            Console.ReadKey();
        }
        static int CreateData(IDataLayer dal) {
            using (UnitOfWork uow = new UnitOfWork()) {
                Person result = uow.FindObject<Person>(null);
                if (result == null) {
                    result = new Person(uow);
                    result.Name = "Uriah";
                }
                uow.CommitChanges();
                return result.Oid;
            }
        }
    }
    public class Person : XPObject {
        public Person(Session session) : base(session) { }
        string fName;
        public string Name {
            get { return fName; }
            set { SetPropertyValue<string>("Name", ref fName, value); }
        }
    }
}
