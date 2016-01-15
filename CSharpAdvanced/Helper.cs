using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public static class Helper
    {
        public static void WorkPerformed1(int hours, WorkType workType)
        {
            Console.WriteLine("Workedperfomed1 called " + hours.ToString());
        }

        public static void WorkPerformed2(int hours, WorkType workType)
        {
            Console.WriteLine("Workedperfomed2 called " + hours.ToString());
        }

        public static void WorkPerformed3(int hours, WorkType workType)
        {
            Console.WriteLine("Workedperfomed3 called " + hours.ToString());
        }

        public static int WorkPerformed4(int hours, WorkType workType)
        {
            return hours + 1;
        }

        public static int WorkPerformed5(int hours, WorkType workType)
        {
            return hours + 2;
        }

        public static int WorkPerformed6(int hours, WorkType workType)
        {
            return hours + 3;
        }

        public static int WorkPerformed7(int x, int y)
        {
            return x * y;
        }

        public static void worker_WorkedCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Worker is done");
        }

        public static void worker_WorkedPerformed(object sender, WorkPerformedEventArgs e)
        {
            Console.WriteLine(e.Hours + " " + e.WorkType);
        }
    }
}
