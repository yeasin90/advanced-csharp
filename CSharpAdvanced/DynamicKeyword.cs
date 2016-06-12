using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;
using Newtonsoft.Json;

namespace CSharpAdvanced
{
    public class DynamicKeyword
    {
        public void Basic()
        {
            // There are two types of Binding
            // 1. Static Binding and 2. Dynamic Binding
            // 1. Static Binding
            //      Say, we have Calculator c = new Calculator()
            //                   c.Add(100)   
            //      here, "Add" is Syntactic element, which Binds to Add() method of variable c representing a Calculator object
            //      this is called Static Binding
            //      Static Binding occurs when we compile the program
            //      When we build in VS - Static binding takes place
            //      If we try, c.Xyz(100) - will throw error because of no such Static Binding
            // 2. Dynamic Binding
            //      dynamic c = CreateCalculator();
            //      c.Add(100)
            //      c.Xyz(100)
            //      Because, c is dynamic - binding will occur at run time rather than compile time like Static Binding
            //      Compiler doesn't know if Add() or Xyz() exists
            //      So, in run-time if object does not contain Add or Xyz method, RunTimeBinding exception will be thrown
            //
            //  So, we can say Even with dynamic C#, type safety is still enforced, only this time it's at run time
        }

        public void DynamicBindingAndRuntimeBindingException()
        {
            // Static Binding
            DateTime dt1 = DateTime.Now;
            string time1 = dt1.ToLongTimeString();
            Console.WriteLine(time1);

            // Dynamic Binding
            dynamic dt2 = DateTime.Now;
            string time2 = dt2.ToLongTimeString(); // dt2 will not give intellisence, because of dynamic binding and will decide at runtime

            // Dynamic Binding with RuntimeBindingException
            dynamic dt3 = DateTime.Now;
            string time3 = dt2.SomeExpextedFunction(); // this line will throw exception at run-time
            // So, runtime expcetion are similar to compile time error, only difference is one happend at runtime another in compile time

            Console.WriteLine("\n\nPress enter to exit....");
            Console.ReadLine();
        }

        public void ImplicitDynamicConversion()
        {
            // dynamic can be implicitly converted to and from other types
            // No casting code is required
            // Converstion happens in Run time
            // dynamic variable can change type at runtime

            int i = 42;
            dynamic di = i; // No explicit cast, because dynamic type is implicitly convertible to and from any other type 
            int i2 = di;

            Console.WriteLine($"i = {i} di = {di} i2 = {i2}");

            // But belows will throw RuntimeBinding expcetion
            string s = "hello";
            dynamic ds = s;
            int x = ds;

            // Can't implicitly convert long to int exception at runtime
            long l = 99;
            dynamic dl = l;
            int y = dl;
            // to solve above, we have to use explicit cast
            long l1 = 99;
            dynamic dl1 = l1;
            int y1 = (int)dl1;

            // dynamic variable can change it's value at any point of program
            dynamic z = "Hi there";
            Console.WriteLine($"z is a {z.GetType()} = {z}");
            z = 42;
            Console.WriteLine($"z is a {z.GetType()} = {z}");

            Console.WriteLine("\n\nPress enter to exit....");
            Console.ReadLine();
        }

        public void VarAndDynamic()
        {
            // Static (compile time) type of d is dynamic
            // Run time type will be string
            // So, in compile time, d will not be string
            // But, in runtime d will be string
            dynamic d = "Hi there";

            // Static (compile time) type of s is string
            // Run time type will also be string
            string s = "Hi there";

            // Static (compile time) type of s is string
            // Run time type will also be string
            var s2 = "Hi there";

            // So, difference is var is compile time and dynamic is runtime
            // var = compiler working out the type
            // dynamic=  Runtime working out the type
        }

        public void RuntimeMethodResolution()
        {
            int i = 42;
            CommonMethod.PrintMe(i); // --> PrintMe(int value) static binding

            dynamic d;
            Console.WriteLine("Create [i]nt or [d]ouble");
            ConsoleKeyInfo choice = Console.ReadKey(intercept: true);
            if (choice.Key == ConsoleKey.I)
            {
                d = 99;
            }
            else
            {
                d = 55.5;
            }
            // for d = 99, PrintMe(int) will be called
            // for d = 55.5 PrintMe(dynamic) will be called, because we have long but not double. dynamic will always find the best possible matching 
            CommonMethod.PrintMe(d);

            d = long.MaxValue;
            // here, PrintMe(long) will be invoked, because we have an overload for long before dynamic overload
            CommonMethod.PrintMe(d);

            d = "Hello";
            // here, PrintMe(dynamic) will be called because no overload for string
            CommonMethod.PrintMe(d);
        }

        public void DynamicVsObject()
        {
            // This is because : in C#, structure of both object and dynamic are same
            // Difference is : dynamic references allows dynamic operations to occur on the object that it points to. 
            // But if we defince a variable as type object, we are not allowed to perform any dynamic operation on that object. like : we have to do boxing-unboxing for object type
        }

        class Customer
        {
            public object FirstName { get; set; }
            public dynamic SecondName { get; set; }
        }

        public void DynamicandObjectTypes()
        {
            Customer c = new Customer();
            // below is the differen of object and dynamic. one is generating compile error, and another will execute in runtime
            //c.FirstName.SomeDynamicMethod();
            //c.SecondName.SomeDynamicMethod();

            // Using reflection to investigate property types : object and dynamic
            PropertyInfo firstNameProperty = typeof(Customer).GetProperty("FirstName");
            foreach (var attribute in firstNameProperty.CustomAttributes)
            {
                Console.WriteLine(attribute);
            }
            Console.WriteLine($"{firstNameProperty.PropertyType} FirstName");

            PropertyInfo secondNameProperty = typeof(Customer).GetProperty("SecondName");
            foreach (var attribute in secondNameProperty.CustomAttributes)
            {
                Console.WriteLine(attribute);
            }
            Console.WriteLine($"{secondNameProperty.PropertyType} SecondName");
        }

