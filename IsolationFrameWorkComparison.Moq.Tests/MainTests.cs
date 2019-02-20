using System;
using IsolationFrameWorkComparison.ExternalDependencies;
using IsolationFrameWorkComparison.Interfaces;
using IsolationFrameWorkComparison.Models;
using Moq;
using NUnit.Framework;

namespace IsolationFrameWorkComparison.Moq.Tests
{
[TestFixture]
    public class MainTests
    {
        private Main main;
        private Mock<ILogger> loggerFake;
        private Mock<INotificationService> notificationServiceFake;
        private Mock<IBusinessRepository> businessRepositoryFake;
        
        [SetUp]
        public void Setup()
        {
            loggerFake = new Mock<ILogger>();
            notificationServiceFake = new Mock<INotificationService>();
            businessRepositoryFake = new Mock<IBusinessRepository>();
            
            main = new Main(loggerFake.Object, notificationServiceFake.Object, businessRepositoryFake.Object);
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
            this.businessRepositoryFake.Verify(b => b.AddBusiness(localBusiness), Times.Never);
        }
        
        [Test]
        public void HandleBusiness_BusinessWithEstInFuture_DoesNotWiteLog()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(1);

            // Act
            var exception = Assert.Throws<Exception>(() => this.main.HandleBusiness(localBusiness));

            // Assert
            this.loggerFake.Verify(l => l.Log(It.Is<string>(s => s.Contains("Handled business for"))), Times.Never);
        }

        [Test]
        public void HandleBusiness_ValidBusiness_CallsBusinessRepositoryAddBusiness()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(-1);

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            this.businessRepositoryFake.Verify(b => b.AddBusiness(localBusiness), Times.Once);
        }
        
        [Test]
        public void HandleBusiness_ValidBusiness_CallsLoggerLog()
        {
            // Arrange
            var localBusiness = MakeLocalBusiness(-1);

            // Act
            this.main.HandleBusiness(localBusiness);

            // Assert
            this.loggerFake.Verify(l => l.Log(It.Is<string>(s => s.Contains("Handled business for"))), Times.Once);
        }

        [Test]
        public void IsBusinessGood_BusinessDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            businessRepositoryFake.Setup(b => b.GetBusiness(It.IsAny<Guid>())).Returns((IBusiness)null);
     
            
            // Act
            var exception = Assert.Throws<UnauthorizedAccessException>(()=>  this.main.IsBusinessGood(Guid.NewGuid()));

            // Assert
            StringAssert.Contains("Unauthorized access", exception.Message);
        }
        
        [Test]
        public void IsBusinessGood_BusinessDoesNotExist_CallsNotificationServiceNotify()
        {
            // Arrange
            businessRepositoryFake.Setup(b => b.GetBusiness(It.IsAny<Guid>())).Returns((IBusiness)null);
            
            // Act
            var exception = Assert.Throws<UnauthorizedAccessException>(()=>  this.main.IsBusinessGood(Guid.NewGuid()));

            // Assert
            this.notificationServiceFake.Verify(n => n.Notify(It.IsAny<Uri>(), It.Is<string>(s => s.Contains("Could not find business"))));
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
            
            businessRepositoryFake.Setup(b => b.GetBusiness(It.IsAny<Guid>())).Returns(localBusiness);
            
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
            
            businessRepositoryFake.Setup(b => b.GetBusiness(It.IsAny<Guid>())).Returns(localBusiness);
            
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