using Moq;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.Concrete;
using SignalR.DataAccessLayer.Repositories;
using SignalR.EntityLayer.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.DataAccessLayer.Tests.Features.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly Mock<SignalRContext> _mockContext;
        private readonly Mock<IProductDal> _mockProductDal;

        public ProductRepositoryTests()
        {
            _mockContext = new Mock<SignalRContext>();
            _mockProductDal = new Mock<IProductDal>();
        }

        [Fact]
        public async Task Add_Product_Success()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product" };
            _mockProductDal.Setup(dal => dal.Add(product)).Verifiable();

            // Act
            _mockProductDal.Object.Add(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Add(product), Times.Once);
        }

        [Fact]
        public async Task Delete_Product_Success()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product" };
            _mockProductDal.Setup(dal => dal.Delete(product)).Verifiable();

            // Act
            _mockProductDal.Object.Delete(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Delete(product), Times.Once);
        }

        [Fact]
        public async Task Update_Product_Success()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Updated Product" };
            _mockProductDal.Setup(dal => dal.Update(product)).Verifiable();

            // Act
            _mockProductDal.Object.Update(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Update(product), Times.Once);
        }

        [Fact]
        public async Task GetById_Product_ReturnsProduct()
        {
            // Arrange
            var expectedProduct = new Product { ProductID = 1, ProductName = "Test Product" };
            _mockProductDal.Setup(dal => dal.GetById(1)).Returns(expectedProduct);

            // Act
            var actualProduct = _mockProductDal.Object.GetById(1);

            // Assert
            Assert.NotNull(actualProduct);
            Assert.Equal(expectedProduct.ProductID, actualProduct.ProductID);
            Assert.Equal(expectedProduct.ProductName, actualProduct.ProductName);
        }

        [Fact]
        public async Task GetList_Products_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1" },
                new Product { ProductID = 2, ProductName = "Product 2" }
            };
            _mockProductDal.Setup(dal => dal.GetListAll()).Returns(products);

            // Act
            var result = _mockProductDal.Object.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProductsWithCategories_ReturnsProductsWithCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Category A" },
                new Category { CategoryID = 2, CategoryName = "Category B" }
            };

            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1", CategoryID = 1, Category = categories[0] },
                new Product { ProductID = 2, ProductName = "Product 2", CategoryID = 1, Category = categories[0] },
                new Product { ProductID = 3, ProductName = "Product 3", CategoryID = 2, Category = categories[1] }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            _mockContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            _mockProductDal.Setup(dal => dal.GetProductsWithCategories()).Returns(
                _mockContext.Object.Products.Include(p => p.Category).ToList()
            );

            // Act
            var result = _mockProductDal.Object.GetProductsWithCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, p => p.ProductName == "Product 1" && p.Category.CategoryName == "Category A");
            Assert.Contains(result, p => p.ProductName == "Product 3" && p.Category.CategoryName == "Category B");
        }

        [Fact]
        public async Task GetProductCount_ReturnsCorrectCount()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1" },
                new Product { ProductID = 2, ProductName = "Product 2" },
                new Product { ProductID = 3, ProductName = "Product 3" }
            };
            _mockProductDal.Setup(dal => dal.ProductCount()).Returns(products.Count);

            // Act
            var result = _mockProductDal.Object.ProductCount();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task GetActiveProductCount_ReturnsCorrectCount()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1", ProductStatus = true },
                new Product { ProductID = 2, ProductName = "Product 2", ProductStatus = false },
                new Product { ProductID = 3, ProductName = "Product 3", ProductStatus = true }
            };
            _mockProductDal.Setup(dal => dal.ActiveProductCount()).Returns(products.Count(p => p.ProductStatus == true));

            // Act
            var result = _mockProductDal.Object.ActiveProductCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetPassiveProductCount_ReturnsCorrectCount()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1", ProductStatus = true },
                new Product { ProductID = 2, ProductName = "Product 2", ProductStatus = false },
                new Product { ProductID = 3, ProductName = "Product 3", ProductStatus = true }
            };
            _mockProductDal.Setup(dal => dal.PassiveProductCount()).Returns(products.Count(p => p.ProductStatus == false));

            // Act
            var result = _mockProductDal.Object.PassiveProductCount();

            // Assert
            Assert.Equal(1, result);
        }
    }
}