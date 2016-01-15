using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class AsyncAwait
    {
        public static int SyncOperation()
        {
            Thread.Sleep(2000);
            return 50;
        }

        public static async Task<int> Async1()
        {
            await Task.Delay(2000); // This line will have impact. Because, await keyword will make a thread jump out of this block of code
            //throw new ArgumentException(); // This exception can be caught with normal try catch in the caller 
            Console.WriteLine("Result ready");
            return 100;
        }

        public static async Task Async2()
        {
            await Task.Delay(5000);
            Console.WriteLine("Delay finish"); // This will not be displayed in Console. Becasue, we are not returning into the main thread after await, rather we are doing a CW
        }

        // We don't need to throw any error into the main thread. Thats why we did not marked as Task, rather made void.
        // In this case, If any error occurs, we just have to log it. NO NEED TO SHOW IT TO UI
        public static async void Async3()
        {
            await Task.Run(() =>
            {
                try
                {
                    throw new ArgumentException();
                    int x = 0;
                    for (int i = 0; i < 100000; i++)
                    {
                        x = x + 1;
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }

        public static void Async4()
        {
            Console.WriteLine("Starting thread : {0}", ThreadHelper.CurrentThread);
            var task = Task.Run(() =>
            {
                Thread.Sleep(3000);
                return ThreadHelper.CurrentThread.ToString();
            })
            .ContinueWith((completedTaskRef) =>
            {
                if (completedTaskRef.IsFaulted)
                    Console.WriteLine("Had errpr");

                Console.WriteLine("Continuation thread : {0}", ThreadHelper.CurrentThread);
                return completedTaskRef.Result;
            });

            Console.WriteLine("Do this!! thread ID : {0}", ThreadHelper.CurrentThread);
            Console.WriteLine("Do that!! thread ID : {0}", ThreadHelper.CurrentThread);

            Console.WriteLine("Result in thread ID : {0}", task.Result);
            Console.WriteLine("Good bye!! Final thread : {0}", ThreadHelper.CurrentThread);
        }

        public static void Async5()
        {
            Console.WriteLine("Starting thread : {0}", ThreadHelper.CurrentThread);
            var task = Task.Run(() =>
            {
                Thread.Sleep(3000);
                return ThreadHelper.CurrentThread.ToString();
            });

            task.ContinueWith((completedTaskRef) =>
            {
                Console.WriteLine("Continuation thread : {0}", ThreadHelper.CurrentThread);
            });


            task.Wait();
            Console.WriteLine("Do this!! thread ID : {0}", ThreadHelper.CurrentThread);
            Console.WriteLine("Do that!! thread ID : {0}", ThreadHelper.CurrentThread);

            Console.WriteLine("Result in thread ID : {0}", task.Result);
            Console.WriteLine("Good bye!! Final thread : {0}", ThreadHelper.CurrentThread);
        }

        public async static Task<string> Async6()
        {
            var task1 = //Task.Delay(7000).ContinueWith((main) => { return "Hello"; }); // This will also have impact

                Task.Factory.StartNew(() =>
                {
                    throw new ArgumentNullException(); // Need to study how to handle exception with WhenAll
                    Task.Delay(20000); // This line will not have any impact. Because, this will execute on a differetn thread. Not on the thread from which it was created which is : Task.Run(() => or the creator
                    Thread.Sleep(7000); // But this will have impact. Because, this code is not a Task and will run on the same thread
                    return "Hello World!!";
                });

            var task2 = Async1();
            var task3 = Task.Delay(4000);

            // starts executing tasks parallely
            await Task.WhenAll(task1, task2, task3);
            Console.WriteLine("Result 1 : {0}..Result 2 : {1}..Result 3 : {2}", task1.Result, task2.Result, task3.Status);
            return "Finish Execution";

            // need to study how to handle Task.WhenAll exception
        }

        public static void Async7()
        {
            var t1 = Task.Run(() =>
            {
                Thread.Sleep(4000);
                return 10;
            });

            var t2 = Task.Run(() =>
            {
                Thread.Sleep(1000);
                return 30;
            });

            var t3 = Task.Run(() =>
            {
                Thread.Sleep(6000);
                return 60;
            });

            Console.WriteLine("Hellow");
            Console.WriteLine("Result 1 : {0}", t1.Result);
            Console.WriteLine("Result 2 : {0}", t2.Result);
            Console.WriteLine("Result 3 : {0}", t3.Result);
        }
    }

    public static class ThreadHelper
    {
        public static int CurrentThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}
