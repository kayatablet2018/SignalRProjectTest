using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using SignalR.DataAccessLayer.Abstract; // Assuming IProductDal and IGenericDal are here
using SignalR.EntityLayer.Entities; // Assuming Product entity is here

namespace SignalR.DataAccessLayer.Tests.Tests
{
    public class ExampleRepositoryTests
    {
        private readonly Mock<IProductDal> _mockProductDal;

        public ExampleRepositoryTests()
        {
            _mockProductDal = new Mock<IProductDal>();
        }

        [Fact]
        public void Add_Product_ShouldCallDalAddMethod()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product", Price = 10.0m, Status = true };

            // Act
            _mockProductDal.Object.Add(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Add(product), Times.Once);
        }

        [Fact]
        public void Update_Product_ShouldCallDalUpdateMethod()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Updated Product", Price = 12.0m, Status = true };

            // Act
            _mockProductDal.Object.Update(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Update(product), Times.Once);
        }

        [Fact]
        public void Delete_Product_ShouldCallDalDeleteMethod()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product", Price = 10.0m, Status = true };

            // Act
            _mockProductDal.Object.Delete(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Delete(product), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectProduct()
        {
            // Arrange
            var expectedProduct = new Product { ProductID = 1, ProductName = "Test Product", Price = 10.0m, Status = true };
            _mockProductDal.Setup(dal => dal.GetById(1)).Returns(expectedProduct);

            // Act
            var actualProduct = _mockProductDal.Object.GetById(1);

            // Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(expectedProduct.ProductID, actualProduct.ProductID);
            Assert.Equal(expectedProduct.ProductName, actualProduct.ProductName);
            _mockProductDal.Verify(dal => dal.GetById(1), Times.Once);
        }

        [Fact]
        public void GetList_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1", Price = 10.0m, Status = true },
                new Product { ProductID = 2, ProductName = "Product 2", Price = 20.0m, Status = true }
            };
            _mockProductDal.Setup(dal => dal.GetListAll()).Returns(products);

            // Act
            var result = _mockProductDal.Object.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockProductDal.Verify(dal => dal.GetListAll(), Times.Once);
        }
    }
}