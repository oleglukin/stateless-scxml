using Stateless;
using System.Linq;

namespace StatelessSCXML
{
    public static class StateMachineExtensions
    {
        /// <summary>
        /// Helper method to raise events to trigger state transitions
        /// </summary>
        /// <param name="machine">Stateless state machine</param>
        /// <param name="eventName">Name of the event to raise</param>
        public static void RaiseEvent(this StateMachine<SCXMLState, Transition> machine, string eventName)
        {
            var trigger = machine.PermittedTriggers.Where(t => t.Event.Equals(eventName)).First();
            machine.Fire(trigger);
        }
    }
}
