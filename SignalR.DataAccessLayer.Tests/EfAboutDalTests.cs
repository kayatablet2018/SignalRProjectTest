using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using SignalR.DataAccessLayer.Concrete.EntityFramework;
using SignalR.DataAccessLayer.Abstract;
using SignalR.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;

namespace SignalR.DataAccessLayer.Tests
{
    public class EfAboutDalTests
    {
        private readonly Mock<DbSet<About>> _mockDbSet;
        private readonly Mock<SignalRContext> _mockContext;
        private readonly EfAboutDal _efAboutDal;

        public EfAboutDalTests()
        {
            var abouts = new List<About>
            {
                new About { AboutID = 1, Title = "Title 1", Description = "Desc 1", ImageUrl = "Img1" },
                new About { AboutID = 2, Title = "Title 2", Description = "Desc 2", ImageUrl = "Img2" }
            }.AsQueryable();

            _mockDbSet = new Mock<DbSet<About>>();
            _mockDbSet.As<IQueryable<About>>().Setup(m => m.Provider).Returns(abouts.Provider);
            _mockDbSet.As<IQueryable<About>>().Setup(m => m.Expression).Returns(abouts.Expression);
            _mockDbSet.As<IQueryable<About>>().Setup(m => m.ElementType).Returns(abouts.ElementType);
            _mockDbSet.As<IQueryable<About>>().Setup(m => m.GetEnumerator()).Returns(abouts.GetEnumerator());

            _mockContext = new Mock<SignalRContext>();
            _mockContext.Setup(c => c.Set<About>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.Abouts).Returns(_mockDbSet.Object); // Assuming Abouts DbSet property exists

            _efAboutDal = new EfAboutDal(_mockContext.Object);
        }

        [Fact]
        public void Add_ShouldAddAboutEntity_WhenCalled()
        {
            // Arrange
            var newAbout = new About { AboutID = 3, Title = "Title 3", Description = "Desc 3", ImageUrl = "Img3" };

            // Act
            _efAboutDal.Add(newAbout);

            // Assert
            _mockDbSet.Verify(m => m.Add(It.IsAny<About>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Delete_ShouldRemoveAboutEntity_WhenCalled()
        {
            // Arrange
            var existingAbout = new About { AboutID = 1, Title = "Title 1", Description = "Desc 1", ImageUrl = "Img1" };

            // Act
            _efAboutDal.Delete(existingAbout);

            // Assert
            _mockDbSet.Verify(m => m.Remove(It.IsAny<About>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Update_ShouldUpdateAboutEntity_WhenCalled()
        {
            // Arrange
            var updatedAbout = new About { AboutID = 1, Title = "Updated Title", Description = "Updated Desc", ImageUrl = "Updated Img" };

            // Act
            _efAboutDal.Update(updatedAbout);

            // Assert
            _mockDbSet.Verify(m => m.Update(It.IsAny<About>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void GetById_ShouldReturnAboutEntity_WhenFound()
        {
            // Arrange
            var aboutId = 1;
            var expectedAbout = new About { AboutID = 1, Title = "Title 1", Description = "Desc 1", ImageUrl = "Img1" };

            _mockDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
                      .Returns((object[] ids) => _mockDbSet.Object.FirstOrDefault(x => x.AboutID == (int)ids[0]));

            // Act
            var result = _efAboutDal.GetById(aboutId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAbout.AboutID, result.AboutID);
            Assert.Equal(expectedAbout.Title, result.Title);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var aboutId = 99;
            _mockDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
                      .Returns((object[] ids) => _mockDbSet.Object.FirstOrDefault(x => x.AboutID == (int)ids[0]));

            // Act
            var result = _efAboutDal.GetById(aboutId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetListAll_ShouldReturnAllAboutEntities_WhenCalled()
        {
            // Act
            var result = _efAboutDal.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.AboutID == 1);
            Assert.Contains(result, a => a.AboutID == 2);
        }

        [Fact]
        public void GetListAll_Filter_ShouldReturnFilteredAboutEntities()
        {
            // Arrange
            Expression<Func<About, bool>> filter = a => a.Title.Contains("1");

            // Act
            var result = _efAboutDal.GetListAll(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Title 1", result.First().Title);
        }
    }
}
