using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class AsyncAwaitTheory
    {
        public static string result = string.Empty;
        public static WebClient client = new WebClient();
        public static HttpClient clinet2 = new HttpClient();
        public static HttpWebRequest req = null;
        public static HttpWebResponse resp = null;

        static void AllCases()
        {
            // Synchronous 1
            result = client.DownloadString("http://www.pluralsight.com/");

            // Synchronous 2
            req = (HttpWebRequest)WebRequest.Create("http://www.pluralsight.com");
            req.Method = "HEAD"; // Telling to return only Header information of the server in response
            resp = (HttpWebResponse)req.GetResponse(); // This line will block until response arrives from server
            result = FormatHeader(resp.Headers);

            // Asynchornous 1
            client.DownloadStringAsync(new Uri("http://www.pluralsight.com"));

            // Asynchrounous 2 - OLD Technology
            var currentSyncThread = SynchronizationContext.Current; // returns currect thread
            req = (HttpWebRequest)WebRequest.Create("http://www.pluralsight.com");
            req.Method = "HEAD";
            req.BeginGetResponse(
                asyncResult => // this is an anonymous method
                {
                    try
                    {
                        resp = (HttpWebResponse)req.EndGetResponse(asyncResult);
                        // As we are using ashynchronous, resp may be returend by another thread
                        // But our controls are in main thread which is different than this respons thread.
                        // So, in order to return the response to main thread, we need to use the following code.
                        // var sync = SynchronizationContext.Current will hold the reference of main or starting thread
                        // this is very similar to JavaScript Ajax done callback event
                        currentSyncThread.Post(
                            delegate
                            {
                                result = FormatHeader(resp.Headers);
                            },
                            null
                            );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                },
                null
                );

            // Asynchronous 3 - NEW TECHNOLOGY
            // Similar asynchonous of Asynchronous 2
            // But here, we don't have to keep track of the main thread reference
            // This is where Task based will come into play.
            // This will internally convert response of different thread into main thread
            RunAsynchronouslyTaskBased();

            // Asynchronous 4 - NEW TECHNOLOGY
            Debug.WriteLine("Calling async function");
            RunAsynchronously1();
            Debug.WriteLine("Calling finish");

            // Asynchronously 5 - NEW TECHNOLOGY LOADING PANEL
            RunAsynchronously2();

            // TPL with Asynchornous
            // TPL - gives ability to write concurrent + async code
            // TPL indtroduced in .NET 4.0
            // in RunAsynchronously2(), we have used event based action when result arrives
            // This concept is called Continuation
            // Here, we will see Continuation with TPL
            // Continuation = We can schedule something to be executed, once our async operation is completed
            var task = Task.Run(
                () =>
                {
                    // do something in async

                    Thread.Sleep(1000);
                    return "Async Result";
                }
                );
            task.ContinueWith((completedTaskRef) =>
            {
                //completedTaskRef.IsFaulted; // set to true if the async operation throws any error

                Console.WriteLine(completedTaskRef.Result); // will print : "Async Result"

                // do something after async operation has been finished
                // this will execute asynchornously
                // which means, will execute in a seperate thread, rather than main thread
                // example : remove loading panel
            });

            // this allow us to define where to try and execute the Continuation
            // task.ContinueWith(completedTaskRef) executes in a seperate thread.
            // this is why we had to pass the main thread referece to the Continuation block through : completedTaskRef
            // But, the below code will execute the continuation into the main thread
            // Thats why, we do not have to pass the main thread reference
            // rather, we used the direc task inside the continuation
            task.ConfigureAwait(true) // this : true  means that the contunuation block will be back to main thread
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    string x = task.Result;
                });



            // Asynchornous 6
            // this asynchornous is done with async await keyword
            // previously, we saw asynchonours operation with TPL
            // it was introduced in .NET 4.0
            // but a problem there was, we had to pass the referecen of main thread to the continuation
            // asynchronous operation with async await is introduced in .NET 4.5
            // here, we don't have to pass the referecen of main thread for result. It's done internally
            // also, we don't have to write Continuation block manually with async and await
            // everyhting after await keyword is continuation
            // With TPL, we had to check IsFault and all other messy stuffs
            // But in async await, we can handle exception just like normal try catch
            try
            {
                // Start loading panel
                var result = LoginAsync(); // this should be await LoginAsync()
                // when result arrives, disable loading panel
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed");
            }


            // State Machine, Deadlock and what happens in async await
            // deadlock : 

            // here, we are creating an asynchronous operation.
            // This will delay for 1sec asynchronously.
            // when 1 sec elapsed, something will happen with ContinueWith
            var t2 = Task.Delay(1).ContinueWith((t) // this t means, we are passing the original caller thread to the continuation
                =>
            {

            });

            task.Wait(); // lock up the application or caller thread, until the above code finished it's execution
            // the above line task.Wait() creats deadlock. But without task.Wait() there will be no deadlock. HOW?
            // These all depends on what we write inside our ContinueWith
            // if we access caller thread inside COntinueWith(), that will create deadlock
            // Because, task.Wait() is blocking the caller thread. 
            // At the same time, we are trying to access the caller thread inside ContinueWith
            // This is how deadlock happend


            var r1 = GetDataAsync().Result;
            // The above line will also block the caller thread. or create Deadlock
            // We did not declared the statement await LoginAsync()
            // rather, we declared without await. This is where problem will arise
            // by not making an asyn fucntion with await, the async function will execute in the main thread
            // at the same time, we used the statment : LoginAsync().Result. This means that, block the main thread until async result arrives.
            // as a result, deadlock
            // to prevent this deadlock, run the async operation in a seperate thread using Task.Run(() => () => GetDataAsync())
            var r2 = Task.Run(() => GetDataAsync()).Result;
            // this time, GetDataAsync() async function will not execute in the main thread. Will execute in a different thread
            // and Result keyword of statement : Task.Run(() => GetDataAsync()).Result, will block the main thread.
            // overall, block the main thread with Result, until async method GetDataAsync() returns a result which is running on a different thread
            // So, we can see that marking a methond async Task<string> does not mean that this will always execute asynchronously

            // So, .Result does actually forcefully blocks our application.
            // But, why do we need to block our application
            // It depends on our need.
            // If we need something, where our some operation will execute asynchronously BUT our main thread should run synchornously, then we need to block our main thread


            // So, Task.Run(() => GetDataAsync()).Result does solves deadlock problem.
            // But when it comes to issue for passing thread reference to Continuation block, there asyn with TPL may not be user friendly
            // So, it's always best to use async and await for asynchornous operation
            // ** PROPER USE OF ASYNCHRONOUS OPERATION
            try
            {
                // Proper use of asynchronous operation
                // var result = await GetDataAsync();
                // continuation statement 1
                // continuation statement 2
            }
            catch (Exception ex)
            {

            }

            // Task<T> = representation of an operation that needs to be completed at some point of the future
            // With task, that operation can be executed : synchronously, asynchronously or in parallel.
            // Task can be combined together or with a Callback
            var r3 = Task.Factory.StartNew<int>(SlowOperation);

            // These code will be executed regardless of r3.Result has arrived or not
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine("Slow operation result : {0}", r3.Result); // But program execution will be blocked on this line unless Task's result is ready
            Console.WriteLine("Slow operation complete on thread : {0}", Thread.CurrentThread.ManagedThreadId); // after r3.Result has arrived, code after r3.Result will get executed. That means this line will be executed after r3.Result has arrived

            // Starts of the asyn operation
            var r4 = SlowOperationAsync();

            // await of async operation will return here and execute the below loop
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                Console.WriteLine("Slow operation result : {0}", r3.Result); // But program execution will be blocked on this line unless Task's result is ready
                Console.WriteLine("Slow operation complete on thread : {0}", Thread.CurrentThread.ManagedThreadId); // after r3.Result has arrived, code after r3.Result will get executed. That means this line will be executed after r3.Result has arrived
            }

            // Async with multple await
            var t1 = GetDataAsync();
            var t4 = SlowOperationAsync();

            // start both asyn task paralelly. WhenAll ensures that, continutaion gets exeucted after all asyn is done
            // the below code should be await Task.WhenAll(t1, t4);
            // This is called TPL with await
            // now, t1 or t4 may throw exception. In that case we can use normal try catch block.
            // than proper exception will be caught either from t1 or t4
            Task.WhenAll(t1, t4);
            Console.WriteLine("Asyn result 1 : " + t1.Result + " Asyn result 2 : " + t4.Result);

            // Async TimeOut
        }

        private static int SlowOperation()
        {
            Console.WriteLine("Slow operation started on thread : {0}", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000); // this line will block th execution. So, we defined Task in called so that it gets executed asynchornously
            Console.WriteLine("Slow operation complete on thread : {0}", Thread.CurrentThread.ManagedThreadId);

            return 42;
        }

        public static async Task<int> SlowOperationAsync()
        {
            Console.WriteLine("Slow operation started on thread : {0}", Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(2000);  // this line will NOT block execution. It will execute asynchronously in a seperate thread
            Console.WriteLine("Slow operation complete on thread : {0}", Thread.CurrentThread.ManagedThreadId);

            return 42;
        }

        private static async Task<string> GetDataAsync()
        {
            await Task.Delay(1000);

            return "Success";
        }

        // if we use async void, we will not be able to cathc expcetion in the calling thread
        // for void return type, use Task
        // for other return type, use Task<T>
        // this is the correct syntax of writting an async method
        private static async Task<string> LoginAsync()
        {
            try
            {
                // runs code asynchornously inside Run() =>
                var loginTask = Task.Run(() =>
                {
                    //throw new UnauthorizedAccessException();
                    Thread.Sleep(2000);
                    return "Async Result";
                });

                var logTask = Task.Delay(2000); // Log the login
                var purchaseTask = Task.Delay(2000); // Fetch purchase

                // WhenAll returns a Task when all operations under WhenAll has been finished
                await Task.WhenAll(loginTask, logTask, purchaseTask);

                return loginTask.Result;
            }
            catch (Exception ex)
            {
                return "Failed";
            }
        }

        private static string FormatHeader(WebHeaderCollection headers)
        {
            var headerStrings = from header in headers.Keys.Cast<string>()
                                select string.Format("{0}: {1}", header, headers[header]);

            return string.Join(Environment.NewLine, headerStrings.ToArray());
        }

        private static async void RunAsynchronously1()
        {
            string data = string.Empty;
            WebClient w = new WebClient();
            // Say, thread1 makes the call to RunAsynchronously()
            // thread1 finds the keyword await
            // when thread1 finds a keyword await, thread1 will return to it's original execution block or caller
            // Then what will happen to function call after await, which is w.DownloadStringTaskAsync("http://www.pluralsight.com");?
            // This will run in a different thread, say thread2.
            // code block after await will not get executed before await method has finished it's works
            var result = await w.DownloadStringTaskAsync("http://www.pluralsight.com");
            data = result; // this line will not execute until, await w.DownloadStringTaskAsync("http://www.pluralsight.com"); has finished downloading
        }

        public static void RunAsynchronously2()
        {
            string data = string.Empty;
            WebClient w = new WebClient();
            // Show loading panel
            w.DownloadStringAsync(new Uri("http://www.pluralsight.com"));
            w.DownloadStringCompleted += W_DownloadStringCompleted;
        }

        private static void W_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // Hide Loading panel
            // Show result with e.Result'
        }

        public static async void RunAsynchronouslyTaskBased()
        {
            req = (HttpWebRequest)WebRequest.Create("http://www.pluralsight.com");
            req.Method = "HEAD";
            Task<WebResponse> getResponseTask = Task.Factory.FromAsync<WebResponse>(
                req.BeginGetResponse, req.EndGetResponse, null
                );
            resp = (HttpWebResponse)await getResponseTask;
            result = FormatHeader(resp.Headers);
        }
    }
}
