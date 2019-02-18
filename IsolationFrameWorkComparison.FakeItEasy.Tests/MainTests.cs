using System;
using FakeItEasy;
using IsolationFrameWorkComparison.ExternalDependencies;
using IsolationFrameWorkComparison.Models;
using NUnit.Framework;

namespace IsolationFrameWorkComparison.FakeItEasy
{
    [TestFixture]
    public class MainTests
    {
        private Main main;
        private IsolationFrameWorkComparison.ExternalDependencies.ILogger loggerFake;
        private INotificationService notificationServiceFake;
        private IBusinessRepository businessRepositoryFake;
        
        [SetUp]
        public void Setup()
        {
            loggerFake = A.Fake<IsolationFrameWorkComparison.ExternalDependencies.ILogger>();
            notificationServiceFake = A.Fake<INotificationService>();
            businessRepositoryFake = A.Fake<IBusinessRepository>();
            
            main = new Main(loggerFake, notificationServiceFake, businessRepositoryFake);
        }
        
        [Test]
        public void HandleBusiness_BusinessWithEstInFuture_ThrowsException()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(1);

            // Act
            var exception = Assert.Throws<Exception>(() => this.main.HandleBusiness(localBusiness));

            // Assert
            StringAssert.Contains("can't set established date in the future", exception.Message);
        }

        [Test]
        public void HandleBusiness_ValidBusiness_CallsBusinessRepositoryAddBusiness()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(-1);

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            A.CallTo(() => this.businessRepositoryFake.AddBusiness(localBusiness)).MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public void HandleBusiness_ValidBusiness_CallsLoggerLog()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(-1);

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            A.CallTo(() => this.loggerFake.Log(A<string>.That.Contains("Handled business for"))).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void IsBusinessGood_BusinessDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            A.CallTo(() => businessRepositoryFake.GetBusiness(A<Guid>.Ignored)).Returns(null);
            
            // Act
            var exception = Assert.Throws<UnauthorizedAccessException>(()=>  this.main.IsBusinessGood(Guid.NewGuid()));

            // Assert
            StringAssert.Contains("Unauthorized access", exception.Message);
        }
        
        [Test]
        public void IsBusinessGood_BusinessDoesNotExist_CallsNotificationServiceNotify()
        {
            // Arrange
            A.CallTo(() => businessRepositoryFake.GetBusiness(A<Guid>.Ignored)).Returns(null);
            
            // Act
            var exception = Assert.Throws<UnauthorizedAccessException>(()=>  this.main.IsBusinessGood(Guid.NewGuid()));

            // Assert
            A.CallTo(() => this.notificationServiceFake.Notify(A<Uri>.Ignored, A<string>.That.Contains("Could not find business"))).MustHaveHappenedOnceExactly();
        }
        
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-3)]
        [TestCase(-4)]
        [TestCase(-5)]
        public void IsBusinessGood_BusinessLessThanFiveOrFiveYearsOld_ReturnsFalse(int years)
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(years);
            
            A.CallTo(() => businessRepositoryFake.GetBusiness(A<Guid>.Ignored)).Returns(localBusiness);
            
            // Act
            var result = this.main.IsBusinessGood(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }
        
        [TestCase(-6)]
        [TestCase(-7)]
        public void IsBusinessGood_BusinessMoreThanFiveYearsOld_ReturnsFalse(int years)
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(years);
            
            A.CallTo(() => businessRepositoryFake.GetBusiness(A<Guid>.Ignored)).Returns(localBusiness);
            
            // Act
            var result = this.main.IsBusinessGood(Guid.NewGuid());

            // Assert
            Assert.IsTrue(result);
        }
        
        private static LocalBusiness MakeLocalBusiness(int years)
        {
            return new LocalBusiness
            {
                Id = Guid.NewGuid(),
                Name = "Whatever",
                Est = DateTime.UtcNow.AddYears(years)
            };
        }
        
    }
}