using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace StatelessSCXML
{
    public class SCXMLToStateless
    {
        private readonly List<SCXMLState> _states = new List<SCXMLState>();
        private readonly SCXMLState _initialState;

        public SCXMLToStateless(XmlDocument xml)
        {
            var root = xml.DocumentElement;

            // create states first
            foreach (XmlNode xnode in root)
            {
                if (xnode.Name.Equals("state"))
                {
                    var idAttr = xnode.Attributes.GetNamedItem("id"); // check if this can be done with XPath
                    if (idAttr != null)
                    {
                        var state = new SCXMLState(idAttr.Value);
                        _states.Add(state);
                    }
                    else
                        Console.WriteLine($"State doesn't have an id. Skipping.");
                }
                // TODO other root nodes (not states)
            }

            // create transitions
            // TODO see if this can be done without going through all nodes (e.g. proper XPath)
            foreach (XmlNode xnode in root)
            {
                if (xnode.Name.Equals("state"))
                {
                    var idAttr = xnode.Attributes.GetNamedItem("id");
                    if (idAttr != null)
                    {
                        var state = _states.Where(s => s.Name.Equals(idAttr.Value)).First();

                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (childnode.Name.Equals("transition"))
                            {
                                var eventAttr = childnode.Attributes.GetNamedItem("event");
                                var targetAttr = childnode.Attributes.GetNamedItem("target");

                                if (eventAttr != null && targetAttr != null)
                                {
                                    var targetState = _states.Where(s => s.Name.Equals(targetAttr.Value)).First();
                                    var transition = new Transition(eventAttr.Value, targetState);
                                    state.Transitions.Add(transition);
                                }
                            }
                            // TODO check if child node is not a transition => process
                        }
                    }
                }
            }

            var initialAttr = root.Attributes.GetNamedItem("initial");
            string initialStateStr = initialAttr?.Value ?? string.Empty;
            _initialState = _states.Where(st => st.Name.Equals(initialStateStr)).First(); // TODO check if it cannot be found
        }


        /// <summary>
        /// Configure each state
        /// </summary>
        /// <returns>Stateless state machine</returns>
        public StateMachine<SCXMLState, Transition> CreateStateMachine()
        {
            var machine = new StateMachine<SCXMLState, Transition>(_initialState);

            _states.ForEach(s =>
            {
                s.Transitions.ForEach(t => machine.Configure(s).Permit(t, t.Target));

                // ignore all other triggers
                _states
                    .SelectMany(state => state.Transitions) // all transitions from all states
                    .Where(transition => !s.Transitions.Contains(transition)) // other transitions that are not in this state
                    .ToList()
                    .ForEach(ot => machine.Configure(s).Ignore(ot));
            });

            return machine;
        }
    }
}
