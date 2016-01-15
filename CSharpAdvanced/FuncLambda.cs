using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public delegate int BizRulesDelegate(int x, int y);

    public class FuncLambda
    {
        public void Invoke1()
        {
            ProcessData data = new ProcessData();
            BizRulesDelegate addDel = (x, y) => x + y;
            BizRulesDelegate multipleDel = (x, y) => x * y;
            data.Process(2, 3, addDel);
        }

        public void Invoke2()
        {
            ProcessData data = new ProcessData();
            Action<int, int> myAction = (x, y) => Console.WriteLine(x + y);
            Action<int, int> myMultiplyAction = (x, y) => Console.WriteLine(x * y);
            data.ProcessAction(2, 3, myAction);
        }

        public void Invoke3()
        {
            ProcessData data = new ProcessData();
            Func<int, int, int> funcAddDel = (x, y) => x + y;
            Func<int, int, int> funcMultiplyDel = (x, y) => x * y;
            data.ProcessFunc(1, 2, funcMultiplyDel);
            data.ProcessFunc(1, 2, (x, y) =>
            {
                x += 15;
                y += 2;
                return x / y;
            });
        }

        public void Invoke4()
        {
            Func<int, int> test = (x) => x + 10;
            Func<int, int, int> mult = (x, y) => x * y;
            Action<int> print = x => Console.WriteLine(x);

            print(mult(10, 20));
        }
    }

    public class ProcessData
    {
        public void Process(int x, int y, BizRulesDelegate del)
        {
            var result = del(x, y);
            Console.WriteLine(result);
        }

        public void ProcessAction(int x, int y, Action<int, int> action)
        {
            action(x, y);
            Console.WriteLine("Action performed");
        }

        public void ProcessFunc(int x, int y, Func<int, int, int> action)
        {
            var result = action(x, y);
            Console.WriteLine(result);
        }
    }
}
