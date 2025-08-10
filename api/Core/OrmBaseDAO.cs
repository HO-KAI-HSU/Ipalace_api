namespace Web.DAO
{
    using com.leslie.Core.ORM.MySql;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;

    public class OrmBaseDAO<T> : BaseDAO
    {
        protected string TableName { get => ORM.GetTableName(typeof(T)); }

        public virtual IEnumerable<T> GetAll()
        {
            return conn.GetList<T>();
        }

        public virtual IEnumerable<T> GetAll(string where)
        {
            return conn.GetList<T>(where);
        }

        public virtual IEnumerable<T> GetAll(string where, object parm, DbTransaction transaction = null)
        {
            return conn.GetList<T>(where, parm, transaction);
        }

        public virtual T Get(object key, DbTransaction transaction = null)
        {
            return conn.Get<T>(key, transaction);
        }

        public virtual T Get(string where, object parm, DbTransaction transaction = null)
        {
            return conn.Get<T>(where, parm, transaction);
        }

        public virtual int Insert(T dto, DbTransaction transaction = null)
        {
            return conn.Insert(dto, transaction);
        }

        public virtual int Replace(T dto, DbTransaction transaction = null)
        {
            return conn.Replace(dto, transaction);
        }

        public virtual int Update(T dto, DbTransaction transaction = null)
        {
            return conn.Update(dto, transaction);
        }

        public virtual int Update(T dto, DbTransaction transaction = null, params Expression<Func<T, object>>[] proprties)
        {
            return conn.Update(dto, transaction, proprties);
        }

        public virtual int Delete(string where, DbTransaction transaction = null)
        {
            return conn.Delete<T>(where, transaction);
        }

        public virtual int Delete(object key, DbTransaction transaction = null)
        {
            return conn.Delete<T>(key, transaction);
        }

        public Paged<T> GetPage(string order, int page, int pageSize = 20, string cols = "*", string where = null, object par = null)
        {
            if (string.IsNullOrEmpty(where))
            {
                where = " 1 = 1";
            }

            return new Paged<T>()
            {
                Count = conn.ExecuteScalar<int>($"SELECT Count(*) FROM {TableName} WHERE {where}", par),
                Data = conn.Query<T>($"SELECT {cols} FROM {TableName} WHERE {where} ORDER BY {order} LIMIT {(page - 1) * pageSize}, {pageSize}", par)
            };
        }

        public virtual bool Any(string where, DbTransaction transaction = null)
        {
            return conn.Query($"SELECT 1 FROM {TableName} WHERE " + where, transaction: transaction).Any();
        }

        public virtual bool Any(string where, object par, DbTransaction transaction = null)
        {
            return conn.Query($"SELECT 1 FROM {TableName} WHERE " + where, par, transaction: transaction).Any();
        }
    }

    public class Paged<T>
    {
        public int Count { get; set; }

        public IEnumerable<T> Data { get; set; }
    }

    public class OrmDAO
    {
        public static OrmBaseDAO<TYPE> GetDAO<TYPE>()
        {
            return new OrmBaseDAO<TYPE>();
        }
    }
}
