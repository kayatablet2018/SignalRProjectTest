using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using SignalR.EntityLayer.Concrete; // Varsayılan olarak EntityLayer projesinden Product modelini alır.
using SignalR.DataAccessLayer.Abstract; // Varsayılan olarak DataAccessLayer projesinden IRepository arayüzünü alır.
using SignalR.BusinessLayer.Abstract; // Varsayılan olarak BusinessLayer projesinden IProductService arayüzünü alır.
using SignalR.BusinessLayer.Concrete; // Varsayılan olarak BusinessLayer projesinden ProductManager sınıfını alır.

namespace SignalR.BusinessLayer.Tests.Services.Concrete
{
    public class TestBusinessServiceTests
    {
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly IProductService _productService;

        public TestBusinessServiceTests()
        {
            _mockProductRepository = new Mock<IRepository<Product>>();
            // ProductManager'ın IRepository<Product> bağımlılığını aldığı varsayılır.
            _productService = new ProductManager(_mockProductRepository.Object);
        }

        [Fact]
        public void GetById_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product { ProductID = productId, ProductName = "Test Product" };
            _mockProductRepository.Setup(repo => repo.GetByID(productId)).Returns(expectedProduct);

            // Act
            var result = _productService.TGetByID(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductID);
            Assert.Equal("Test Product", result.ProductName);
            _mockProductRepository.Verify(repo => repo.GetByID(productId), Times.Once);
        }

        [Fact]
        public void Add_AddsProductSuccessfully()
        {
            // Arrange
            var newProduct = new Product { ProductID = 2, ProductName = "New Product" };
            _mockProductRepository.Setup(repo => repo.Add(It.IsAny<Product>()));

            // Act
            _productService.TAdd(newProduct);

            // Assert
            _mockProductRepository.Verify(repo => repo.Add(newProduct), Times.Once);
        }

        [Fact]
        public void Update_UpdatesProductSuccessfully()
        {
            // Arrange
            var existingProduct = new Product { ProductID = 3, ProductName = "Existing Product" };
            _mockProductRepository.Setup(repo => repo.GetByID(3)).Returns(existingProduct); // Güncelleme öncesi mevcut ürünü döndür
            _mockProductRepository.Setup(repo => repo.Update(It.IsAny<Product>()));

            existingProduct.ProductName = "Updated Product"; // Ürünü güncelle

            // Act
            _productService.TUpdate(existingProduct);

            // Assert
            _mockProductRepository.Verify(repo => repo.Update(existingProduct), Times.Once);
        }

        [Fact]
        public void Delete_DeletesProductSuccessfully()
        {
            // Arrange
            var productIdToDelete = 4;
            var productToDelete = new Product { ProductID = productIdToDelete, ProductName = "Product to Delete" };
            _mockProductRepository.Setup(repo => repo.GetByID(productIdToDelete)).Returns(productToDelete);
            _mockProductRepository.Setup(repo => repo.Delete(It.IsAny<Product>()));

            // Act
            _productService.TDelete(productToDelete);

            // Assert
            _mockProductRepository.Verify(repo => repo.Delete(productToDelete), Times.Once);
        }

        [Fact]
        public void GetList_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1" },
                new Product { ProductID = 2, ProductName = "Product 2" }
            };
            _mockProductRepository.Setup(repo => repo.GetListAll()).Returns(products);

            // Act
            var result = _productService.TGetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.ProductName == "Product 1");
            _mockProductRepository.Verify(repo => repo.GetListAll(), Times.Once);
        }
    }
}