using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class ConcurrentCSharp
    {
        public static readonly List<string> AllShirtNames =
           new List<string> { "technologyhour", "Code School", "jDays", "buddhistgeeks", "iGeek" };

        public void Run()
        {
            MultiThreaded_NotThreadSafe();
            MultiThreaded_ThreadSafe(); // use of ConcurrentCollection
            AtomicMethod();
            WhyLockIsNotGoodChoice();
            RaceConditionConcurrentCollection();
            TypesOfConcurrentCollection();

            // Use of ConcurrentDictionray()
            ConcurrentDictionaryTheory();
            DictionaryStandard();
            DictionaryConcurrent();
            ConcurrentDictionary_ProblemWithTryUpdate();
            ConcurrentDictionary_AddOrUpdate();
            ConcurrentDictionary_BewareRaceCondition();
            PrincipleInConcurrency();
            ConcurrentDictionary_GetOrAddMethod();

            // ConcurrentDictionary Demo
            //StockController controller = new StockController();
            //TimeSpan workDay = new TimeSpan(0, 0, 2); // how long each sales person will work for

            //Task t1 = Task.Run(() => new SalesPerson("Sahil").Work(controller, workDay));
            //Task t2 = Task.Run(() => new SalesPerson("Peter").Work(controller, workDay));
            //Task t3 = Task.Run(() => new SalesPerson("Juliette").Work(controller, workDay));
            //Task t4 = Task.Run(() => new SalesPerson("Xavier").Work(controller, workDay));

            //Task.WaitAll(t1, t2, t3, t4);
            //controller.DisplayStatus();
        }

        private static void ConcurrentDictionary_GetOrAddMethod()
        {
            // As we saw, methods on Concurrent Dictionary are : 
            // - TryGetValue()
            // - TryAdd()
            // - TryRemove()
            // - TryUpdate()
            // - AddOrUpdate();
            // Above methods won't throw exceptions if they fail

            // There is another method : GetOrAdd() which avoids exception by adding the Key in the dictionary if the key is not in the Dicsionary
            // GetOrAdd(TKey key, TValue value)
            // So there are three ways to looking up a value in the Dicsionary
            // 1. int psStock=  stock["x"] - indexer
            // 2.   int psStock;
            //      bool success = stock.TryGetValue("x", out stock)
            // 3. int psStock = stock.GetOrAdd("x", 32);
        }

        private static void PrincipleInConcurrency()
        {
            // Principle : Do Each operation in One method call

            // Example : 
            // There are two way we can get the value from a dictionary

            // 1 .
            // int psStock = stock.AddOrUpdate("x", 1, (Key, oldValue) => oldValue + 1);
            // Console.WriteLine("New value is " + psStock)
            // - Here, we are diplaying the new value after the update
            // Basically, we are doing single operatio here
            // a. Use AddOrUpdate() to change the value and return it to a varibale

            // 2.
            // int psStock = stock.AddOrUpdate("x", 1, (Key, oldValue) => oldValue + 1);
            // Console.WriteLine("New value is " + stock["x"])
            // - here, we are accessing what is in the dicstionary with indexer : stock["x"]
            // So, basically we are doing two operations here
            // a. One to update the dictionary with AddOrUpdate()
            // b. another is accessing the Dictionary again to get value

            // But principle is : 
            // If we want to do any operation on the dicsionary, then you should try to do it in one method call to the concurrent dictionary
            // Reason is just to avoid possible Race condition
            // So, 2. is an example of Race Condition , as we are updating value but we are accessing it in later time
            // And we also know that, Concurrent collections do not protect you from race conditions between method calls
            // So, it's our resposibility to write code in way that we can avoid Race Condition
            // To overcome it, user GetOrAdd()
        }

        private static void ConcurrentDictionary_BewareRaceCondition()
        {
            // #Approach1
            // In single threaded code, we can use below to proint dictionary
            // Console.WriteLine("New value is " + stock["pluralsight"])

            // #Approach2
            // In multi-threaded code, we use the bleow return value to print
            // int psStock = stock.AddOrUpdate("pluralsight", 1, (key, oldValue) => oldValue + 1);
            // Console.WriteLine("New value is " + psStock);

            // Diff between #AApp1 and #App2
            // - #A2 always displays the actual value that was set during AddOrUpdate()
            // - #A1 displays the value that was present at the time the indexer stock["pluralsight"] was invoked
            // - In single-threaded app #A1 and #A2 value would be same
            // - In multi-threaded app, those value could be different
            // - In #A2, another thread can change the value of Dicstionary, as soon as AddOrUpdate() Retunr
            // - #A1 even could throw an exception as another thread might delete the value

            // So in multi-threaded app, always think : 
            // - Are there any other thread that might modify the collection?
            // - If there are, then don't assume the value that has been set in the collection haven't been changed by another thread
            // - Use the retunr value of AddOrUpdate() to get the current value of dictionary, as this avoids possible race condition
        }

        private static void ConcurrentDictionary_AddOrUpdate()
        {
            var stock = new ConcurrentDictionary<string, int>();
            stock.TryAdd("jDays", 4);
            stock.TryAdd("technolgyhour", 3);
            Console.WriteLine(string.Format("No. of shirts in stock = {0}", stock.Count));

            bool success = stock.TryAdd("pluralsight", 6);
            Console.WriteLine("Added succeeded? " + success);
            success = stock.TryAdd("pluralsight", 6);
            Console.WriteLine("Added succeeded? " + success);

            stock["budidstgeeks"] = 5;

            //stock["pluralsight"]++; indexer syntax which causes Race condition
            int psStock = stock.AddOrUpdate("pluralsight", 1, (key, oldValue) => oldValue + 1);
            Console.WriteLine("New value is " + psStock);

            int jDaysValue;
            success = stock.TryRemove("jDays", out jDaysValue);
            Console.WriteLine("value removed was: " + success);

            Console.WriteLine("\r\nEnumerating:");
            foreach (var keyValPair in stock)
            {
                Console.WriteLine("{0}: {1}", keyValPair.Key, keyValPair.Value);
            }
        }

        private static void ConcurrentDictionary_ProblemWithTryUpdate()
        {
            // Say, we want to increment a value against a key of a dictionary
            // in single-threaded code : stock["pluralsight"]++;
            // in muti-thread : YOU ABSOLUTELY MUST NOT DO THIS -> stock["pluralsight"]++;
            // WHY??
            // stock["pluralsight"]++; will be compiled to something like below : 
            // int temp = stock["pluralsight"];
            // stock["pluralsight"] = temp + 1;
            // above code is not Atomic.
            // HOW?
            // Say, Thread1 retrieved and old value of stock["pluralsight"], say 6,
            // And then, immediately, another thread Thread2 modifies this value into dictionary, say from 6 to 4
            // Now, the original thread, Thread1 resumes and add 1 to it's catched value 6, and assign 7 to dictionary
            // So, here Thread1 just lost the original value, which was 6
            // This is a BUG and This is a perfect example of Race Condition
            // 
            // So, using indexing syntax will cause Race condition
            // To solve it, can we use TryUpdate() ????? 
            // unfortunately, that won't work either
            // example : 
            // int temp = stock["pluralsight"]; ----- l1
            // bool success = stock.TryUpdate("pluralsight", temp + 1, temp); --- l2
            // if(!success)
            // {
            //
            // Say, Thread1 tried to update the value
            // In the mean time, Thread2 just updated the ol value
            // In this case, TryUpdate() will return false
            // Then what can we do now???
            // we can again perform : temp = stock["pluralsight"] but that will also cause problem if another Thread chnages the old value
            // AddOrUpdate() is here to help us
        }

        private static void DictionaryConcurrent()
        {
            // In standard Dictionray, we use Add or Remove or indexing to update (myDictionary["key"]
            // In ConcurrentDictionary, we use TryAdd(), TryRemove(), TryUpdate() etc
            // In standard Dictionray Add, Remove or indexing will throw exception 
            // In ConcurrentDictionray, ryAdd(), TryRemove(), TryUpdate() will not throw exception
            // thus follows the rule of atomic method, which is : AtomicMethod(); [press F12 in this method to learn]

            var stock = new ConcurrentDictionary<string, int>();
            stock.TryAdd("jDays", 4);
            stock.TryAdd("technolgyhour", 3);
            Console.WriteLine(string.Format("No. of shirts in stock = {0}", stock.Count));

            bool success = stock.TryAdd("pluralsight", 6);
            Console.WriteLine("Added succeeded? " + success);
            success = stock.TryAdd("pluralsight", 6);
            Console.WriteLine("Added succeeded? " + success);

            stock["budidstgeeks"] = 5;

            //stock["pluralsight"] = 7;
            success = stock.TryUpdate("pluralsight", 7, 6); // update from 6 to 7
            Console.WriteLine(string.Format("pluralsight = {0}, did update work? {1}", stock["pluralsight"], success));

            success = stock.TryUpdate("pluralsight", 8, 6); // update from 6 to 8. but previously, we changed from 6 to 7. So, rather throwing an expcetion, this line will return false
            Console.WriteLine(string.Format("pluralsight = {0}, did update work? {1}", stock["pluralsight"], success));

            int jDaysValue;
            success = stock.TryRemove("jDays", out jDaysValue);
            Console.WriteLine("value removed was: " + success);

            Console.WriteLine("\r\nEnumerating:");
            foreach (var keyValPair in stock)
            {
                Console.WriteLine("{0}: {1}", keyValPair.Key, keyValPair.Value);
            }
        }

        private static void DictionaryStandard()
        {
            var stock = new Dictionary<string, int>()
            {
                {"jDays", 4 },
                {"technolgyhour", 3 }
            };
            Console.WriteLine(string.Format("No. of shirts in stock = {0}", stock.Count));

            stock.Add("pluralsight", 6);
            stock["budidstgeeks"] = 5;

            stock["pluralsight"] = 7;
            Console.WriteLine(string.Format("\r\nstock[pluralsight] = {0}", stock["pluralsight"]));

            stock.Remove("jDays");

            Console.WriteLine("\r\nEnumerating:");
            foreach (var keyValPair in stock)
            {
                Console.WriteLine("{0}: {1}", keyValPair.Key, keyValPair.Value);
            }
        }

        private static void ConcurrentDictionaryTheory()
        {
            // ConcurrentDictionary is very simmilar to standard generic Dictionary
            // Many methods in common
            // But ConcurrentDictionary works with Multi thread and does not protect from Race condition
            // Key ConcurrentDictionary methods : AddOrUpdate() and GetOrAdd()
            // 
        }

        private static void TypesOfConcurrentCollection()
        {
            // There are mainly four ConcurrentCollection in .net
            //  1. ConcurrentDictionary<TKey, TValue>
            //  2. ConcurrentQueue<T>
            //  3. ConcurrentStack<T>
            //  4. ConcurrentBag<T>
            // Standard collections like : 
            //  List<T>, Queue<T>, Stack<T>, Dictionary<TKey, TValue> etc does not have ConcurrentCollection version :(
            // So, if we want a General Purpose Thread Safe collection, our only choice is 
            //  - ConcurrentDictionary<TKey, TValue>
            // So, if we want to use a thread safe List<T> or array : we can use ConcurrentDictionray<TKey, TValue> in place
            // Again, if we want ot use a thread safe HashSet<T> : we can use ConcurrentDictionary inplace
        }

        private static void RaceConditionConcurrentCollection()
        {
            // ConcurrentCollection will ensure thread safety
            // But it will give a tension on RACE CONDITION
            // A race condition occurs, when the results of processeing are snesitive to the process order in which code on different thread executes
            // Example : Running :  MultiThreaded_ThreadSafe() multiple times will produce different result
            // THis is beacuse of the variations of the timing of the threads
            // Though in MultiThreaded_ThreadSafe() order of adding in the queue is not a Matter
            // But sometimes it can be a matter depending on the requreiments
            // In those cases, race condition in ConcurrentCollection will produce bug for different timing of thread execution
            // So, ConcurrentCollection 
            //  - Will give thread safety
            //  - But won't protect you from Race Condition
            // So, to use ConCurrentCollection, you need to have a different mind set
            // 
        }

        private static void WhyLockIsNotGoodChoice()
        {
            // We could have use _lock as an alternative soluction instead of ConcurrentCollection for thread safety
            var orders = new Queue<string>();
            Task task1 = Task.Run(() => PlaceOrdersWithLock(orders, "Siam"));
            Task task2 = Task.Run(() => PlaceOrdersWithLock(orders, "Samit"));
            Task.WaitAll(task1, task2);

            foreach (string order in orders)
                Console.WriteLine("ORDER: " + order);

            // Question : If _lock is for save, why bother about ConcurrentCollection?
            // - we have to add in many places the _lock, so will not be maintanable
            // - When the thread increases, Thread gets blockes more and more often with _lock statement, this is because, each thread will run at a time
            // - ^So, its not scalable
            // With ConcurrecntCollection, the order of thread exectuion will be place in a friendly manner with some efficient algorithm in behind
            // So, code will become more scalable too.
        }



        private static void AtomicMethod()
        {
            // An operation is Atomic if it satisfies : 
            //  1. Other threads will see this method either NEVER STARTED or COMPLETED (NOT as part-complete)
            //  2. Method will finish either sucessfully or fail without making any changes to the data it is working on (no matter what other threads are doing at that time)
            // Normal Queue.Enqueue() is not threadsafe as it is not Atomic.
            // Other threads may find this as half completed (check MSDN)
            // But ConcurrentQueue.Enqueue() is atomic by the use of some internal clever algorithm
            // And the same is true for most of the ConcurrentCollection method
            // This is why ConcurrentCollection ensures Thread Safety, becasue they never exposes half completed operations to other threads

        }

        private static void MultiThreaded_NotThreadSafe()
        {
            var orders = new Queue<string>();
            // Place orders for two different user in two different thread in parallel but in the same queue
            // Will cause, either exception or un-expected result as Queue is not Thread safe
            Task task1 = Task.Run(() => PlaceOrders(orders, "Siam"));
            Task task2 = Task.Run(() => PlaceOrders(orders, "Samit"));
            Task.WaitAll(task1, task2);

            foreach (string order in orders)
                Console.WriteLine("ORDER: " + order);
        }

        private static void MultiThreaded_ThreadSafe()
        {
            // This will be thread safe
            // But order of adding orders in the queue will not be same for each execution
            // Means, sometimes Siam order will be placed first and some times Samit
            // This is because, we did not specified the order of execution of the thread
            // This is the only minor thing, other than that, it's safe to use and Thread Safe
            var orders = new ConcurrentQueue<string>();
            Task task1 = Task.Run(() => PlaceOrders(orders, "Siam"));
            Task task2 = Task.Run(() => PlaceOrders(orders, "Samit"));
            Task.WaitAll(task1, task2);

            foreach (string order in orders)
                Console.WriteLine("ORDER: " + order);
        }

        private static void PlaceOrders(ConcurrentQueue<string> orders, string customerName)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1);
                string orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
                orders.Enqueue(orderName);
            }
        }

        private static void PlaceOrders(Queue<string> orders, string customerName)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1);
                string orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
                orders.Enqueue(orderName);
            }
        }

        static object _lockObj = new object();

        private static void PlaceOrdersWithLock(Queue<string> orders, string customerName)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1);
                string orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
                lock (_lockObj)
                {
                    orders.Enqueue(orderName);
                }
            }
        }
    }
}
