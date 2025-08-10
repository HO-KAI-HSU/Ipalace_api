namespace com.leslie.Core.ORM.MySql
{
    using Dapper;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public static class ORM
    {
        private static Dictionary<string, string> nameMapping = new Dictionary<string, string>();
        private static Dictionary<Type, string> allSelectSQLs = new Dictionary<Type, string>();
        private static Dictionary<Type, string> partSelectSQLs = new Dictionary<Type, string>();

        public static void Initial()
        {
            List<Type> types = new List<Type>();
            foreach (var t in typeof(ORM).Assembly.GetTypes())
            {
                if (t.IsClass)
                {
                    TableNameAttribute name = (TableNameAttribute)t.GetCustomAttributes(false)?.FirstOrDefault(x => x is TableNameAttribute);
                    if (name != null)
                    {
                        nameMapping[t.Name] = name.Name;
                        types.Add(t);
                        Debug.WriteLine("Register DTO: " + t.Name);
                        continue;
                    }
                    else
                    {
                        TableAttribute tname = (TableAttribute)t.GetCustomAttributes(false)?.FirstOrDefault(x => x is TableAttribute);
                        if (tname != null)
                        {
                            nameMapping[t.Name] = t.Name.Replace("DTO", "");

                            types.Add(t);
                            Debug.WriteLine("Register DTO: " + t.Name);
                            continue;
                        }
                    }
                }
            }

            foreach (var t in types)
            {
                var sql = " `" + string.Join("`,`", GetColumnList(t).Select(x => x.Name)) + "` FROM " + GetTableName(t);
                allSelectSQLs[t] = sql;
                sql = " `" + string.Join("`,`", GetColumnList(t, false).Select(x => x.Name)) + "` FROM " + GetTableName(t);
                partSelectSQLs[t] = sql;

                foreach (var p in GetColumnList(t))
                {
                    DBJsonAttribute json = (DBJsonAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBJsonAttribute);
                    if (json != null)
                    {
                        SqlMapper.AddTypeHandler(p.PropertyType, new JsonTypeHandler());
                        Debug.WriteLine("Register JSON: " + t.Name + "." + p.Name);
                    }
                }
            }
        }

        public static string GetTableName(Type t)
        {
            string sname = t.Name;
            if (nameMapping.TryGetValue(sname, out sname))
            {
                return $"`{sname}`";
            }

            return $"`{nameMapping[t.Name]}`";
        }

        public static IEnumerable<T> GetList<T>(this DbConnection conn, DbTransaction transaction = null)
        {
            return conn.Query<T>("SELECT " + GetSelectSQL<T>(false), transaction: transaction);
        }

        public static IEnumerable<T> GetList<T>(this DbConnection conn, string predicates, DbTransaction transaction = null)
        {
            return conn.Query<T>("SELECT " + GetSelectSQL<T>(false) + " WHERE " + predicates, transaction: transaction);
        }

        public static IEnumerable<T> GetList<T>(this DbConnection conn, string predicates, object prarm, DbTransaction transaction = null)
        {
            return conn.Query<T>("SELECT " + GetSelectSQL<T>(false) + " WHERE " + predicates, prarm, transaction: transaction);
        }

        public static IEnumerable<T> GetList<T>(this DbConnection conn, Func<T, T> predicates, DbTransaction transaction = null)
            where T : new()
        {
            T t = predicates.Invoke(new T());
            T dt = new T();
            Type type = typeof(T);
            StringBuilder sb = new StringBuilder(1024);
            foreach (var p in type.GetProperties())
            {
                object value = p.GetValue(t);
                if (p.DeclaringType.IsValueType)
                {
                    if (value != p.GetValue(dt))
                    {
                        sb.Append($"{p.Name} = {value} AND ");
                    }
                }
                else if (value != null)
                {
                    sb.Append($"{p.Name} = {value} AND ");
                }
            }

            if (sb.Length > 0)
            {
                string sql = sb.ToString().Substring(0, sb.Length - 5);
                return GetList<T>(conn, sql, transaction: transaction);
            }
            else
            {
                return GetList<T>(conn, transaction: transaction);
            }
        }

        public static T Get<T>(this DbConnection conn, string predicates, DbTransaction transaction = null)
        {
            return conn.Query<T>("SELECT " + GetSelectSQL<T>() + " WHERE " + predicates + " LIMIT 1", transaction: transaction).FirstOrDefault();
        }

        public static async Task<T> GetAsync<T>(this DbConnection conn, string predicates, DbTransaction transaction = null)
        {
            return (await conn.QueryAsync<T>("SELECT " + GetSelectSQL<T>() + " WHERE " + predicates + " LIMIT 1", transaction: transaction)).FirstOrDefault();
        }

        public static T Get<T>(this DbConnection conn, string predicates, object par, DbTransaction transaction = null)
        {
            return conn.Query<T>("SELECT " + GetSelectSQL<T>() + " WHERE " + predicates + " LIMIT 1", par, transaction: transaction).FirstOrDefault();
        }

        public static async Task<T> GetAsync<T>(this DbConnection conn, string predicates, object par, DbTransaction transaction = null)
        {
            return (await conn.QueryAsync<T>("SELECT " + GetSelectSQL<T>() + " WHERE " + predicates + " LIMIT 1", par, transaction: transaction)).FirstOrDefault();
        }

        public static T Get<T>(this DbConnection conn, object key, DbTransaction transaction = null)
        {
            Type type = typeof(T);
            string sql = "";
            foreach (var p in type.GetProperties())
            {
                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null)
                {
                    sql = $"{p.Name} = {key}";
                    break;
                }
            }

            return Get<T>(conn, sql, transaction: transaction);
        }

        public static async Task<T> GetAsync<T>(this DbConnection conn, object key, DbTransaction transaction = null)
        {
            Type type = typeof(T);
            string sql = "";
            foreach (var p in type.GetProperties())
            {
                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null)
                {
                    sql = $"{p.Name} = {key}";
                    break;
                }
            }

            return await GetAsync<T>(conn, (object)sql, transaction: transaction);
        }

        public static int Delete<T>(this DbConnection conn, string predicates, DbTransaction transaction = null)
        {
            return conn.Execute("DELETE FROM " + GetTableName(typeof(T)) + " WHERE " + predicates, transaction: transaction);
        }

        public static int Delete<T>(this DbConnection conn, object key, DbTransaction transaction = null)
        {
            Type type = typeof(T);
            string sql = "";
            foreach (var p in type.GetProperties())
            {
                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null)
                {
                    sql = $"`{p.Name}` = {key}";
                    break;
                }
            }

            return Delete<T>(conn, sql, transaction);
        }

        public static int Delete<T>(this DbConnection conn, T obj, DbTransaction transaction = null)
        {
            Type type = typeof(T);
            string sql = "";
            foreach (var p in type.GetProperties())
            {
                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null)
                {
                    sql = $"{p.Name} = {p.GetValue(obj)}";
                    break;
                }
            }

            return Delete<T>(conn, sql, transaction);
        }

        public static int Update<T>(this DbConnection conn, T obj, DbTransaction transaction = null)
        {
            Type type = typeof(T);
            List<string> cols = new List<string>();
            string strWhere = string.Empty;
            foreach (var p in type.GetProperties())
            {
                DBIgnoreAttribute dBIgnore = (DBIgnoreAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBIgnoreAttribute);
                if (dBIgnore != null)
                {
                    continue;
                }

                DBUpdateIgnoreAttribute dBuIgnore = (DBUpdateIgnoreAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBUpdateIgnoreAttribute);
                if (dBuIgnore != null)
                {
                    continue;
                }

                DBNowAttribute dBNow = (DBNowAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBNowAttribute);
                if (dBNow != null && !dBNow.Update)
                {
                    continue;
                }

                object val = p.GetValue(obj);
                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null)
                {
                    strWhere = $"`{p.Name}` = @{p.Name}";
                }
                else
                {
                    if (val != null)
                    {
                        cols.Add($"`{p.Name}` = @{p.Name}");
                    }
                }
            }

            return conn.Execute("UPDATE " + GetTableName(typeof(T)) + " SET " + string.Join(",", cols) + " WHERE " + strWhere, obj, transaction: transaction);
        }

        public static int Update<T>(this DbConnection conn, T obj, DbTransaction transaction = null, params Expression<Func<T, object>>[] proprties)
        {
            Type type = typeof(T);
            List<string> cols = new List<string>();
            string strWhere = string.Empty;
            HashSet<string> keys = new HashSet<string>();
            if (proprties != null)
            {
                foreach (var p in proprties)
                {
                    var member = GetMemberExpression(p);
                    if (member != null)
                    {
                        keys.Add(member.Member.Name);
                    }
                }
            }

            foreach (var p in type.GetProperties())
            {
                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null)
                {
                    strWhere = $"`{p.Name}` = @{p.Name}";
                }
                else
                {
                    if (!keys.Contains(p.Name))
                    {
                        continue;
                    }

                    DBNowAttribute dBNow = (DBNowAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBNowAttribute);
                    if (dBNow != null && !dBNow.Update)
                    {
                        continue;
                    }

                    object val = p.GetValue(obj);

                    if (val != null)
                    {
                        cols.Add($"`{p.Name}` = @{p.Name}");
                    }
                }
            }

            return conn.Execute("UPDATE " + GetTableName(typeof(T)) + " SET " + string.Join(",", cols) + " WHERE " + strWhere, obj, transaction: transaction);
        }

        public static int Update<T>(this DbConnection conn, string set, string where, DbTransaction transaction = null)
        {
            return conn.Execute("UPDATE " + GetTableName(typeof(T)) + " SET " + set + " WHERE " + where, transaction: transaction);
        }

        public static int Insert<T>(this DbConnection conn, T obj, DbTransaction transaction = null, bool overridePrimaryKey = false)
        {
            Type type = typeof(T);
            List<string> cols = new List<string>();
            foreach (var p in type.GetProperties())
            {
                DBIgnoreAttribute dBIgnore = (DBIgnoreAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBIgnoreAttribute);
                if (dBIgnore != null)
                {
                    continue;
                }

                DBNowAttribute dBNow = (DBNowAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBNowAttribute);
                if (dBNow != null)
                {
                    if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                    {
                        p.SetValue(obj, DateTime.Now);
                    }
                    else if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(obj, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null && pk.AutoIncrement && !overridePrimaryKey)
                {
                    continue;
                }
                else
                {
                    cols.Add(p.Name);
                }
            }

            return conn.ExecuteScalar<int>("INSERT INTO " + GetTableName(typeof(T)) + " (`" + string.Join("`,`", cols) + "`) VALUES ( @" + string.Join(",@", cols) + ");SELECT LAST_INSERT_ID();", obj, transaction: transaction);
        }

        public static int Replace<T>(this DbConnection conn, T obj, DbTransaction transaction = null, bool overridePrimaryKey = false)
        {
            Type type = typeof(T);
            List<string> cols = new List<string>();
            foreach (var p in type.GetProperties())
            {
                DBIgnoreAttribute dBIgnore = (DBIgnoreAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBIgnoreAttribute);
                if (dBIgnore != null)
                {
                    continue;
                }

                DBNowAttribute dBNow = (DBNowAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBNowAttribute);
                if (dBNow != null)
                {
                    if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                    {
                        p.SetValue(obj, DateTime.Now);
                    }
                    else if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(obj, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                PrimaryKeyAttribute pk = (PrimaryKeyAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is PrimaryKeyAttribute);
                if (pk != null && pk.AutoIncrement && !overridePrimaryKey)
                {
                    continue;
                }
                else
                {
                    cols.Add(p.Name);
                }
            }

            return conn.ExecuteScalar<int>("REPLACE INTO " + GetTableName(typeof(T)) + " (`" + string.Join("`,`", cols) + "`) VALUES ( @" + string.Join(",@", cols) + ");SELECT LAST_INSERT_ID();", obj, transaction: transaction);
        }

        #region Private method

        private static IEnumerable<PropertyInfo> GetColumnList<T>(bool allColumn = true)
        {
            return GetColumnList(typeof(T), allColumn);
        }

        private static IEnumerable<PropertyInfo> GetColumnList(Type type, bool allColumn = true)
        {
            foreach (var p in type.GetProperties())
            {
                DBIgnoreAttribute ignore = (DBIgnoreAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBIgnoreAttribute);
                if (ignore != null)
                {
                    continue;
                }

                if (!allColumn)
                {
                    DBIgnoreWhenListAttribute iwl = (DBIgnoreWhenListAttribute)p.GetCustomAttributes(false)?.FirstOrDefault(x => x is DBIgnoreWhenListAttribute);
                    if (iwl != null)
                    {
                        continue;
                    }
                }

                yield return p;
            }
        }

        private static string GetSelectSQL<T>(bool allColumn = true)
        {
            var type = typeof(T);
            if (allColumn)
            {
                return allSelectSQLs[type];
            }
            else
            {
                return partSelectSQLs[type];
            }
        }

        private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> exp)
        {
            var member = exp.Body as MemberExpression;
            var unary = exp.Body as UnaryExpression;
            return member ?? (unary != null ? unary.Operand as MemberExpression : null);
        }

        private class JsonTypeHandler : SqlMapper.ITypeHandler
        {
            public object Parse(Type destinationType, object value)
            {
                return JsonConvert.DeserializeObject(value.ToString(), destinationType);
            }

            public void SetValue(IDbDataParameter parameter, object value)
            {
                parameter.Value = (value == null) ? (object)DBNull.Value : JsonConvert.SerializeObject(value);
                parameter.DbType = DbType.String;
            }
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class PrimaryKeyAttribute : System.Attribute
    {
        public string Name { get; set; } = null;

        public bool AutoIncrement { get; set; } = false;

        public PrimaryKeyAttribute(string name = null, bool autoIncrement = false)
        {
            this.Name = name;
            this.AutoIncrement = autoIncrement;
        }

        public PrimaryKeyAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DBIgnoreAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DBUpdateIgnoreAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DBIgnoreWhenListAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class TableNameAttribute : System.Attribute
    {
        public string Name { get; set; }

        public TableNameAttribute(string name)
        {
            this.Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class TableAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DBJsonAttribute : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DBNowAttribute : System.Attribute
    {
        public bool Update { get; set; }

        public DBNowAttribute(bool update = false)
        {
            this.Update = update;
        }
    }
}
