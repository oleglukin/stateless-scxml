namespace StatelessSCXML
{
    public class Transition
    {
        public string Event { get; }
        public string Target { get; }

        public Transition(string _event, string target) => (Event, Target) = (_event, target);
    }
}
