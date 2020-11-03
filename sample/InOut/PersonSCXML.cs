using Stateless;
using StatelessSCXML;
using System;

namespace InOut
{
    // This state machine is received from the SCXMLToStateless compiler
    class PersonSCXML : IPerson
    {
        private readonly StateMachine<SCXMLState, Transition> _machine;

        public PersonSCXML(StateMachine<SCXMLState, Transition> machine) => _machine = machine;

        public void DisplayState() => Console.WriteLine($"Person is {_machine.State.Name}");

        public void Enter() => _machine.RaiseEvent("enter");

        public void Exit() => _machine.RaiseEvent("exit");
    }
}
