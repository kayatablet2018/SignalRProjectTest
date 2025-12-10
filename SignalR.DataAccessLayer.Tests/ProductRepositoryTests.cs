using Moq;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.Concrete;
using SignalR.DataAccessLayer.EntityFramework;
using SignalR.EntityLayer.Entities;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SignalR.DataAccessLayer.Tests
{
    public class ProductRepositoryTests
    {
        private readonly Mock<SignalRContext> _mockContext;
        private readonly Mock<DbSet<Product>> _mockSet;
        private readonly EfProductRepository _productRepository;
        private List<Product> _products;

        public ProductRepositoryTests()
        {
            _mockContext = new Mock<SignalRContext>();
            _mockSet = new Mock<DbSet<Product>>();
            _products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Test Ürün 1", Description = "Açıklama 1", Price = 10.0m, ImageUrl = "url1", ProductStatus = true, CategoryID = 1, Category = new Category { CategoryID = 1, CategoryName = "Kategori 1" } },
                new Product { ProductID = 2, ProductName = "Test Ürün 2", Description = "Açıklama 2", Price = 20.0m, ImageUrl = "url2", ProductStatus = false, CategoryID = 2, Category = new Category { CategoryID = 2, CategoryName = "Kategori 2" } }
            };

            var queryableProducts = _products.AsQueryable();

            _mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(queryableProducts.Provider);
            _mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(queryableProducts.Expression);
            _mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(queryableProducts.ElementType);
            _mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(queryableProducts.GetEnumerator());

            // Set up Find method
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                    .Returns((object[] ids) => _products.FirstOrDefault(p => p.ProductID == (int)ids[0]));

            // Set up Add method
            _mockSet.Setup(m => m.Add(It.IsAny<Product>()))
                    .Callback<Product>(product => _products.Add(product));

            // Set up Remove method
            _mockSet.Setup(m => m.Remove(It.IsAny<Product>()))
                    .Callback<Product>(product => _products.Remove(product));

            _mockContext.Setup(c => c.Set<Product>()).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.Products).Returns(_mockSet.Object);

            _productRepository = new EfProductRepository(_mockContext.Object);
        }

        [Fact]
        public void Add_ShouldAddNewProduct_WhenProductIsValid()
        {
            // Arrange
            var newProduct = new Product { ProductID = 3, ProductName = "Test Ürün 3", Description = "Açıklama 3", Price = 30.0m, ImageUrl = "url3", ProductStatus = true, CategoryID = 1 };

            // Act
            _productRepository.Add(newProduct);

            // Assert
            Assert.Contains(newProduct, _products);
            Assert.Equal(3, _products.Count);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldRemoveProduct_WhenProductExists()
        {
            // Arrange
            var productToDelete = _products.First(); // ProductID = 1

            // Act
            _productRepository.Delete(productToDelete);

            // Assert
            Assert.DoesNotContain(productToDelete, _products);
            Assert.Equal(1, _products.Count);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Update_ShouldModifyExistingProduct_WhenProductExists()
        {
            // Arrange
            var productToUpdate = _products.First(); // ProductID = 1
            productToUpdate.ProductName = "Updated Product Name";

            // Act
            _productRepository.Update(productToUpdate);

            // Assert
            var updatedProduct = _products.FirstOrDefault(p => p.ProductID == 1);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product Name", updatedProduct.ProductName);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var expectedProduct = _products.First(); // ProductID = 1

            // Act
            var result = _productRepository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.ProductID, result.ProductID);
            Assert.Equal(expectedProduct.ProductName, result.ProductName);
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var result = _productRepository.GetById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProducts()
        {
            // Act
            var result = _productRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_products.Count, result.Count());
            Assert.True(result.Contains(_products[0]));
            Assert.True(result.Contains(_products[1]));
        }

        [Fact]
        public void GetProductsWithCategories_ShouldReturnProductsWithIncludedCategories()
        {
            // Arrange (mocking Include for demonstration, actual EF Core handles this internally)
            // For simpler in-memory list, we ensure the Category navigation property is populated

            // Act
            var result = _productRepository.GetProductsWithCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_products.Count, result.Count());
            foreach (var product in result)
            {
                Assert.NotNull(product.Category);
                Assert.False(string.IsNullOrEmpty(product.Category.CategoryName));
            }
        }
    }
}
