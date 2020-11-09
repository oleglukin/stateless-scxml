using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace StatelessSCXML
{
    /// <summary>
    /// SCXML document to Stateless state machine compiler
    /// </summary>
    public class SCXMLToStateless
    {
        private readonly List<SCXMLState> _states;
        private readonly SCXMLState _initialState;

        
        public SCXMLToStateless(XmlDocument xml) => _states = SCXMLParser.Parse(xml, out _initialState);



        /// <summary>
        /// Compile Stateless state machine based on data parsed from the input SCXML document
        /// </summary>
        /// <returns>Stateless state machine</returns>
        public StateMachine<SCXMLState, Transition> CreateStateMachine(object caller = null)
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

                // configure OnEntry method invocation
                if (!string.IsNullOrEmpty(s.OnEntry))
                {
                    Type type = caller?.GetType();
                    MethodInfo method = type?.GetMethod(s.OnEntry);

                    if (method != null)
                    {
                        machine.Configure(s).OnEntry(t => method.Invoke(caller, null));
                    }
                }
            });

            return machine;
        }
    }
}
