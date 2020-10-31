using Stateless;
using StatelessSCXML;
using System;
using System.Linq;

namespace InOut
{
    class PersonSCXML : IPerson
    {
        private readonly StateMachine<SCXMLState, Transition> _machine;

        public PersonSCXML(StateMachine<SCXMLState, Transition> machine) => _machine = machine;

        public void DisplayState() => Console.WriteLine($"Person is {_machine.State.Name}");

        public void Enter()
        {
            var trigger = _machine.PermittedTriggers.Where(t => t.Event.Equals("enter")).First();
            _machine.Fire(trigger);
        }

        public void Exit()
        {
            var trigger = _machine.PermittedTriggers.Where(t => t.Event.Equals("exit")).First();
            _machine.Fire(trigger);
        }
    }
}
