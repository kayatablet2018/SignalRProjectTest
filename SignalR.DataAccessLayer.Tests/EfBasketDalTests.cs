using Moq;
using SignalR.DataAccessLayer.Concrete;
using SignalR.EntityLayer.Entities;
using SignalR.DataAccessLayer.Abstract;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq.Expressions;

namespace SignalR.DataAccessLayer.Tests
{
    public class EfBasketDalTests
    {
        private readonly Mock<SignalRContext> _mockContext;
        private readonly Mock<DbSet<Basket>> _mockDbSet;
        private readonly EfBasketDal _efBasketDal;

        public EfBasketDalTests()
        {
            _mockContext = new Mock<SignalRContext>();
            _mockDbSet = new Mock<DbSet<Basket>>();
            _efBasketDal = new EfBasketDal(_mockContext.Object);

            // Setup DbSet as Queryable for methods like GetListAll
            var baskets = new List<Basket>
            {
                new Basket { BasketID = 1, ProductID = 101, Count = 2, Price = 25.0M, TotalPrice = 50.0M },
                new Basket { BasketID = 2, ProductID = 102, Count = 1, Price = 15.0M, TotalPrice = 15.0M }
            }.AsQueryable();

            _mockDbSet.As<IQueryable<Basket>>().Setup(m => m.Provider).Returns(baskets.Provider);
            _mockDbSet.As<IQueryable<Basket>>().Setup(m => m.Expression).Returns(baskets.Expression);
            _mockDbSet.As<IQueryable<Basket>>().Setup(m => m.ElementType).Returns(baskets.ElementType);
            _mockDbSet.As<IQueryable<Basket>>().Setup(m => m.GetEnumerator()).Returns(() => baskets.GetEnumerator());

            _mockContext.Setup(m => m.Set<Basket>()).Returns(_mockDbSet.Object);
        }

        [Fact]
        public void Add_ShouldAddBasketToContextAndSaveChanges()
        {
            // Arrange
            var newBasket = new Basket { BasketID = 3, ProductID = 103, Count = 3, Price = 10.0M, TotalPrice = 30.0M };

            // Act
            _efBasketDal.Add(newBasket);

            // Assert
            _mockDbSet.Verify(m => m.Add(It.IsAny<Basket>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldRemoveBasketFromContextAndSaveChanges()
        {
            // Arrange
            var existingBasket = new Basket { BasketID = 1, ProductID = 101, Count = 2, Price = 25.0M, TotalPrice = 50.0M };

            // Act
            _efBasketDal.Delete(existingBasket);

            // Assert
            _mockDbSet.Verify(m => m.Remove(It.IsAny<Basket>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Update_ShouldUpdateBasketInContextAndSaveChanges()
        {
            // Arrange
            var updatedBasket = new Basket { BasketID = 1, ProductID = 101, Count = 3, Price = 25.0M, TotalPrice = 75.0M };

            // Act
            _efBasketDal.Update(updatedBasket);

            // Assert
            _mockDbSet.Verify(m => m.Update(It.IsAny<Basket>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetByID_ShouldReturnCorrectBasket()
        {
            // Arrange
            var expectedBasket = new Basket { BasketID = 1, ProductID = 101, Count = 2, Price = 25.0M, TotalPrice = 50.0M };
            _mockDbSet.Setup(m => m.Find(1)).Returns(expectedBasket);

            // Act
            var result = _efBasketDal.GetByID(1);

            // Assert
            Assert.Equal(expectedBasket.BasketID, result.BasketID);
            Assert.Equal(expectedBasket.ProductID, result.ProductID);
        }

        [Fact]
        public void GetListAll_ShouldReturnAllBaskets()
        {
            // Arrange - Baskets are set up in the constructor

            // Act
            var result = _efBasketDal.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Based on initial mock data
        }

        [Fact]
        public void GetListAllByFilter_ShouldReturnFilteredBaskets()
        {
            // Arrange
            Expression<Func<Basket, bool>> filter = b => b.Count > 1;

            // Act
            var result = _efBasketDal.GetListAll(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Only the basket with Count = 2 should match
            Assert.Equal(1, result.First().BasketID);
        }

        [Fact]
        public void GetBasketByProductID_ShouldReturnCorrectBasketWhenExists()
        {
            // Arrange
            var expectedBasket = new Basket { BasketID = 1, ProductID = 101, Count = 2, Price = 25.0M, TotalPrice = 50.0M };
            _mockContext.Setup(c => c.Baskets.Where(It.IsAny<Expression<Func<Basket, bool>>>())).Returns(new List<Basket> { expectedBasket }.AsQueryable());

            // Act
            var result = _efBasketDal.GetBasketByProductID(101);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBasket.BasketID, result.BasketID);
            Assert.Equal(expectedBasket.ProductID, result.ProductID);
        }

        [Fact]
        public void GetBasketByProductID_ShouldReturnNullWhenNotExists()
        {
            // Arrange
            _mockContext.Setup(c => c.Baskets.Where(It.IsAny<Expression<Func<Basket, bool>>>())).Returns(new List<Basket>().AsQueryable());

            // Act
            var result = _efBasketDal.GetBasketByProductID(999);

            // Assert
            Assert.Null(result);
        }
    }
}