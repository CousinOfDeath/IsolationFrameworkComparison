using System;
using IsolationFrameWorkComparison.ExternalDependencies;
using IsolationFrameWorkComparison.Models;
using NSubstitute;
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
        
        [OneTimeSetUp]
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
            var localBusiness = new LocalBusiness
            {
                Id = Guid.NewGuid(),
                Name = "Whatever",
                Est = DateTime.UtcNow.AddYears(1)
            };

            // Act
            var exception = Assert.Throws<Exception>(() => this.main.HandleBusiness(localBusiness));

            // Assert
            StringAssert.Contains("can't set established date in the future", exception.Message);
        }

        [Test]
        public void HandleBusiness_ValidBusiness_CallsBusinessRepositoryAddBusiness()
        {
            // Arrange
            var localBusiness = new LocalBusiness
            {
                Id = Guid.NewGuid(),
                Name = "Whatever",
                Est = DateTime.UtcNow.AddYears(-1)
            };

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            this.businessRepositoryFake.Received().AddBusiness(localBusiness);
        }
        
        [Test]
        public void HandleBusiness_ValidBusiness_CallsLoggerLog()
        {
            // Arrange
            var localBusiness = new LocalBusiness
            {
                Id = Guid.NewGuid(),
                Name = "Whatever",
                Est = DateTime.UtcNow.AddYears(-1)
            };

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            this.loggerFake.Received().Log(Arg.Any<string>());
        }
    }
}