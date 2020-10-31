using System.Collections.Generic;

namespace StatelessSCXML
{
    public class SCXMLState
    {
        public string Name { get; }
        public List<Transition> Transitions { get; } = new List<Transition>();

        public SCXMLState(string name) => Name = name;
    }
}
