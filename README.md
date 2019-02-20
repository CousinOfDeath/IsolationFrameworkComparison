# IsolationFrameworkComparison

Very simple pseudo project with some fake external dependencies in order to test different Isolation Frameworks and their behavior. Unit tests follow the AAA (Arrange, Act, Assert) pattern and the test naming follows [UnitOfWork_StateUnderTest_ExpectedBehavior] convention (for more see Roy Osherove's blog http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html). As a general rule I tried to keep the tests readable, trustworthy and maintainable. Usually you need to do some tradeoffs between these factors and eventhough I have formed some kind of way to resolve these issues, I still find very often that my tests suck (at least the first iteration).

The Isolation Frameworks experimented are

1. NSubstitute
2. FakeItEasy
3. MOQ (Not done yet)
