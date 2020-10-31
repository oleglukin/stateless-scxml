using StatelessSCXML;
using System;
using System.Xml;

namespace InOut
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating person state machine using C# code:");
            var person1 = new Person();
            TestPerson(person1);

            Console.WriteLine("\n\nCreating person state machine from SCXML:");

            // Provide SCXML input
            var xml = new XmlDocument();
            xml.Load("scxml.xml");

            var parser = new SCXMLToStateless(xml);
            Type state = parser.GetStates();
            Type events = parser.GetEvents();
            object machine = parser.GetStateMachine();

            //IPerson person2 = new PersonSCXML<state, events>();

            Type personType = typeof(PersonSCXML<,>).MakeGenericType(state, events);
            IPerson person2 = (IPerson)Activator.CreateInstance(personType, machine);

            TestPerson(person2);

            Console.ReadKey();
        }

        private static void TestPerson(IPerson person1)
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
