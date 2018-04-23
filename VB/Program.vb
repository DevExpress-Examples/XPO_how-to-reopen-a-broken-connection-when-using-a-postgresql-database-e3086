Imports Microsoft.VisualBasic
Imports System
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports System.Data

Namespace B190497
	Friend Class Program
		Shared Sub Main(ByVal args() As String)
			Dim dataStore As New SafePostgreSqlConnectionProvider("user id=Uriah;password=a1rPl4Ne;server=localhost;database=B190497", AutoCreateOption.DatabaseAndSchema)
			Dim dal As IDataLayer = New SimpleDataLayer(dataStore)
			Dim id As Integer = CreateData(dal)
			Console.WriteLine("restart the database, and press any key to continue ..")
			Console.ReadKey()
			CType(New Session(dal), Session).GetObjectByKey(Of Person)(id)
			CType(dataStore, IDisposable).Dispose()
			dal.Dispose()
			Console.WriteLine("done" & Constants.vbLf & "press any key to exit ..")
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
End Namespace
