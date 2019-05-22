using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sqling {

    /// <summary>
    /// Attribute for describing a field as a SQL field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldAttribute : Attribute {

        public string SqlName = null;

        public FieldAttribute(string name) {
            SqlName = name;
        }

        public FieldAttribute() {

        }
    }
}
