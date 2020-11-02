using StatelessSCXML;
using System;
using System.Xml;

namespace InOut
{
    class Program
    {
        static void Main(string[] _)
        {
            Console.WriteLine("Creating person state machine using C# code:");
            var person1 = new Person();
            TestPersonStates(person1);

            Console.WriteLine("\n\nCreating person state machine from SCXML:");

            // Provide SCXML input
            var xml = new XmlDocument();
            xml.Load("scxml.xml");

            var parser = new SCXMLToStateless(xml);
            var machine = parser.CreateStateMachine();

            var person2 = new PersonSCXML(machine);
            TestPersonStates(person2);

            Console.ReadKey();
        }

        // Display states, trigger transition events
        private static void TestPersonStates(IPerson person1)
        {
            person1.DisplayState();

            Console.Write("Trying to enter => ");
            person1.Enter();
            person1.DisplayState();

            Console.Write("Trying to enter again => ");
            person1.Enter();
            person1.DisplayState();

            Console.Write("Trying to exit => ");
            person1.Exit();
            person1.DisplayState();
        }
    }
}
