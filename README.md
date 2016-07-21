# Pro.NBench.xUnit

Initial, simple approach to allow NBench tests to be discovered, executed, reported, and debugged using xUnit in ReSharper, and to a less polished extent, in Visual Studio Test Explorer.

Heavily influenced by Andrea Angella's approach and article for integrating with NUnit.

http://www.andreaangella.com/2016/03/nbench-performance-testing-nunit-resharper-integration/

##Usage

The Tests project contains a set of three standard NBench test classes, attrributed to collect Measurements, and Assert some conditions.

The PerformanceTests.cs class contains a straightforward definition of an XUnit Test, attributed to act as a Theory, with a data source specified as a MemberData entry that uses the referenced NBenchTestHandler to discover test data from one of the aformentioned standard NBench Test Classes.

![PerformanceTests](PerformanceTests.PNG)

When executed using ReSharper, the Tests are displayed, with the result of NBench Assertions, and any Measurements reported by each Test.

![ResharperResults](ResharperResults.PNG)

Please note, the example includes a vairety of passing, and deliberately failing tests.

##Future plans

Release as NuGet package.

Possibly the creation of a custom TheoryAttribute, and implementation of IDataDiscoverer.

https://github.com/xunit/xunit/blob/master/src/xunit.core/TheoryAttribute.cs

