Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata

Namespace WindowsApplication4
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub


		Private Function CreateObject(ByVal s As IDataStore, ByVal name As String, ByVal baseType As Type) As XPClassInfo
			Dim tables() As DBTable = (CType(s, IDataStoreSchemaExplorer)).GetStorageTables(name)
			Dim table As DBTable = tables(0)
			Dim info As XPClassInfo = XpoDefault.Dictionary.CreateClass(XpoDefault.Dictionary.QueryClassInfo(baseType), table.Name)
			Dim key As String = table.PrimaryKey.Columns(0)
			For Each c As DBColumn In table.Columns
				Dim mi As XPMemberInfo = info.CreateMember(c.Name, DBColumn.GetType(c.ColumnType))
				If key = c.Name Then
					mi.AddAttribute(New KeyAttribute(c.IsIdentity))
				End If
			Next c
			Return info
		End Function


		Private connection As String = AccessConnectionProvider.GetConnectionString("nwind.mdb")

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			Dim s As IDataStore = XpoDefault.GetConnectionProvider(connection, AutoCreateOption.None)
			Dim d As XPDictionary = New ReflectionDictionary()
			XpoDefault.Dictionary = d
			XpoDefault.DataLayer = New SimpleDataLayer(d, s)

			Dim order As XPClassInfo = CreateObject(s, "orders", GetType(DetailDataObject))
			Dim customer As XPClassInfo = CreateObject(s, "customers", GetType(ParentDataObject))
			Dim a As New AssociationAttribute("CustomerOrders") ', typeof(DetailDataObject)
			a.ElementTypeName = "orders"

			Dim a1 As New AssociationAttribute("CustomerOrders") ', typeof(ParentDataObject)

			order.CreateMember("CustomerID", customer, New Attribute() { a1 })
			customer.CreateMember("Orders", GetType(XPCollection), True, New Attribute() { a })
			gridControl1.DataSource = New XPCollection(Session.DefaultSession, customer)

		End Sub
	End Class
	<NonPersistent, MemberDesignTimeVisibility(False)> _
	Public Class DetailDataObject
		Inherits XPLiteObject
		Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
			MyBase.New(session, classInfo)
		End Sub
	End Class

	<NonPersistent, MemberDesignTimeVisibility(False)> _
	Public Class ParentDataObject
		Inherits XPLiteObject
		Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
			MyBase.New(session, classInfo)
		End Sub
	End Class

End Namespace