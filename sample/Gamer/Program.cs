using StatelessSCXML;
using System;
using System.Threading;
using System.Xml;

namespace Gamer
{
    /// <summary>
    /// This sample has the following features:
    /// - OnEnter method invocation
    /// - Multiple events in one transition
    /// - Child events
    /// </summary>
    class Program
    {
        static void Main(string[] _)
        {
            var worker = new Worker();
            worker.Start();
        }
    }


    class Worker
    {
        internal void Start()
        {
            Console.WriteLine("Creating an online gamer state machine from SCXML.\nActions:" +
                "\n1: Attempt to connect" +
                "\n2: Cancel connection process" +
                "\n3: Timeout connection process" +
                "\n4: Connected" +
                "\n5: Disconnect" +
                "\n6: Quit");

            // Provide SCXML input
            var xml = new XmlDocument();
            xml.Load("scxml.xml");

            var parser = new SCXMLToStateless(xml);
            var machine = parser.CreateStateMachine(caller: this);

            while (true)
            {
                Console.Write("\nEnter event number (1-6): ");
                var keyInfo = Console.ReadKey();
                Thread.Sleep(400);

                var eventName = keyInfo.KeyChar switch
                {
                    '1' => "connect.start",
                    '2' => "connect.cancel",
                    '3' => "timeout",
                    '4' => "connected",
                    '5' => "disconnect",
                    '6' => "quit",
                    _ => "invalideventname"
                };

                if (eventName.Equals("quit")) break;
                else if (eventName.Equals("invalideventname")) continue;
                else machine.RaiseEvent(eventName);
            }

            Console.WriteLine("\nFinished game. Press any key...");
            Console.ReadKey();
        }

        public static void DisplayMessageConnecting() => Console.Write("\nConnecting...");
        public static void DisplayMessageConnected() => Console.Write("\nConnected!");
    }
}
