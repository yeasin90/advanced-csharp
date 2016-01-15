using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class Delegates
    {
        public delegate void Delegate1(int hours, WorkType workType);
        public delegate int Delegate2(int hours, WorkType workType);
        public delegate int Delegate3(int x, int y);

        public void Invoke1()
        {
            Delegate1 del1 = new Delegate1(Helper.WorkPerformed1);
            Delegate1 del2 = new Delegate1(Helper.WorkPerformed2);
            Delegate1 del3 = new Delegate1(Helper.WorkPerformed3);

            del1 += del2 + del3;

            del1(10, WorkType.GenerateReports);
        }

        public void Invoke2()
        {
            Delegate2 del1 = new Delegate2(Helper.WorkPerformed4);
            Delegate2 del2 = new Delegate2(Helper.WorkPerformed5);
            Delegate2 del3 = new Delegate2(Helper.WorkPerformed6);

            del1 += del2 + del3;

            int result = del1(10, WorkType.Golf);
            Console.WriteLine(result);
        }

        public void Invoke3()
        {
            Delegate3 del1 = new Delegate3(Helper.WorkPerformed7);
            this.pvtMethod1(del1);
        }

        public void Invoke4()
        {
            Delegate2 del2 = new Delegate2(Helper.WorkPerformed6);
            Delegate3 del3 = new Delegate3(Helper.WorkPerformed7);

            del2(35, WorkType.Golf);
            int result = del3(45, 2);
            Console.WriteLine(result);
        }

        private void pvtMethod1(Delegate3 del)
        {
            del(30, 36);
        }
    }
}
