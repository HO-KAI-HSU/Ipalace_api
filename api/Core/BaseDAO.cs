namespace Web.DAO
{
    using com.leslie.Core.ORM.MySql;
    using Dapper;
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    public class BaseDAO : IDisposable
    {
        protected Database database = null;

        protected DbConnection conn      
        {
            get
            {
                if (database == null)
                {
                    database = new Database();
                }

                return database.Connection;
            }

            set
            {
                if (database == null)
                {
                    database = new Database(false);
                }

                database.Connection = value as MySqlConnection;
            }
        }

        public void Combine(BaseDAO dao)
        {
            this.conn = dao.conn;
        }

        public BaseDAO()
        {
        }

        public void Dispose()
        {
            database?.Dispose();
        }

        protected IEnumerable<T> QuerySP<T>(string storedProcedure, object parameters, DbTransaction transaction = null)
        {
            return conn.Query<T>(storedProcedure, param: parameters, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
        }

        protected int ExecuteSP(string storedProcedure, object parameters, DbTransaction transaction = null)
        {
            return conn.ExecuteScalar<int>(storedProcedure, param: parameters, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
        }

        public DbTransaction BeginTransaction()
        {
            return conn.BeginTransaction();
        }

        protected static T GetDAO<T>()
           where T : BaseDAO, new()
        {
            return new T();
        }

        protected static T GetDAO<T>(BaseDAO dao)
            where T : BaseDAO, new()
        {
            var t = new T();
            t.Combine(dao);
            return t;
        }

        public DbConnection GetConnection()
        {
            return conn;
        }
    }
}
