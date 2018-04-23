Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.DB.Exceptions

Namespace B190497
	Public Class SafePostgreSqlConnectionProvider
		Implements IDataStore, IDisposable
		Private InnerDataStore As PostgreSqlConnectionProvider
		Private Connection As IDbConnection
		Private ConnectionString As String
		Private AutoCreateOption As AutoCreateOption

		Public Sub New(ByVal connectionString As String, ByVal autoCreateOption As AutoCreateOption)
			Me.ConnectionString = connectionString
			Me.AutoCreateOption = autoCreateOption
			DoReconnect()
		End Sub

		Protected Overrides Sub Finalize()
			Dispose(False)
		End Sub

		Private Sub DoReconnect()
			DoDispose(False)
			Connection = PostgreSqlConnectionProvider.CreateConnection(ConnectionString)
			InnerDataStore = New PostgreSqlConnectionProvider(Connection, AutoCreateOption)
		End Sub

		Private Sub DoDispose(ByVal closeConnection As Boolean)
			If Connection IsNot Nothing Then
				If closeConnection Then
					Connection.Close()
					Connection.Dispose()
				End If
				Connection = Nothing
			End If
		End Sub

		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				DoDispose(True)
			End If
		End Sub

		Private Sub HandleNullReferenceException(ByVal ex As Exception)
			If ex Is Nothing Then
				Return
			End If
			If TypeOf ex Is NullReferenceException AndAlso InnerDataStore.Connection.State = ConnectionState.Open Then
				DoReconnect()
			Else
				Throw ex
			End If
		End Sub

		Private ReadOnly Property IDataStore_AutoCreateOption() As AutoCreateOption Implements IDataStore.AutoCreateOption
			Get
				Return InnerDataStore.AutoCreateOption
			End Get
		End Property

		Private Function ModifyData(ParamArray ByVal dmlStatements() As ModificationStatement) As ModificationResult Implements IDataStore.ModifyData
			Try
				Return InnerDataStore.ModifyData(dmlStatements)
			Catch ex As SqlExecutionErrorException
				HandleNullReferenceException(ex.InnerException)
			End Try
			Return InnerDataStore.ModifyData(dmlStatements)
		End Function

		Private Function SelectData(ParamArray ByVal selects() As SelectStatement) As SelectedData Implements IDataStore.SelectData
			Try
				Return InnerDataStore.SelectData(selects)
			Catch ex As NullReferenceException
				HandleNullReferenceException(ex.InnerException)
			End Try
			Return InnerDataStore.SelectData(selects)
		End Function

		Private Function IDataStore_UpdateSchema(ByVal dontCreateIfFirstTableNotExist As Boolean, ParamArray ByVal tables() As DBTable) As UpdateSchemaResult Implements IDataStore.UpdateSchema
			Try
				Return InnerDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables)
			Catch ex As SqlExecutionErrorException
				HandleNullReferenceException(ex.InnerException)
			End Try
			Return InnerDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables)
		End Function

		Private Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub
	End Class
End Namespace