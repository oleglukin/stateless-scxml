namespace StatelessSCXML
{
    /// <summary>
    /// State transition
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// Triggering event
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// Target state
        /// </summary>
        public SCXMLState Target { get; }

        public Transition(string _event, SCXMLState target) => (Event, Target) = (_event, target);
    }
}
