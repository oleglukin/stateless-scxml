using Stateless;
using System;

namespace InOut
{
    // The simplest possible use of state machine, configured the usual way
    class Person : IPerson
    {
        // a person can be outside or inside a building
        private enum State { Inside, Outside }

        private enum Trigger { Enter, Exit }

        private readonly StateMachine<State, Trigger> _machine;


        public Person()
        {
            // initially persion is outside
            _machine = new StateMachine<State, Trigger>(State.Outside);

            _machine.Configure(State.Outside)
                .Permit(Trigger.Enter, State.Inside)
                .Ignore(Trigger.Exit);

            _machine.Configure(State.Inside)
                .Permit(Trigger.Exit, State.Outside)
                .Ignore(Trigger.Enter);
        }

        public void Enter() => _machine.Fire(Trigger.Enter);
        public void Exit() => _machine.Fire(Trigger.Exit);
        public void DisplayState() => Console.WriteLine($"Person is {_machine.State}");
    }
}
