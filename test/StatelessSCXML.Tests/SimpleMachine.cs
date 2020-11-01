using Stateless;
using System.Linq;
using System.Xml;
using Xunit;

namespace StatelessSCXML.Tests
{
    public class SimpleMachine
    {
        private readonly string scxml = @"
            <scxml initial=""Outside"" version=""1.0"" xmlns=""http://www.w3.org/2005/07/scxml"">
              <state id=""Outside"">
                <transition event=""enter"" target=""Inside"" />
              </state>
              <state id=""Inside"">
                <transition event=""exit"" target=""Outside"" />
              </state>
            </scxml>";

        private readonly StateMachine<SCXMLState, Transition> machine;

        public SimpleMachine()
        {
            var xml = new XmlDocument();
            xml.LoadXml(scxml);
            var parser = new SCXMLToStateless(xml);
            machine = parser.CreateStateMachine();
        }


        [Fact]
        public void VerifyInitialState() => Assert.Equal("Outside", machine.State.Name);


        [Fact]
        public void VerifyStateTransition()
        {
            var trigger = machine.PermittedTriggers.Where(t => t.Event.Equals("enter")).First();
            machine.Fire(trigger);
            Assert.True(machine.State.Name.Equals("Inside"),
                "Enter event triggered, it is expected to transition into the Inside state");
        }


        [Fact]
        public void InvalidStateTransitionShouldBeIgnored()
        {
            var trigger = machine.PermittedTriggers.Where(t => t.Event.Equals("enter")).First();
            machine.Fire(trigger);
            machine.Fire(trigger);
            Assert.True(machine.State.Name.Equals("Inside"),
                "Enter event triggered twice, it is expected to transition into the Inside state and stay there");
        }


        [Fact]
        public void VerifymultipleStateTransitions()
        {
            var enterTrigger = machine.PermittedTriggers.Where(t => t.Event.Equals("enter")).First();
            machine.Fire(enterTrigger);
            var exitTrigger = machine.PermittedTriggers.Where(t => t.Event.Equals("exit")).First();
            machine.Fire(exitTrigger);
            Assert.True(machine.State.Name.Equals("Outside"),
                "Enter event triggered, then exit event triggered. It is expected to transition " +
                "into the Inside and then back to Outside");
        }
    }
}
