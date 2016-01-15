using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    public class StaticCtor
    {
        //1. Will Execute only once
        //2. Execute before object ctor.
        //3. Should be parameterless.
        //4. Used to initialize static varibles only
        //5. It can be used mainly where, in our program, we want to do something before any reference for the class is created, like changing the background of the application.
        //6. If this clss hsa only a static ctor, it can be newd like StaticCtor obj = new StaticCtor();
        //7. StaticCtor obj1 = new StaticCtor(); and StaticCtor ob2 = new StaticCtor(); korleo akbar ei static ctor invoke hobe.
        static StaticCtor()
        {
            Console.WriteLine("From Static CTor");
        }

        public StaticCtor(string number)
        {
            Console.WriteLine("From Object CTor : " + number);
        }
    }

    public class PrivteCtor
    {
        //1. Prevents from creting object from outside.
        //2. Prevents from being inherited
        private PrivteCtor()
        {
            Console.WriteLine("From Privte Ctor");
        }

        public PrivteCtor(string number)
            : this()
        {
            Console.WriteLine("From Public ctor");
        }
    }

    //1. Used only for serving as a container
    //2. Can have both static and non-static fields
    //3. a static class cannot be instantiated. In other words, you cannot use the new keyword to create a variable of the class type. 
    //4. A static class can be used as a convenient container for sets of methods that just operate on input parameters and do not have to get or set any internal instance fields. 
    //   For example, in the .NET Framework Class Library, the static System.Math class contains methods that perform mathematical operations, without any requirement to store or retrieve data that is unique to a particular instance of the Math class. 
    //   That is, you apply the members of the class by specifying the class name and the method name, as shown in the following example. 
    //Example : Console.WriteLine(Math.Abs(dub));
    //          Console.WriteLine(Math.Floor(dub));
    //5. Creating a static class is therefore basically the same as creating a class that contains only static members and a private constructor. 
    //6. Static classes are sealed and therefore cannot be inherited. 
    //7. Static classes cannot contain an instance constructor; however, they can contain a static constructor. 
    public static class StaticClss
    {
        public static string Hi { get; set; }
        // This Static CTor will not be invoked.
        static StaticClss()
        {
            Console.WriteLine("Static CLass");
        }
    }

    // Singleton 1
    public sealed class Singleton1
    {
        public string Name { get; set; }
        public static Singleton1 _singletonObj;

        static Singleton1()
        {
            _singletonObj = new Singleton1();
        }
    }

    class Singleton2
    {
        private static Singleton2 _instance;
        private Singleton2()
        {

        }

        public static Singleton2 Instance()
        {
            if (_instance == null)
            {
                _instance = new Singleton2();
            }
            return _instance;
        }
    }
}
