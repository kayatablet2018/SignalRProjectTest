using Moq;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.Concrete;
using SignalR.DataAccessLayer.EntityFramework;
using SignalR.EntityLayer.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.DataAccessLayer.Tests.Features.Repositories
{
    // Test amaçlı kullanılacak örnek bir entity
    public class TestEntity : BaseEntity
    {
        public string Name { get; set; }
    }

    // BaseEntity, Id özelliği için tanımlanmalı veya EntityLayer'dan gelmeli
    // Eğer yoksa, test için burada tanımlanır.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    public class GenericRepositoryTests
    {
        private readonly Mock<SignalRContext> _mockContext;
        private readonly Mock<DbSet<TestEntity>> _mockDbSet;
        private readonly GenericRepository<TestEntity> _repository;
        private List<TestEntity> _data;

        public GenericRepositoryTests()
        {
            _mockContext = new Mock<SignalRContext>();
            _mockDbSet = new Mock<DbSet<TestEntity>>();
            _data = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "Entity 1" },
                new TestEntity { Id = 2, Name = "Entity 2" },
                new TestEntity { Id = 3, Name = "Entity 3" }
            };

            // DbSet'in IQueryable arayüzünü mock'lama
            var queryableData = _data.AsQueryable();

            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            // DbSet metotlarını mock'lama
            _mockDbSet.Setup(m => m.Add(It.IsAny<TestEntity>()))
                      .Callback<TestEntity>(entity => _data.Add(entity));
            _mockDbSet.Setup(m => m.Remove(It.IsAny<TestEntity>()))
                      .Callback<TestEntity>(entity => _data.Remove(entity));
            _mockDbSet.Setup(m => m.Update(It.IsAny<TestEntity>()))
                      .Callback<TestEntity>(entity =>
                      {
                          var existing = _data.FirstOrDefault(e => e.Id == entity.Id);
                          if (existing != null)
                          {
                              _data[_data.IndexOf(existing)] = entity;
                          }
                      });
            
            // Find metodu için mocklama. Genellikle ID ile bulur.
            // Eğer GenericRepository FindAsync kullanıyorsa, buna göre güncellenmeli.
            _mockDbSet.Setup(m => m.Find(It.IsAny<object[]>()))
                      .Returns((object[] ids) => _data.FirstOrDefault(e => e.Id == (int)ids[0]));

            _mockContext.Setup(c => c.Set<TestEntity>()).Returns(_mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1); // Bir değişiklik yapıldığını simüle et

            _repository = new GenericRepository<TestEntity>(_mockContext.Object);
        }

        [Fact]
        public void Add_ShouldAddNewEntityAndSaveChanges()
        {
            // Arrange
            var newEntity = new TestEntity { Id = 4, Name = "New Entity" };
            var initialCount = _data.Count;

            // Act
            _repository.Add(newEntity);

            // Assert
            Assert.Equal(initialCount + 1, _data.Count);
            Assert.Contains(newEntity, _data);
            _mockDbSet.Verify(m => m.Add(It.IsAny<TestEntity>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldRemoveEntityAndSaveChanges()
        {
            // Arrange
            var entityToDelete = _data.First();
            var initialCount = _data.Count;

            // Act
            _repository.Delete(entityToDelete);

            // Assert
            Assert.Equal(initialCount - 1, _data.Count);
            Assert.DoesNotContain(entityToDelete, _data);
            _mockDbSet.Verify(m => m.Remove(It.IsAny<TestEntity>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateExistingEntityAndSaveChanges()
        {
            // Arrange
            var entityToUpdate = _data.First();
            entityToUpdate.Name = "Updated Entity 1";

            // Act
            _repository.Update(entityToUpdate);

            // Assert
            var updatedEntity = _data.FirstOrDefault(e => e.Id == entityToUpdate.Id);
            Assert.NotNull(updatedEntity);
            Assert.Equal("Updated Entity 1", updatedEntity.Name);
            _mockDbSet.Verify(m => m.Update(It.IsAny<TestEntity>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectEntity()
        {
            // Arrange
            var expectedEntity = _data.First();

            // Act
            var result = _repository.GetById(expectedEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntity.Id, result.Id);
            Assert.Equal(expectedEntity.Name, result.Name);
            _mockDbSet.Verify(m => m.Find(It.IsAny<object[]>()), Times.Once());
        }

        [Fact]
        public void GetAll_ShouldReturnAllEntities()
        {
            // Arrange
            var expectedCount = _data.Count;

            // Act
            var result = _repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCount, result.Count);
            Assert.Equal(_data, result);
        }

        [Fact]
        public void GetById_ShouldReturnNullForNonExistingEntity()
        {
            // Arrange
            var nonExistingId = 99;

            // Act
            var result = _repository.GetById(nonExistingId);

            // Assert
            Assert.Null(result);
            _mockDbSet.Verify(m => m.Find(It.IsAny<object[]>()), Times.Once());
        }
    }
}