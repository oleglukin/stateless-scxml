using Stateless;
using System.Xml;
using Xunit;

namespace StatelessSCXML.Tests
{
    /// <summary>
    /// Tests to make sure that SCXMLToStateless can parse transitions with multiple events
    /// </summary>
    public class MultipleEvents
    {
        // This SCXML has a transition (from Moving state) that has multiple events
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

        public MultipleEvents()
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
