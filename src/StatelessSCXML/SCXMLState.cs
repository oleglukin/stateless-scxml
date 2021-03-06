﻿using System.Collections.Generic;

namespace StatelessSCXML
{
    /// <summary>
    /// State machine state type
    /// </summary>
    public class SCXMLState
    {
        /// <summary>
        /// State name, e.g. Off, On Call
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of transitions from current state into other states
        /// </summary>
        public List<Transition> Transitions { get; } = new List<Transition>();

        /// <summary>
        /// Method name to invoke on entering this state
        /// </summary>
        public string OnEntry { get; set; }

        public SCXMLState(string name) => Name = name;
    }
}
