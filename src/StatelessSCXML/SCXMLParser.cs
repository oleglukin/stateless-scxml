﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace StatelessSCXML
{
    internal class SCXMLParser
    {
        /// <summary>
        /// Parse input SCXML file into list of states and transitions
        /// </summary>
        /// <param name="xml">Input SCXML document</param>
        public static List<SCXMLState> Parse(XmlDocument xml, out SCXMLState initialState)
        {
            var states = new List<SCXMLState>();

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
                        states.Add(state);
                    }
                    else
                        Console.WriteLine($"State doesn't have an id. Skipping."); // TODO replace with proper logging
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
                        var state = states.Where(s => s.Name.Equals(idAttr.Value)).First();

                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (childnode.Name.Equals("transition"))
                            {
                                var eventAttr = childnode.Attributes.GetNamedItem("event");
                                var targetAttr = childnode.Attributes.GetNamedItem("target");

                                if (!string.IsNullOrWhiteSpace(eventAttr?.Value) &&
                                    !string.IsNullOrWhiteSpace(targetAttr?.Value))
                                {
                                    var targetState = states.Where(s => s.Name.Equals(targetAttr.Value)).First();

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
            initialState = states.Where(st => st.Name.Equals(initialStateStr)).First(); // TODO check if it cannot be found

            return states;
        }
    }
}
