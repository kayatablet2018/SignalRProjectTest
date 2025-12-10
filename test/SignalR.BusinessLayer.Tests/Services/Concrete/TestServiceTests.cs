using Xunit;
using Moq;
using SignalR.BusinessLayer.Abstract;
using SignalR.BusinessLayer.Concrete;
using SignalR.DataAccessLayer.Abstract;
using SignalR.EntityLayer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SignalR.BusinessLayer.Tests.Services.Concrete
{
    public class TestServiceTests
    {
        private readonly Mock<ITestDal> _mockTestDal;
        private readonly TestService _testService;

        public TestServiceTests()
        {
            _mockTestDal = new Mock<ITestDal>();
            _testService = new TestService(_mockTestDal.Object);
        }

        [Fact]
        public void TGetList_ShouldReturnAllItems()
        {
            // Arrange
            var expectedItems = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test1" }, new TestEntity { Id = 2, Name = "Test2" } };
            _mockTestDal.Setup(x => x.GetListAll()).Returns(expectedItems);

            // Act
            var actualItems = _testService.TGetListAll();

            // Assert
            Assert.NotNull(actualItems);
            Assert.Equal(expectedItems.Count, actualItems.Count);
            Assert.Equal(expectedItems.First().Name, actualItems.First().Name);
        }

        [Fact]
        public void TAdd_ShouldCallDalAddMethod()
        {
            // Arrange
            var newItem = new TestEntity { Id = 3, Name = "NewTest" };

            // Act
            _testService.TAdd(newItem);

            // Assert
            _mockTestDal.Verify(x => x.Add(newItem), Times.Once);
        }

        [Fact]
        public void TUpdate_ShouldCallDalUpdateMethod()
        {
            // Arrange
            var existingItem = new TestEntity { Id = 1, Name = "UpdatedTest" };

            // Act
            _testService.TUpdate(existingItem);

            // Assert
            _mockTestDal.Verify(x => x.Update(existingItem), Times.Once);
        }

        [Fact]
        public void TDelete_ShouldCallDalDeleteMethod()
        {
            // Arrange
            var itemToDelete = new TestEntity { Id = 1 };

            // Act
            _testService.TDelete(itemToDelete);

            // Assert
            _mockTestDal.Verify(x => x.Delete(itemToDelete), Times.Once);
        }

        // Not: Bu kısımda TestEntity, ITestDal ve TestService sınıflarının BusinessLayer projesinde
        // var olduğu ve mocklanabilir yapıda olduğu varsayılmıştır. Gerçek uygulamada bu yapıların
        // BusinessLayer ve DataAccessLayer projelerinde tanımlı olması gerekmektedir.
    }

    // Bu test için gerekli olan örnek arayüz ve somut sınıf tanımları (gerçek projeden alınacaktır)
    // Simülasyon amacıyla buraya eklenmiştir.

    // Örnek Test Entity
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Örnek Data Access Layer Arayüzü
    public interface ITestDal : IGenericDal<TestEntity>
    {
        // Özel metotlar buraya eklenebilir
    }

    // Örnek Business Layer Arayüzü
    public interface ITestService : IGenericService<TestEntity>
    {
        // Özel metotlar buraya eklenebilir
    }

    // Örnek Generic Data Access Layer Arayüzü (SignalR.DataAccessLayer.Abstract içinde olması beklenir)
    public interface IGenericDal<T>
        where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        List<T> GetListAll();
        T GetByID(int id);
    }

    // Örnek Generic Business Layer Arayüzü (SignalR.BusinessLayer.Abstract içinde olması beklenir)
    public interface IGenericService<T>
        where T : class
    {
        void TAdd(T entity);
        void TDelete(T entity);
        void TUpdate(T entity);
        List<T> TGetListAll();
        T TGetByID(int id);
    }

    // Örnek Business Layer Somut Sınıfı
    public class TestService : ITestService
    {
        private readonly ITestDal _testDal;

        public TestService(ITestDal testDal)
        {
            _testDal = testDal;
        }

        public void TAdd(TestEntity entity)
        {
            _testDal.Add(entity);
        }

        public void TDelete(TestEntity entity)
        {
            _testDal.Delete(entity);
        }

        public TestEntity TGetByID(int id)
        {
            return _testDal.GetByID(id);
        }

        public List<TestEntity> TGetListAll()
        {
            return _testDal.GetListAll();
        }

        public void TUpdate(TestEntity entity)
        {
            _testDal.Update(entity);
        }
    }
}