        public void LimitationsOfCallableMethodWithDynamic()
        {
            // 1. Extension methods cannot be applied on dynamic types
            // example : 
            // static class StringExtensions
            // {
            //      public static string PrependHello(this string s)
            //      {
            //          return $"Hello {s}";
            //      }
            // }
            //
            //
            // dynamic gentry = "Gentry";
            // gentry.PrependHello(); // RuntimeBindingExpcetion
            // this is because, extension methods are compile time concept

            // 2. If our class implements an interface explicityly, then dynamic not usabel
            // 3. Try to consume return value of a void method
            // example : 
            // class Person
            // {
            //      public void DoStuff()
            //      {
            //          Console.WriteLine("Hhihi");
            //      }
            // }
            //
            // dynamic p = new Person();
            // var x = p.DoStuff(); // RuntimeBinderException
            // Cannot implicitly convert type void to object
        }

        public void ExpandoObject()
        {
            // ExpandoObject is a C# dynamic type
            // Replace Dictionary with more dynamicity
            var customer = new Dictionary<string, string>();
            customer.Add("ID", "42");


            dynamic newObj = new ExpandoObject();
            newObj.ID = "42";
            newObj.SB = new StringBuilder("a string builder");
            // ExpandoObject has no Add method like dictionary
            // But ExpandoObject inherits IDictionary and IEnumerable
            // So, to get Dictionary behaviours, we have to cast ExpandoObject to Dictionary
            var temp = (IDictionary<string, object>)newObj;
            temp.Add("Key3", 123);


            // Adding behaviour to ExpandoObject
            dynamic newObj2 = new ExpandoObject();
            newObj2.Print = (Action)(() =>
            {
                foreach (var item in newObj2)
                {
                    Console.WriteLine($"{item.Key} : {item.Value}");
                }
            });

            newObj2.Count = (Func<int>)(() =>
            {
                var c = (IDictionary<string, object>)newObj2;
                return c.Count;
            });

            newObj2.ID = "42";
            newObj2.SB = new StringBuilder("a string builder");

            newObj2.Print();
            int count = newObj2.Count();
        }

        public void RefactorReflectionWithDynamic()
        {
            // InvokeMethodUsingReflection
            StringBuilder sb1 = new StringBuilder();
            sb1.GetType()
                .InvokeMember("AppendLine",
                BindingFlags.InvokeMethod,
                null,
                sb1,
                new object[] { "Hello Reflection!" });


            // InvokeMethodUsingDynamic
            StringBuilder sb2 = new StringBuilder();
            ((dynamic)sb2).AppendLine("Hello dynamic!");
        }

        public void UnifiedNumericMethod()
        {
            // There is not common interface for numeric type in .NET
            // So, we hhave to write several overloads for each numeric type : int, double, float, long etc
            // Here, we will write a single method that can take any numeric type
            int int1 = 5;
            int int2 = 2;

            CommonMethod.Add(int1, int2);

            double d1 = 20;
            double d2 = 30;

            CommonMethod.Add(d1, d2);

            // But below can cause runtime error : 
            var result = CommonMethod.Add(d1, d2);
        }

        public class Customer2
        {
            public string FirstName { get; set; }
            public string SecondName { get; set; }
        }

        public void JsonWithDynamic()
        {
            const string CustomerJson = "{'FirstName': 'Sarah','SecondName': 'Smith'}";
            Customer2 c = JsonConvert.DeserializeObject<Customer2>(CustomerJson);

            var firstName = c.FirstName;
            var secondName = c.SecondName;

            // With dynamic, no need for a new strongly type model class
            dynamic c2 = JsonConvert.DeserializeObject(CustomerJson);
            var firstName2 = c2.FirstName;
            var secondName2 = c2.SecondName;
        }
    }

    static class CommonMethod
    {
        public static void PrintMe(int value)
        {
            Console.WriteLine($"PrintMe(int) called value: {value}");
        }

        public static void PrintMe(long value)
        {
            Console.WriteLine($"PrintMe(long) called value: {value}");
        }

        public static void PrintMe(dynamic value)
        {
            Console.WriteLine($"PrintMe(dynamic) called with a {value.GetType()} value: {value}");
        }

        // But below will cause compile time error
        // This is because : in C#, structure of both object and dynamic are same
        // Difference is : dynamic references allows dynamic operations to occur on the object that it points to. 
        // But if we defince a variable as type object, we are not allowed to perform any dynamic operation on that object. like : we have to do boxing-unboxing for object type
        public static void PrintMe(object value)
        {

        }

        //public static T Add<T>(T a, T b)
        //{
        //    // below line will give error because, + cannot be apply in T
        //    // on the other hand, we cannot use generic constrain, because, there will be lots of numeric type
        //    // So, generic solution for making Common numeric method will not work
        //    return a + b;
        //}

        // Dynamic Solution
        public static dynamic Add(dynamic a, dynamic b)
        {
            dynamic result = a + b;
            return result;
        }

        // Dynamic with generic solution. Type safe
        public static T Add<T>(T a, T b)
        {
            dynamic result = (dynamic)a + b;
            return result; // With this line, we are relying on implicit cast, which may not always be safe
        }

        // More safer version with explicit casting
        public static T Add2<T>(T a, T b)
        {
            dynamic result = (dynamic)a + b;
            return (T)result;
        }
    }
}
