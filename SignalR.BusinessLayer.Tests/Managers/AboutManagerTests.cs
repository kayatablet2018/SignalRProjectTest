using Xunit; 
using Moq; 
using SignalR.BusinessLayer.Concrete; 
using SignalR.DataAccessLayer.Abstract; 
using SignalR.EntityLayer.Entities; 
using System.Collections.Generic; 
using System.Linq; 
 
namespace SignalR.BusinessLayer.Tests.Managers 
{ 
    public class AboutManagerTests 
    { 
        private readonly Mock<IGenericDal<About>> _mockAboutDal; 
        private readonly AboutManager _aboutManager; 
 
        public AboutManagerTests() 
        { 
            _mockAboutDal = new Mock<IGenericDal<About>>(); 
            _aboutManager = new AboutManager(_mockAboutDal.Object); 
        } 
 
        [Fact] 
        public void TGetList_ShouldReturnAllAbouts() 
        { 
            // Arrange 
            var expectedAbouts = new List<About> 
            { 
                new About { AboutID = 1, Title = "Title 1", Description = "Description 1", ImageUrl = "url1" }, 
                new About { AboutID = 2, Title = "Title 2", Description = "Description 2", ImageUrl = "url2" } 
            }; 
            _mockAboutDal.Setup(dal => dal.TGetList()).Returns(expectedAbouts); 
 
            // Act 
            var result = _aboutManager.TGetList(); 
 
            // Assert 
            Assert.NotNull(result); 
            Assert.Equal(expectedAbouts.Count, result.Count); 
            Assert.Equal(expectedAbouts.First().Title, result.First().Title); 
            _mockAboutDal.Verify(dal => dal.TGetList(), Times.Once); 
        } 
 
        [Fact] 
        public void TAdd_ShouldCallDalAddMethod() 
        { 
            // Arrange 
            var newAbout = new About { AboutID = 3, Title = "New Title", Description = "New Description", ImageUrl = "newurl" }; 
 
            _mockAboutDal.Setup(dal => dal.TAdd(It.IsAny<About>())).Verifiable(); 
 
            // Act 
            _aboutManager.TAdd(newAbout); 
 
            // Assert 
            _mockAboutDal.Verify(dal => dal.TAdd(newAbout), Times.Once); 
        } 
 
        [Fact] 
        public void TGetByID_ShouldReturnCorrectAbout() 
        { 
            // Arrange 
            var expectedAbout = new About { AboutID = 1, Title = "Title 1", Description = "Description 1", ImageUrl = "url1" }; 
            _mockAboutDal.Setup(dal => dal.TGetByID(1)).Returns(expectedAbout); 
 
            // Act 
            var result = _aboutManager.TGetByID(1); 
 
            // Assert 
            Assert.NotNull(result); 
            Assert.Equal(expectedAbout.AboutID, result.AboutID); 
            Assert.Equal(expectedAbout.Title, result.Title); 
            _mockAboutDal.Verify(dal => dal.TGetByID(1), Times.Once); 
        } 
 
        [Fact] 
        public void TDelete_ShouldCallDalDeleteMethod() 
        { 
            // Arrange 
            var aboutToDelete = new About { AboutID = 1, Title = "Title 1", Description = "Description 1", ImageUrl = "url1" }; 
            _mockAboutDal.Setup(dal => dal.TDelete(It.IsAny<About>())).Verifiable(); 
 
            // Act 
            _aboutManager.TDelete(aboutToDelete); 
 
            // Assert 
            _mockAboutDal.Verify(dal => dal.TDelete(aboutToDelete), Times.Once); 
        } 
 
        [Fact] 
        public void TUpdate_ShouldCallDalUpdateMethod() 
        { 
            // Arrange 
            var aboutToUpdate = new About { AboutID = 1, Title = "Updated Title", Description = "Updated Description", ImageUrl = "updatedurl" }; 
            _mockAboutDal.Setup(dal => dal.TUpdate(It.IsAny<About>())).Verifiable(); 
 
            // Act 
            _aboutManager.TUpdate(aboutToUpdate); 
 
            // Assert 
            _mockAboutDal.Verify(dal => dal.TUpdate(aboutToUpdate), Times.Once); 
        } 
    } 
}