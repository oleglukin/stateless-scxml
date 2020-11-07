using Stateless;
using System;
using System.Xml;
using Xunit;

namespace StatelessSCXML.Tests
{
    /// <summary>
    /// Tests to make sure that SCXML can provide configuration for method invocation (e.g. OnEntry, OnExit)
    /// </summary>
    public class MethodInvocation
    {
        private static bool invoked = false;

        // This SCXML has a transition (from Moving state) that has multiple events
        private readonly string scxml = @"
            <scxml initial=""Off"" version=""1.0"" xmlns=""http://www.w3.org/2005/07/scxml"">
              <state id=""Off"">
                <transition event=""switch"" target=""On"" />
              </state>
              <state id=""On"">
                <onentry>
                  <send event=""TestMethod"" />
                </onentry>
                <transition event=""switch"" target=""Off"" />
              </state>
            </scxml>";

        private readonly StateMachine<SCXMLState, Transition> machine;

        public MethodInvocation()
        {
            var xml = new XmlDocument();
            xml.LoadXml(scxml);
            var parser = new SCXMLToStateless(xml);
            machine = parser.CreateStateMachine(this);
        }

        [Fact]
        public void OnEntry()
        {
            invoked = false;
            machine.RaiseEvent("switch");
            Assert.True(invoked, "Raised 'switch' event, entering 'On' state with configured <onentry>. " +
                $"Expected to invoke {nameof(TestMethod)} and change {nameof(invoked)} property to true.");
        }

        public static void TestMethod() => invoked = true;
    }
}
