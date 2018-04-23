using System;
using System.Data;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using System.Reflection;
using System.Globalization;

namespace B190497
{
    public class SafePostgreSqlConnectionProvider : IDataStore, IDisposable
    {
        PostgreSqlConnectionProvider InnerDataStore;
        IDbConnection Connection;
        string ConnectionString;
        AutoCreateOption AutoCreateOption;

        public SafePostgreSqlConnectionProvider(string connectionString, AutoCreateOption autoCreateOption)
        {
            this.ConnectionString = connectionString;
            this.AutoCreateOption = autoCreateOption;
            DoReconnect();
        }

        ~SafePostgreSqlConnectionProvider()
        {
            Dispose(false);
        }

        void DoReconnect()
        {
            DoDispose(false);
            Connection = PostgreSqlConnectionProvider.CreateConnection(ConnectionString);
            InnerDataStore = new PostgreSqlConnectionProvider(Connection, AutoCreateOption);
        }

        void DoDispose(bool closeConnection)
        {
            if (Connection != null)
            {
                if (closeConnection)
                {
                    Connection.Close();
                    Connection.Dispose();
                }
                Connection = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                DoDispose(true);
        }

        void HandleNullReferenceException(Exception ex)
        {
            if (ex == null) return;
            if (ex is NullReferenceException && InnerDataStore.Connection.State == ConnectionState.Open)
                DoReconnect();
            else throw ex;
        }

        AutoCreateOption IDataStore.AutoCreateOption
        {
            get { return InnerDataStore.AutoCreateOption; }
        }

        ModificationResult IDataStore.ModifyData(params ModificationStatement[] dmlStatements)
        {
            try
            {
                return InnerDataStore.ModifyData(dmlStatements);
            }
            catch (SqlExecutionErrorException ex)
            {
                HandleNullReferenceException(ex.InnerException);
            }
            return InnerDataStore.ModifyData(dmlStatements);
        }

        SelectedData IDataStore.SelectData(params SelectStatement[] selects)
        {
            try
            {
                return InnerDataStore.SelectData(selects);
            }
            catch (NullReferenceException ex)
            {
                HandleNullReferenceException(ex.InnerException);
            }
            return InnerDataStore.SelectData(selects);
        }

        UpdateSchemaResult IDataStore.UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
        {
            try
            {
                return InnerDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
            }
            catch (SqlExecutionErrorException ex)
            {
                HandleNullReferenceException(ex.InnerException);
            }
            return InnerDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}