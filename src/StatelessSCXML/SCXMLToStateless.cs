﻿using Stateless;
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
        private readonly List<SCXMLState> _states = new List<SCXMLState>();
        private readonly SCXMLState _initialState;

        /// <summary>
        /// Parse input SCXML file into list of states and transitions
        /// </summary>
        /// <param name="xml">Input SCXML document</param>
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

                                if (!string.IsNullOrWhiteSpace(eventAttr?.Value) &&
                                    !string.IsNullOrWhiteSpace(targetAttr?.Value))
                                {
                                    var targetState = _states.Where(s => s.Name.Equals(targetAttr.Value)).First();

                                    var events = eventAttr.Value.Split(' ')
                                            .Select(s => s.Trim())
                                            .Where(s => !string.IsNullOrWhiteSpace(s));
                                    
                                    foreach (var evnt in events)
                                    {
                                        var transition = new Transition(evnt, targetState);
                                        state.Transitions.Add(transition);
                                    }
                                }
                            }
                            else if (childnode.Name.Equals("onentry") && childnode.HasChildNodes)
                            {
                                foreach (XmlNode onentryChild in childnode.ChildNodes)
                                {
                                    if (onentryChild.Name.Equals("send"))
                                    {
                                        var eventAttr = onentryChild.Attributes?.GetNamedItem("event");
                                        if (!string.IsNullOrWhiteSpace(eventAttr?.Value))
                                        {
                                            state.OnEntry = eventAttr.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var initialAttr = root.Attributes.GetNamedItem("initial");
            string initialStateStr = initialAttr?.Value ?? string.Empty;
            _initialState = _states.Where(st => st.Name.Equals(initialStateStr)).First(); // TODO check if it cannot be found
        }



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
                        machine.Configure(s).OnEntry(t => method.Invoke(null, null));
                    }
                }
            });

            return machine;
        }
    }
}
