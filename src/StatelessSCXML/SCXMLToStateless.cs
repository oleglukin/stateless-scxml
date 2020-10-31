using Stateless;
using System;
using System.Collections.Generic;
using System.Xml;

namespace StatelessSCXML
{
    public class SCXMLToStateless
    {
        private readonly XmlDocument _xml;

        private readonly List<SCXMLState> _states = new List<StatelessSCXML.SCXMLState>();

        public SCXMLToStateless(XmlDocument xml)
        {
            _xml = xml;
            var root = xml.DocumentElement;

            foreach (XmlNode xnode in root)
            {
                if (xnode.Name.Equals("state"))
                {
                    var idAttr = xnode.Attributes.GetNamedItem("id");
                    if (idAttr != null)
                    {
                        var state = new SCXMLState(idAttr.Value);
                        _states.Add(state);

                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (childnode.Name.Equals("transition"))
                            {
                                var eventAttr = childnode.Attributes.GetNamedItem("event");
                                var targetAttr = childnode.Attributes.GetNamedItem("target");

                                if (eventAttr != null && targetAttr != null)
                                {
                                    var transition = new Transition(eventAttr.Value, targetAttr.Value);

                                    state.Transitions.Add(transition);
                                }
                            }
                            // TODO check if child node is not a transition => process
                        }
                    }
                    else
                        Console.WriteLine($"State doesn't have an id. Skipping.");
                }

                // TODO other root nodes (not states)
            }
        }




        private enum State { Inside, Outside }

        private enum Trigger { Enter, Exit }

        public Type StateType => typeof(State);

        public Type EventType => typeof(Trigger);

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
