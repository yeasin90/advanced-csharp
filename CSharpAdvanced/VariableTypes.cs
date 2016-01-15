using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class VariableTypes
    {
        public const string const_var = "Constant Variable"; // By defult static. Value cannot be changed anywhere. Ctor, Function, runtime etc no-where
        public readonly string read_only = "Read Only variable"; // Value can be changed through Ctor at runtime. But not through Function
        public static readonly string stat_read_only = "Static Read Only"; // Value can be changed through static ctor at runtime. But not through Function
    }
}
