using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace WindowsApplication4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private XPClassInfo CreateObject(IDataStore s, string name, Type baseType) {
            DBTable[] tables = ((IDataStoreSchemaExplorer)s).GetStorageTables(name);
            DBTable table = tables[0];
            XPClassInfo info = XpoDefault.Dictionary.CreateClass(XpoDefault.Dictionary.QueryClassInfo(baseType), table.Name);
            string key = table.PrimaryKey.Columns[0];
            foreach (DBColumn c in table.Columns)
            {
                XPMemberInfo mi = info.CreateMember(c.Name, DBColumn.GetType(c.ColumnType));
                if (key == c.Name)
                    mi.AddAttribute(new KeyAttribute(c.IsIdentity));
            }
            return info;
        }


        string connection = SQLiteConnectionProvider.GetConnectionString(@"nwind.sqlite");

        private void Form1_Load(object sender, EventArgs e)
        {
            IDataStore s = XpoDefault.GetConnectionProvider(connection, AutoCreateOption.None);
            XPDictionary d = new ReflectionDictionary();
            XpoDefault.Dictionary = d;
            XpoDefault.DataLayer = new SimpleDataLayer(d, s);

            XPClassInfo order = CreateObject(s, "Order", typeof(DetailDataObject));
            XPClassInfo customer = CreateObject(s, "Customer", typeof(ParentDataObject));
            AssociationAttribute a = new AssociationAttribute("CustomerOrders"); //, typeof(DetailDataObject)
            a.ElementTypeName = "Order";

            AssociationAttribute a1 = new AssociationAttribute("CustomerOrders"); //, typeof(ParentDataObject)

            order.CreateMember("CustomerId", customer, new Attribute[] { a1 });
            customer.CreateMember("Orders", typeof(XPCollection), true, new Attribute[] { a });
            gridControl1.DataSource = new XPCollection(Session.DefaultSession, customer);

        }
    }
    [NonPersistent, MemberDesignTimeVisibility(false)]
    public class DetailDataObject : XPLiteObject
    {
        public DetailDataObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
    }

    [NonPersistent, MemberDesignTimeVisibility(false)]
    public class ParentDataObject : XPLiteObject
    {
        public ParentDataObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
    }
    
}