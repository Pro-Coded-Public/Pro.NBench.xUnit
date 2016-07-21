# Pro.NBench.xUnit

Initial, simple approach to allow NBench tests to be discovered, executed, reported, and debugged using xUnit in ReSharper.

Heavily influenced by Andrea Angella's approach and article for integrating with NUnit.

http://www.andreaangella.com/2016/03/nbench-performance-testing-nunit-resharper-integration/

##Future plans

Release as NuGet package.

Possibly the creation of a custom TheoryAttribute, and implementation of IDataDiscoverer.

https://github.com/xunit/xunit/blob/master/src/xunit.core/TheoryAttribute.cs

