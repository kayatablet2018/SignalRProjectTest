using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using SignalR.EntityLayer.Entities;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.Repositories.Concrete;

namespace SignalR.DataAccessLayer.Tests.Repositories.Concrete
{
    public class TestEntity : BaseEntity // Assuming a BaseEntity exists or creating a simple one
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Bu test sınıfı, SignalR.DataAccessLayer'daki repository'ler için örnek bir birim testidir.
    // Gerçek uygulamada, ilgili Entity ve IRepository arayüzleri kullanılmalıdır.
    public class TestRepositoryTests
    {
        private readonly Mock<IGenericDal<TestEntity>> _mockRepository;

        public TestRepositoryTests()
        {
            _mockRepository = new Mock<IGenericDal<TestEntity>>();
        }

        [Fact]
        public void GetAll_ReturnsAllEntities()
        {
            // Arrange
            var testEntities = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Test Entity 1" },
                new TestEntity { Id = 2, Name = "Test Entity 2" }
            };
            _mockRepository.Setup(repo => repo.GetListAll()).Returns(testEntities);

            // Act
            var result = _mockRepository.Object.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, e => e.Name == "Test Entity 1");
            Assert.Contains(result, e => e.Name == "Test Entity 2");
        }

        [Fact]
        public void GetById_ReturnsCorrectEntity()
        {
            // Arrange
            var testEntity = new TestEntity { Id = 1, Name = "Test Entity 1" };
            _mockRepository.Setup(repo => repo.GetByID(1)).Returns(testEntity);

            // Act
            var result = _mockRepository.Object.GetByID(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Entity 1", result.Name);
        }

        [Fact]
        public void Add_CallsInsertMethod()
        {
            // Arrange
            var newEntity = new TestEntity { Id = 3, Name = "New Test Entity" };

            // Act
            _mockRepository.Object.Insert(newEntity);

            // Assert
            _mockRepository.Verify(repo => repo.Insert(newEntity), Times.Once());
        }

        [Fact]
        public void Update_CallsUpdateMethod()
        {
            // Arrange
            var existingEntity = new TestEntity { Id = 1, Name = "Updated Entity" };

            // Act
            _mockRepository.Object.Update(existingEntity);

            // Assert
            _mockRepository.Verify(repo => repo.Update(existingEntity), Times.Once());
        }

        [Fact]
        public void Delete_CallsDeleteMethod()
        {
            // Arrange
            var entityToDelete = new TestEntity { Id = 1, Name = "Entity To Delete" };

            // Act
            _mockRepository.Object.Delete(entityToDelete);

            // Assert
            _mockRepository.Verify(repo => repo.Delete(entityToDelete), Times.Once());
        }
    }
}
