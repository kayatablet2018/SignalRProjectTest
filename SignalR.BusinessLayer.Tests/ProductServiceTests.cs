using Xunit;
using Moq;
using SignalR.BusinessLayer.Concrete;
using SignalR.DataAccessLayer.Abstract;
using SignalR.EntityLayer.Entities;
using System.Collections.Generic;

namespace SignalR.BusinessLayer.Tests
{
    // ProductServiceTests, ProductManager'ın IProductService arayüzünü uygulayan somut sınıf olduğu varsayılmıştır.
    public class ProductServiceTests
    {
        private readonly Mock<IProductDal> _mockProductDal;
        private readonly ProductManager _productManager;

        public ProductServiceTests()
        {
            _mockProductDal = new Mock<IProductDal>();
            // ProductManager'ın yapıcı metodunun sadece IProductDal aldığını varsayıyoruz.
            _productManager = new ProductManager(_mockProductDal.Object);
        }

        [Fact]
        public void TAdd_ShouldCallDalAddOnce()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product Add" };

            // Act
            _productManager.TAdd(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Add(product), Times.Once);
        }

        [Fact]
        public void TDelete_ShouldCallDalDeleteOnce()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product Delete" };

            // Act
            _productManager.TDelete(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Delete(product), Times.Once);
        }

        [Fact]
        public void TUpdate_ShouldCallDalUpdateOnce()
        {
            // Arrange
            var product = new Product { ProductID = 1, ProductName = "Test Product Update", Price = 10.5m };

            // Act
            _productManager.TUpdate(product);

            // Assert
            _mockProductDal.Verify(dal => dal.Update(product), Times.Once);
        }

        [Fact]
        public void TGetByID_ShouldReturnCorrectProduct()
        {
            // Arrange
            var expectedProduct = new Product { ProductID = 1, ProductName = "Test Product GetByID" };
            _mockProductDal.Setup(dal => dal.GetByID(1)).Returns(expectedProduct);

            // Act
            var result = _productManager.TGetByID(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.ProductID, result.ProductID);
            Assert.Equal(expectedProduct.ProductName, result.ProductName);
            _mockProductDal.Verify(dal => dal.GetByID(1), Times.Once);
        }

        [Fact]
        public void TGetListAll_ShouldReturnAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Product 1" },
                new Product { ProductID = 2, ProductName = "Product 2" }
            };
            _mockProductDal.Setup(dal => dal.GetListAll()).Returns(expectedProducts);

            // Act
            var result = _productManager.TGetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts.Count, result.Count);
            _mockProductDal.Verify(dal => dal.GetListAll(), Times.Once);
        }

        [Fact]
        public void TGetProductsWithCategories_ShouldReturnProductsWithTheirCategories()
        {
            // Arrange
            var productsWithCategories = new List<Product>
            {
                new Product { ProductID = 1, ProductName = "Hamburger", CategoryID = 1, Category = new Category { CategoryID = 1, CategoryName = "Fast Food" } },
                new Product { ProductID = 2, ProductName = "Cola", CategoryID = 1, Category = new Category { CategoryID = 1, CategoryName = "Fast Food" } }
            };
            // IProductDal arayüzünde GetProductsWithCategories metodunun olduğunu varsayıyoruz.
            _mockProductDal.Setup(dal => dal.GetProductsWithCategories()).Returns(productsWithCategories);

            // Act
            var result = _productManager.TGetProductsWithCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productsWithCategories.Count, result.Count);
            Assert.Contains(result, p => p.ProductName == "Hamburger" && p.Category.CategoryName == "Fast Food");
            _mockProductDal.Verify(dal => dal.GetProductsWithCategories(), Times.Once);
        }
    }
}
