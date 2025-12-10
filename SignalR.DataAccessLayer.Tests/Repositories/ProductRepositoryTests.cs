using Moq;
using SignalR.DataAccessLayer.Abstract;
using SignalR.EntityLayer.Entities;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.DataAccessLayer.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;

        public ProductRepositoryTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsListOfProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Burger", Price = 10.0m },
                new Product { ProductID = 2, ProductName = "Pizza", Price = 15.0m }
            };
            _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedProducts);

            // Act
            var actualProducts = await _mockProductRepository.Object.GetAllAsync();

            // Assert
            Assert.NotNull(actualProducts);
            Assert.Equal(expectedProducts.Count, actualProducts.Count);
            Assert.Equal(expectedProducts.First().ProductName, actualProducts.First().ProductName);
            _mockProductRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductById_ReturnsCorrectProduct_WhenProductExists()
        {
            // Arrange
            var expectedProduct = new Product { ProductID = 1, ProductName = "Burger", Price = 10.0m };
            _mockProductRepository.Setup(repo => repo.GetByIDAsync(1)).ReturnsAsync(expectedProduct);

            // Act
            var actualProduct = await _mockProductRepository.Object.GetByIDAsync(1);

            // Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(expectedProduct.ProductID, actualProduct.ProductID);
            Assert.Equal(expectedProduct.ProductName, actualProduct.ProductName);
            _mockProductRepository.Verify(repo => repo.GetByIDAsync(1), Times.Once);
        }

        [Fact]
        public async Task AddProduct_CallsAddMethodOnce()
        {
            // Arrange
            var newProduct = new Product { ProductID = 3, ProductName = "Fries", Price = 5.0m };
            _mockProductRepository.Setup(repo => repo.AddAsync(newProduct)).Returns(Task.CompletedTask);

            // Act
            await _mockProductRepository.Object.AddAsync(newProduct);

            // Assert
            _mockProductRepository.Verify(repo => repo.AddAsync(newProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_CallsUpdateMethodOnce()
        {
            // Arrange
            var existingProduct = new Product { ProductID = 1, ProductName = "Burger", Price = 10.0m };
            _mockProductRepository.Setup(repo => repo.UpdateAsync(existingProduct)).Returns(Task.CompletedTask);

            // Act
            await _mockProductRepository.Object.UpdateAsync(existingProduct);

            // Assert
            _mockProductRepository.Verify(repo => repo.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task RemoveProduct_CallsRemoveMethodOnce()
        {
            // Arrange
            var productToRemove = new Product { ProductID = 1, ProductName = "Burger", Price = 10.0m };
            _mockProductRepository.Setup(repo => repo.RemoveAsync(productToRemove)).Returns(Task.CompletedTask);

            // Act
            await _mockProductRepository.Object.RemoveAsync(productToRemove);

            // Assert
            _mockProductRepository.Verify(repo => repo.RemoveAsync(productToRemove), Times.Once);
        }
    }
}