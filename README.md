# Objectflow: Workflows for busy people

Objectflow is a framework for creating workflows in C# with a fluent syntax. Objectflow manages those pesky 
transitions between the database and your web application with ease.

Worflows are defined in cycles of `Start` `Do` `Yield` order. The `Start` part isn't explicitly defined, 
it's just the beginning of the workflow. `Do` is any unit of work that needs to be done. `Yield` says to 
pause the current execution of the workflow so that it can be stored in a database or returned to a user.

	// Define a workflow
	var workflow = new StatefulWorflow<SiteVisit>()
		.Do(x => x.Initialize())
		.Yield("Scheduled")
		.Do(x => x.Open())
		.Yield("In Progress")
		.Do(x => x.Close());
		
	// Start an object through the workflow
	workflow.Start(new SiteVisit());

We also provide other facilities for interacting with workflows to provide persistance and security frameworks.
See the [Quickstart guide](https://github.com/tkellogg/objectflow/wiki/Quickstart-Guide) for more information.