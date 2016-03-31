using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAdvanced
{
    // reference : http://www.codeproject.com/Articles/741064/Delegates-its-Modern-Flavors-Func-Action-Predicate
    public class DifferenceOfDelegateFuncActionPredicate
    {
        protected delegate int tempFunctionPointer(string strParameter, int intParamater);

        public void UseOfDelegate()
        {
            // When Do I Use Delegate?
            // - Looking at the sample above, a lot of us might think this can be also achieved using Interface or abstract class, then why do we need delegate.

            // Delegate can be used in the following scenarios:
            // - If you don’t want to pass your interface or abstract class dependence to internal class or layers.
            // - If the code doesn't need access to any other attributes or method of the class from which logic needs to be processed.
            // - Event driven implementation needs to be done.

            DelegateSample tempObj = new DelegateSample();
            tempFunctionPointer funcPointer = tempObj.FirstTestFunction;
            funcPointer("hello", 1);
            Console.ReadKey();
            funcPointer = tempObj.SecondTestFunction;
            funcPointer("hello", 1);
            Console.ReadKey();
        }

        public void UseOfFunc()
        {
            // Different Flavors of Delegate
            // As.NET Framework evolved over a period of time, new flavors have been added to keep implementation simple & optimized.
            // By default, you get all the features & functionality with flavors which you get with delegate.Let’s have a look at Func delegate.

            // Func < TParameter, TOutput >

            // Func is logically similar to base delegate implementation.
            // The difference is in the way we declare. At the time of declaration, we need to provide the signature parameter & its return type.

            DelegateSample tempObj = new DelegateSample();
            Func<string, int, int> tempFuncPointer = tempObj.FirstTestFunction;
            int value = tempFuncPointer("hello", 3);
            Console.ReadKey();
        }

        public void UseOfAction()
        {
            // Action<TParameter>

            // Action is used when we do not have any return type from method.Method with void signature is being used with Action delegate.

            // Action<string, int> tempActionPointer;
            // Similar to Func delegate, the first two parameters are the method input parameters.
            // Since we do not have return object or type, all the parameters are considered as input parameters.

            DelegateSample tempObj = new DelegateSample();
            Action<string, int> tempActionPointer = tempObj.ThirdTestFunction;
            tempActionPointer("hello", 4);
            Console.ReadKey();
        }

        public void UseOfPredicate()
        {
            // Predicate <in T >

            // Predicate is a function pointer for method which returns boolean value.
            // They are commonly used for iterating a collection or to verify if the value does already exist. Declaration for the same looks like this:

            // Predicate < Employee > tempPredicatePointer;
            // For sample, I have created an Array which holds a list of Employees. Predicate is used to get employee below age of 27:
            DelegateSample tempObj = new DelegateSample();
            Predicate<Employee> tempPredicatePointer = tempObj.FourthTestFunction;
            Employee[] lstEmployee = (new Employee[]
            {
                   new Employee(){ Name = "Ashwin", Age = 31},
                   new Employee(){ Name = "Akil", Age = 25},
                   new Employee(){ Name = "Amit", Age = 28},
                   new Employee(){ Name = "Ajay", Age = 29},
            });

            Employee tempEmployee = Array.Find(lstEmployee, tempPredicatePointer);
            Console.WriteLine("Person below 27 age :" + tempEmployee.Name);
            Console.ReadKey();
        }

        public void UseOfExpression()
        {
            // http://www.tutorialsteacher.com/linq/linq-lambda-expression
            // http://www.codeproject.com/Articles/24255/Exploring-Lambda-Expression-in-C
        }
    }

    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class XEmployee
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public bool IsExEmployee
        {
            get { return true; }
        }
    }

    public class DelegateSample
    {
        public int FirstTestFunction(string strParameter, int intParamater)
        {
            Console.WriteLine("First Test Function Execution");
            Console.WriteLine(strParameter);
            return intParamater;
        }

        public int SecondTestFunction(string strParameter, int intParamater)
        {
            Console.WriteLine("Second Test Function Execution");
            Console.WriteLine(strParameter);
            return intParamater;
        }

        public void ThirdTestFunction(string strParameter, int intParamater)
        {
            Console.WriteLine("Third Test Function Execution");
            Console.WriteLine(strParameter);
        }

        public bool FourthTestFunction(Employee employee)
        {
            return employee.Age < 27;
        }

        public XEmployee FifthTestFunction(Employee employee)
        {
            return new XEmployee() { Name = employee.Name, Age = employee.Age };
        }

        public int SixTestFunction(Employee strParameter1, Employee strParamater2)
        {
            return strParameter1.Name.CompareTo(strParamater2.Name);
        }
    }
}
