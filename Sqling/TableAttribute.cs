using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqling {


    /// <summary>
    /// Attribute for describing a class as a model of a SQL table 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute {

        public string SqlName = null;

        public TableAttribute(string name) {
            SqlName = name;
        }

        public TableAttribute() {

        }

    }
}
