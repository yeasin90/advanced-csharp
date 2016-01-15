using CSharpAdvanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class EventInvoker
    {
        public void Invoke1()
        {
            Events events = new Events();
            events.WorkPerformed += new EventHandler<WorkPerformedEventArgs>(Helper.worker_WorkedPerformed);
            events.WorkCompleted += new EventHandler(Helper.worker_WorkedCompleted);
            events.DoWork(10, WorkType.GenerateReports);
        }

        public void Invoke2()
        {
            Events events = new Events();
            events.WorkPerformed += Helper.worker_WorkedPerformed;
            events.WorkCompleted += Helper.worker_WorkedCompleted;
            events.DoWork(45, WorkType.GoToMeetings);
        }

        public void Invoke3()
        {
            Events events = new Events();
            events.WorkPerformed += delegate (object sender, WorkPerformedEventArgs e)
            {
                Console.WriteLine(e.Hours + " " + e.WorkType);
            };
            events.WorkCompleted += delegate (object sender, EventArgs e)
            {
                Console.WriteLine("Worker is done");
            };
            events.DoWork(51, WorkType.GoToMeetings);
        }

        public void Invoke4()
        {
            Events events = new Events();
            events.WorkPerformed += (s, e) =>
            {
                Console.WriteLine(e.Hours + " " + e.WorkType);
            };
            events.WorkCompleted += (s, e) =>
            {
                Console.WriteLine("Worker is done");
            };
            events.DoWork(8, WorkType.GenerateReports);
        }
    }

    public class Events
    {
        public event EventHandler<WorkPerformedEventArgs> WorkPerformed;
        public event EventHandler WorkCompleted;

        public void DoWork(int hours, WorkType workType)
        {
            for (int i = 0; i < hours; i++)
            {
                System.Threading.Thread.Sleep(1000);
                OnWorkedPerformed(i + 1, workType);
            }
            OnWorkCompleted();
        }

        protected virtual void OnWorkedPerformed(int hours, WorkType workType)
        {
            var del = WorkPerformed as EventHandler<WorkPerformedEventArgs>;
            if (del != null)
            {
                del(this, new WorkPerformedEventArgs(hours, workType));
            }
        }

        protected virtual void OnWorkCompleted()
        {
            var del = WorkCompleted as EventHandler;
            if (del != null)
            {
                del(this, EventArgs.Empty);
            }
        }
    }

    public class WorkPerformedEventArgs : EventArgs
    {
        public WorkPerformedEventArgs(int hours, WorkType workType)
        {
            Hours = hours;
            WorkType = workType;
        }

        public int Hours { get; set; }
        public WorkType WorkType { get; set; }
    }
}
