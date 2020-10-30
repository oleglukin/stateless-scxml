using Stateless;
using System;

namespace StatelessSCXML
{
    public class SCXMLToStateless
    {
        private enum State { Inside, Outside }

        private enum Trigger { Enter, Exit }

        public Type GetStates()
        {
            return typeof(State);
        }

        public Type GetEvents()
        {
            return typeof(Trigger);
        }

        public object GetStateMachine()
        {
            // TODO implement machine configuration
            var machine = new StateMachine<State, Trigger>(State.Outside);

            machine.Configure(State.Outside)
                .Permit(Trigger.Enter, State.Inside)
                .Ignore(Trigger.Exit);

            machine.Configure(State.Inside)
                .Permit(Trigger.Exit, State.Outside)
                .Ignore(Trigger.Enter);

            return machine;
        }
    }
}
