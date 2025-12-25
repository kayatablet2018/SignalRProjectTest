using Moq;
using SignalR.BusinessLayer.Abstract;
using SignalR.DtoLayer.ProductDto;
using SignalRApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRApi.Tests.Controllers
{
    public class TestControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly TestController _controller;

        public TestControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new TestController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<ResultProductDto>
            {
                new ResultProductDto { ProductID = 1, ProductName = "Test Product 1" },
                new ResultProductDto { ProductID = 2, ProductName = "Test Product 2" }
            };
            _mockProductService.Setup(s => s.TGetListAll()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<List<ResultProductDto>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count);
        }

        [Fact]
        public async Task GetProductById_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new ResultProductDto { ProductID = 1, ProductName = "Test Product" };
            _mockProductService.Setup(s => s.TGetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsAssignableFrom<ResultProductDto>(okResult.Value);
            Assert.Equal(1, returnedProduct.ProductID);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.TGetByIdAsync(99)).ReturnsAsync((ResultProductDto)null);

            // Act
            var result = await _controller.GetProductById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddProduct_ReturnsOkResult_OnSuccess()
        {
            // Arrange
            var createProductDto = new CreateProductDto { ProductName = "New Product" };
            _mockProductService.Setup(s => s.TAddAsync(It.IsAny<CreateProductDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddProduct(createProductDto);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockProductService.Verify(s => s.TAddAsync(It.IsAny<CreateProductDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOkResult_OnSuccess()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto { ProductID = 1, ProductName = "Updated Product" };
            _mockProductService.Setup(s => s.TUpdateAsync(It.IsAny<UpdateProductDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProduct(updateProductDto);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockProductService.Verify(s => s.TUpdateAsync(It.IsAny<UpdateProductDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsOkResult_OnSuccess()
        {
            // Arrange
            _mockProductService.Setup(s => s.TDeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockProductService.Verify(s => s.TDeleteAsync(1), Times.Once);
        }

        // Assume TestController exists in SignalRApi and uses IProductService
        // You might need to adjust the actual controller's signature and methods
        // to match these tests. This is a placeholder for demonstration.
        private class TestController : ControllerBase
        {
            private readonly IProductService _productService;

            public TestController(IProductService productService)
            {
                _productService = productService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAllProducts()
            {
                var products = await _productService.TGetListAll();
                return Ok(products);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetProductById(int id)
            {
                var product = await _productService.TGetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }

            [HttpPost]
            public async Task<IActionResult> AddProduct([FromBody] CreateProductDto createProductDto)
            {
                await _productService.TAddAsync(createProductDto);
                return Ok();
            }

            [HttpPut]
            public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto updateProductDto)
            {
                await _productService.TUpdateAsync(updateProductDto);
                return Ok();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProduct(int id)
            {
                await _productService.TDeleteAsync(id);
                return Ok();
            }
        }
    }
}
