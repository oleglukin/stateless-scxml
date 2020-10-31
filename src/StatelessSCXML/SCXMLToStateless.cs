using Stateless;
using System;
using System.Collections.Generic;
using System.Xml;

namespace StatelessSCXML
{
    public class SCXMLToStateless
    {
        private readonly XmlDocument _xml;

        private readonly List<string> _states = new List<string>();

        public SCXMLToStateless(XmlDocument xml)
        {
            _xml = xml;
            var root = xml.DocumentElement;

            foreach (XmlNode xnode in root)
            {
                if (xnode.Name.Equals("state"))
                {
                    Console.Write("state: ");

                    var idAttr = xnode.Attributes.GetNamedItem("id");

                    if (idAttr != null)
                    {
                        var id = idAttr.Value;


                        _states.Add(id);
                    }
                    else
                    {
                        Console.WriteLine($"State doesn't have an id. Skipping.");
                    }
                }


                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes[0];
                    if (attr != null)
                        Console.WriteLine(attr.Value);
                }
                // обходим все дочерние узлы элемента user
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    // если узел - company
                    if (childnode.Name == "transition")
                    {
                        Console.WriteLine($"transition: {childnode.InnerText}");
                    }
                    // если узел age
                    if (childnode.Name == "age")
                    {
                        Console.WriteLine($"Возраст: {childnode.InnerText}");
                    }
                }
                Console.WriteLine();
            }
        }




        private enum State { Inside, Outside }

        private enum Trigger { Enter, Exit }

        public Type GetStates()
        {
            return typeof(State);
        }

        public Type GetEvents()
        {
            return typeof(Trigger);
        }

        public object GetStateMachine()
        {
            // TODO implement machine configuration
            var machine = new StateMachine<State, Trigger>(State.Outside);

            machine.Configure(State.Outside)
                .Permit(Trigger.Enter, State.Inside)
                .Ignore(Trigger.Exit);

            machine.Configure(State.Inside)
                .Permit(Trigger.Exit, State.Outside)
                .Ignore(Trigger.Enter);

            return machine;
        }
    }
}
