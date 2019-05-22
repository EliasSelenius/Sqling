using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Data.SqlClient;

namespace Sqling {


    /// <summary>
    /// static class for generating SQL and initializing models
    /// </summary>
    /// <typeparam name="T">The model that this class creates SQL and objects for</typeparam>
    public static class SQLTable<T> where T : new() {

        /// <summary>
        /// Dictionary of the SQL name (Key) and FieldInfo (Value) wich is the type parameter's fields and their equivalent SQL field name
        /// </summary>
        public static readonly Dictionary<string, FieldInfo> SqlFields = new Dictionary<string, FieldInfo>();
        /// <summary>
        /// The type prameter's SQL table name
        /// </summary>
        public static string TableName { get; private set; }

        static SQLTable() {
            GetSqlFields();
        }

        private static void GetSqlFields() {
            var type = typeof(T);

            // SQL table
            TableName = Attribute.IsDefined(type, typeof(TableAttribute)) ? type.GetCustomAttribute<TableAttribute>().SqlName ?? type.Name : type.Name;

            // SQL Fields
            foreach(FieldInfo f in type.GetFields()) {
                if(Attribute.IsDefined(f, typeof(FieldAttribute))) {
                    FieldAttribute fattrib = f.GetCustomAttribute<FieldAttribute>();
                    SqlFields.Add(fattrib.SqlName ?? f.Name, f);
                }
            }
            
        }


        /// <summary>
        /// Generates a query that SELECT all columns FROM the type parameter's sql table name
        /// </summary>
        /// <returns>SQL script</returns>
        public static string CreateSelectCommand() {

            // SELECT:
            var res = $"SELECT TOP 100";
            if(SqlFields.Count == 0) {
                res += " *";
            } else {
                foreach (var kv in SqlFields) {
                    res += $" {kv.Key},";
                }
                res = res.Remove(res.Length - 1);
            }

            // FROM: 
            res += $" FROM {TableName}";

            return res;
        }


        /// <summary>
        /// Creates a new instance of the type parameter from a given SqlDataReader
        /// </summary>
        /// <param name="reader">The data to be populated in the type parameter</param>
        /// <returns>A new instance of T</returns>
        public static T NewFromReader(SqlDataReader reader) {
            T t = new T();
            foreach (var kv in SqlFields) {
                kv.Value.SetValue(t, reader[kv.Key]);                
            }
            return t;
        }

        /// <summary>
        /// Selects all from a given database
        /// </summary>
        /// <param name="db">The database</param>
        /// <returns>All records of T</returns>
        public static IEnumerable<T> AllFrom(Db db) {
            var res = new List<T>();

            db.ExecuteReader(CreateSelectCommand(), x => res.Add(NewFromReader(x)), null);

            return res;
        }



    }
}
