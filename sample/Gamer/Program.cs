using StatelessSCXML;
using System;
using System.Threading;
using System.Xml;

namespace Gamer
{
    class Program
    {
        static void Main(string[] _)
        {
            Console.WriteLine("Creating an online gamer state machine from SCXML.\nActions:" +
                "1: Attempt to connect" +
                "2: Cancel connection" +
                "3: Disconnect" +
                "any other key: quit");

            // Provide SCXML input
            var xml = new XmlDocument();
            xml.Load("scxml.xml");

            var parser = new SCXMLToStateless(xml);
            var machine = parser.CreateStateMachine();

            while (true)
            {
                Console.Write("\nEnter event number: ");
                var keyInfo = Console.ReadKey();
                Thread.Sleep(400);

                var eventName = keyInfo.KeyChar switch
                {
                    var one when one == '1' => "connect.start",
                    var one when one == '2' => "connect.cancel",
                    var one when one == '3' => "disconnect",
                    var _ => "quit"
                };

                if (eventName.Equals("quit")) break;

                machine.RaiseEvent(eventName);
            }

            Console.WriteLine("Finished game. Press any key...");
            Console.ReadKey();

            //machine.Configure(new SCXMLState("sd")).OnEntry(DisplayMessage);
        }

        static void DisplayMessage() => Console.WriteLine("++++");
    }
}
