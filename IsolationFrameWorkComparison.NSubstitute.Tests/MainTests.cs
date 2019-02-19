using System;
using IsolationFrameWorkComparison.ExternalDependencies;
using IsolationFrameWorkComparison.Interfaces;
using IsolationFrameWorkComparison.Models;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace IsolationFrameWorkComparison.NSubstitute.Tests
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
            loggerFake = Substitute.For<IsolationFrameWorkComparison.ExternalDependencies.ILogger>();
            notificationServiceFake = Substitute.For<INotificationService>();
            businessRepositoryFake = Substitute.For<IBusinessRepository>();
            
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
        public void HandleBusiness_BusinessWithEstInFuture_DoesNotAddBusinessToRepository()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(1);

            // Act
            var exception = Assert.Throws<Exception>(() => this.main.HandleBusiness(localBusiness));

            // Assert
            this.businessRepositoryFake.DidNotReceive().AddBusiness(localBusiness);
        }
        
        [Test]
        public void HandleBusiness_BusinessWithEstInFuture_DoesNotWiteLog()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(1);

            // Act
            var exception = Assert.Throws<Exception>(() => this.main.HandleBusiness(localBusiness));

            // Assert
            this.loggerFake.DidNotReceive().Log(Arg.Is<string>(s => s.Contains("Handled business for")));
        }

        [Test]
        public void HandleBusiness_ValidBusiness_CallsBusinessRepositoryAddBusiness()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(-1);

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            this.businessRepositoryFake.Received().AddBusiness(localBusiness);
        }
        
        [Test]
        public void HandleBusiness_ValidBusiness_CallsLoggerLog()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(-1);

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            this.loggerFake.Received().Log(Arg.Is<string>(s => s.StartsWith("Handled business for")));
        }

        [Test]
        public void IsBusinessGood_BusinessDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            businessRepositoryFake.GetBusiness(Arg.Any<Guid>()).ReturnsNull();
            
            // Act
            var exception = Assert.Throws<UnauthorizedAccessException>(()=>  this.main.IsBusinessGood(Guid.NewGuid()));

            // Assert
            StringAssert.Contains("Unauthorized access", exception.Message);
        }
        
        [Test]
        public void IsBusinessGood_BusinessDoesNotExist_CallsNotificationServiceNotify()
        {
            // Arrange
            businessRepositoryFake.GetBusiness(Arg.Any<Guid>()).ReturnsNull();
            
            // Act
            var exception = Assert.Throws<UnauthorizedAccessException>(()=>  this.main.IsBusinessGood(Guid.NewGuid()));

            // Assert
            this.notificationServiceFake.Received().Notify(Arg.Any<Uri>(), "Could not find business");
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
            
            businessRepositoryFake.GetBusiness(Arg.Any<Guid>()).Returns(localBusiness);
            
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
            
            businessRepositoryFake.GetBusiness(Arg.Any<Guid>()).Returns(localBusiness);
            
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