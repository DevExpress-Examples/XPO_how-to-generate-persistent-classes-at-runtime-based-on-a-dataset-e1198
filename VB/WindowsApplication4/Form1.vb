Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata

Namespace WindowsApplication4

    Public Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Function CreateObject(ByVal s As IDataStore, ByVal name As String, ByVal baseType As Type) As XPClassInfo
            Dim tables As DBTable() = CType(s, IDataStoreSchemaExplorer).GetStorageTables(name)
            Dim table As DBTable = tables(0)
            Dim info As XPClassInfo = XpoDefault.Dictionary.CreateClass(XpoDefault.Dictionary.QueryClassInfo(baseType), table.Name)
            Dim key As String = table.PrimaryKey.Columns(0)
            For Each c As DBColumn In table.Columns
                Dim mi As XPMemberInfo = info.CreateMember(c.Name, DBColumn.[GetType](c.ColumnType))
                If Equals(key, c.Name) Then mi.AddAttribute(New KeyAttribute(c.IsIdentity))
            Next

            Return info
        End Function

        Private connection As String = SQLiteConnectionProvider.GetConnectionString("nwind.sqlite")

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            Dim s As IDataStore = XpoDefault.GetConnectionProvider(connection, AutoCreateOption.None)
            Dim d As XPDictionary = New ReflectionDictionary()
            XpoDefault.Dictionary = d
            XpoDefault.DataLayer = New SimpleDataLayer(d, s)
            Dim order As XPClassInfo = CreateObject(s, "Order", GetType(DetailDataObject))
            Dim customer As XPClassInfo = CreateObject(s, "Customer", GetType(ParentDataObject))
            Dim a As AssociationAttribute = New AssociationAttribute("CustomerOrders") ', typeof(DetailDataObject)
            a.ElementTypeName = "Order"
            Dim a1 As AssociationAttribute = New AssociationAttribute("CustomerOrders") ', typeof(ParentDataObject)
            order.CreateMember("CustomerId", customer, New Attribute() {a1})
            customer.CreateMember("Orders", GetType(XPCollection), True, New Attribute() {a})
            gridControl1.DataSource = New XPCollection(Session.DefaultSession, customer)
        End Sub
    End Class

    <NonPersistent, MemberDesignTimeVisibility(False)>
    Public Class DetailDataObject
        Inherits XPLiteObject

        Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
            MyBase.New(session, classInfo)
        End Sub
    End Class

    <NonPersistent, MemberDesignTimeVisibility(False)>
    Public Class ParentDataObject
        Inherits XPLiteObject

        Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
            MyBase.New(session, classInfo)
        End Sub
    End Class
End Namespace
