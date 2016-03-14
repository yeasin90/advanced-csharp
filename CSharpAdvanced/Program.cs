using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Delegates delegates = new Delegates();
            delegates.Invoke1();
            delegates.Invoke2();
            delegates.Invoke3();
            delegates.Invoke4();

            EventInvoker eventInvoker = new EventInvoker();
            eventInvoker.Invoke1();
            eventInvoker.Invoke2();
            eventInvoker.Invoke3();
            eventInvoker.Invoke4();

            FuncLambda funcLambda = new FuncLambda();
            funcLambda.Invoke1();
            funcLambda.Invoke2();
            funcLambda.Invoke3();
            funcLambda.Invoke4();
        }
    }
}
