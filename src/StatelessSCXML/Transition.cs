namespace StatelessSCXML
{
    public class Transition
    {
        public string Event { get; }
        public SCXMLState Target { get; }

        public Transition(string _event, SCXMLState target) => (Event, Target) = (_event, target);
    }
}
