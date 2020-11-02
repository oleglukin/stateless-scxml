# stateless-scxml
SCXML to Stateless compiler.  
Refer to issue [387](https://github.com/dotnet-state-machine/stateless/issues/387) in dotnet-state-machine / stateless

## Examples
SCXML document:

```
<scxml initial="Outside" version="1.0" xmlns="http://www.w3.org/2005/07/scxml">
  <state id="Outside">
    <transition event="enter" target="Inside" />
  </state>
  <state id="Inside">
    <transition event="exit" target="Outside" />
  </state>
</scxml>
```

Load this document and use library API to compile it to a Stateless state machine:

```
var xml = new XmlDocument();
xml.Load("scxml.xml");

var parser = new SCXMLToStateless(xml);
var machine = parser.CreateStateMachine();

// trigger machine events like this:
var trigger = machine.PermittedTriggers.Where(t => t.Event.Equals("enter")).First();
machine.Fire(trigger);
```

The state machine compiled from SCXML would be equivalent to manual configuration like this:

```
machine = new StateMachine<State, Trigger>(State.Outside);

machine.Configure(State.Outside)
	.Permit(Trigger.Enter, State.Inside)
	.Ignore(Trigger.Exit);

machine.Configure(State.Inside)
	.Permit(Trigger.Exit, State.Outside)
	.Ignore(Trigger.Enter);
```


## Useful links
[Qt SCXML Examples](https://doc.qt.io/qt-5/examples-qtscxml.html)  
[JSSCxml](https://jsscxml.org) SCXML Viewer  
[SCION tutorials](https://scion.scxml.io/tutorials/fundamentals)  