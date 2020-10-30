using Stateless;
using System;

namespace InOut
{
    class PersonSCXML<TState, TEvent> : IPerson
        where TState : Enum
        where TEvent : Enum
    {
        private readonly StateMachine<TState, TEvent> _machine;

        public PersonSCXML(object machine)
        {
            _machine = (StateMachine<TState, TEvent>)machine;
        }

        public void DisplayState() => Console.WriteLine($"Person is {_machine.State}");

        public void Enter()
        {
            if (Enum.TryParse(typeof(TEvent), "enter", true, out object? trigger))
                _machine.Fire((TEvent)trigger);
            else
                throw new Exception($"{nameof(TEvent)} does not have 'enter' event");
        }

        public void Exit()
        {
            if (Enum.TryParse(typeof(TEvent), "exit", true, out object? trigger))
                _machine.Fire((TEvent)trigger);
            else
                throw new Exception($"{nameof(TEvent)} does not have 'exit' event");
        }
    }
}
