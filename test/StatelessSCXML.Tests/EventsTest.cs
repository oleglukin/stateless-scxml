using Stateless;
using System.Xml;
using Xunit;

namespace StatelessSCXML.Tests
{
    public class EventsTest
    {
        private readonly string scxml = @"
            <scxml initial=""Idle"" version=""1.0"" xmlns=""http://www.w3.org/2005/07/scxml"">
              <state id=""Idle"">
                <transition event=""startmoving"" target=""Moving"" />
              </state>
              <state id=""Moving"">
                <transition event=""stopmoving reachend"" target=""Idle"" />
              </state>
            </scxml>";

        private readonly StateMachine<SCXMLState, Transition> machine;

        public EventsTest()
        {
            var xml = new XmlDocument();
            xml.LoadXml(scxml);
            var parser = new SCXMLToStateless(xml);
            machine = parser.CreateStateMachine();
        }

        [Fact]
        public void VerifyFirstEvent()
        {
            machine.RaiseEvent("startmoving");
            machine.RaiseEvent("stopmoving"); // first event in the Moving state transition
            Assert.True(machine.State.Name.Equals("Idle"),
                "'startmoving' event triggered, then 'stopmoving' event triggered. It is expected to transition " +
                "into the Moving and then back to Idle");
        }

        [Fact]
        public void VerifySecondEvent()
        {
            machine.RaiseEvent("startmoving");
            machine.RaiseEvent("reachend"); // second event in the Moving state transition
            Assert.True(machine.State.Name.Equals("Idle"),
                "'startmoving' event triggered, then 'reachend' event triggered. It is expected to transition " +
                "into the Moving and then back to Idle");
        }
    }
}
