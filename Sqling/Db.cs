using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace Sqling {


    public delegate void ReaderFunc(SqlDataReader reader);


    /// <summary>
    /// A database handler
    /// </summary>
    public class Db {


        /// <summary>
        /// The SqlConnectionStringBuilder that is used to create SqlConnections
        /// </summary>
        public readonly SqlConnectionStringBuilder ConnectionString;

        /// <summary>
        /// Creates an instance from a connection string
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        public Db(string connectionString) {
            ConnectionString = new SqlConnectionStringBuilder(connectionString);
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Db() {
            ConnectionString = new SqlConnectionStringBuilder();
        }

        /// <summary>
        /// Creates a new connection
        /// </summary>
        /// <returns></returns>
        protected SqlConnection NewConnection() => new SqlConnection(ConnectionString.ConnectionString);


        /// <summary>
        /// Executes commandText as SQL and invokes the ReaderFunc delegate for each record 
        /// </summary>
        /// <param name="commandText">The SQL to be executed</param>
        /// <param name="func">The delegate that runs foreach record</param>
        /// <param name="variables">The SQL variables</param>
        /// <returns>true if SqlDataReader has any rows else false</returns>
        public bool ExecuteReader(string commandText, ReaderFunc func, dynamic variables) {
            bool hasRows = false;
            using (var con = NewConnection()) {
                con.Open();
                using (var cmd = con.CreateCommand()) {

                    // Setup command:
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = commandText;

                    if(variables != null) {
                        System.Reflection.PropertyInfo[] varprops = variables.GetType().GetProperties();
                        foreach (var prop in varprops) {
                            cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(variables));
                        }
                    }
                    
                    // Start reading:
                    using (var reader = cmd.ExecuteReader()) {

                        while(reader.Read()) {
                            func(reader);
                        }

                        hasRows = reader.HasRows;
                        reader.Close();
                    }

                }
                con.Close();
            }
            return hasRows;
        }


        public IEnumerable<T> Select<T>() where T : new() {
            var res = new List<T>();
            ExecuteReader(SQLTable<T>.CreateSelectCommand(), r => res.Add(SQLTable<T>.NewFromReader(r)), null);
            return res;
        }

    }
}
