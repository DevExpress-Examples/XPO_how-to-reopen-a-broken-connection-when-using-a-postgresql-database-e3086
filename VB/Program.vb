Imports System
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB

Namespace B190497
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			SafePostgreSqlConnectionProvider.Register()
			Dim dataStore As New SafePostgreSqlConnectionProvider("user id=postgres;password=admin;server=donchakDBFarm;database=XpoUnitTests;port=5434", AutoCreateOption.DatabaseAndSchema)
			Dim dal As IDataLayer = New SimpleDataLayer(dataStore)
			Dim id As Integer = CreateData(dal)
			Console.WriteLine("restart the database, and press any key to continue ..")
			Console.ReadKey()
			CType(New Session(dal), Session).GetObjectByKey(Of Person)(id)
			DirectCast(dataStore, IDisposable).Dispose()
			dal.Dispose()
			Console.WriteLine("done" & ControlChars.Lf & "press any key to exit ..")
			Console.ReadKey()
		End Sub
		Private Shared Function CreateData(ByVal dal As IDataLayer) As Integer
			Using uow As New UnitOfWork()
				Dim result As Person = uow.FindObject(Of Person)(Nothing)
				If result Is Nothing Then
					result = New Person(uow)
					result.Name = "Uriah"
				End If
				uow.CommitChanges()
				Return result.Oid
			End Using
		End Function
	End Class
	Public Class Person
		Inherits XPObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Private fName As String
		Public Property Name() As String
			Get
				Return fName
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)("Name", fName, value)
			End Set
		End Property
	End Class
End Namespace
