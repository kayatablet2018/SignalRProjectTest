using Microsoft.AspNetCore.Mvc;
using Moq;
using SignalR.BusinessLayer.Abstract;
using SignalR.DtoLayer.CategoryDto;
using SignalR.EntityLayer.Concrete;
using SignalRApi.Controllers;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRApi.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockCategoryService.Object);
        }

        [Fact]
        public async Task GetListAllCategory_ReturnsOkResult_WithListOfCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Burger", Status = true },
                new Category { CategoryID = 2, CategoryName = "Pizza", Status = true }
            };
            _mockCategoryService.Setup(s => s.TGetListAll()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetListAllCategory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCategories = Assert.IsType<List<ResultCategoryDto>>(okResult.Value);
            Assert.Equal(2, returnCategories.Count);
        }

        [Fact]
        public async Task GetByIDCategory_ReturnsOkResult_WithCategory()
        {
            // Arrange
            var category = new Category { CategoryID = 1, CategoryName = "Burger", Status = true };
            _mockCategoryService.Setup(s => s.TGetByID(1)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetByIDCategory(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCategory = Assert.IsType<ResultCategoryDto>(okResult.Value);
            Assert.Equal(1, returnCategory.CategoryID);
        }

        [Fact]
        public async Task GetByIDCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.TGetByID(99)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.GetByIDCategory(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ReturnsOkResult_WhenModelIsValid()
        {
            // Arrange
            var createDto = new CreateCategoryDto { CategoryName = "Dessert", Status = true };
            var category = new Category { CategoryName = "Dessert", Status = true };

            _mockCategoryService.Setup(s => s.TAdd(It.IsAny<Category>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateCategory(createDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCategoryService.Verify(s => s.TAdd(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsOkResult_WhenModelIsValid()
        {
            // Arrange
            var existingCategory = new Category { CategoryID = 1, CategoryName = "Burger", Status = true };
            var updateDto = new UpdateCategoryDto { CategoryID = 1, CategoryName = "Updated Burger", Status = false };

            _mockCategoryService.Setup(s => s.TGetByID(1)).ReturnsAsync(existingCategory);
            _mockCategoryService.Setup(s => s.TUpdate(It.IsAny<Category>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCategory(updateDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCategoryService.Verify(s => s.TUpdate(It.Is<Category>(c => c.CategoryID == 1 && c.CategoryName == "Updated Burger" && c.Status == false)), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateCategoryDto { CategoryID = 99, CategoryName = "NonExistent", Status = true };
            _mockCategoryService.Setup(s => s.TGetByID(99)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.UpdateCategory(updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCategoryService.Verify(s => s.TUpdate(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOkResult_WhenCategoryExists()
        {
            // Arrange
            var existingCategory = new Category { CategoryID = 1, CategoryName = "Burger", Status = true };
            _mockCategoryService.Setup(s => s.TGetByID(1)).ReturnsAsync(existingCategory);
            _mockCategoryService.Setup(s => s.TDelete(It.IsAny<Category>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCategory(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCategoryService.Verify(s => s.TDelete(It.Is<Category>(c => c.CategoryID == 1)), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.TGetByID(99)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.DeleteCategory(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCategoryService.Verify(s => s.TDelete(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryStatus_ReturnsOkResult_WhenCategoryExists()
        {
            // Arrange
            var existingCategory = new Category { CategoryID = 1, CategoryName = "Burger", Status = true };
            _mockCategoryService.Setup(s => s.TGetByID(1)).ReturnsAsync(existingCategory);
            _mockCategoryService.Setup(s => s.TChangeStatus(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangeCategoryStatus(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockCategoryService.Verify(s => s.TChangeStatus(1), Times.Once);
        }

        [Fact]
        public async Task ChangeCategoryStatus_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.TGetByID(99)).ReturnsAsync((Category)null);

            // Act
            var result = await _controller.ChangeCategoryStatus(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockCategoryService.Verify(s => s.TChangeStatus(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CategoryCount_ReturnsOkResult_WithCorrectCount()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.TCategoryCount()).ReturnsAsync(5);

            // Act
            var result = await _controller.CategoryCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5, okResult.Value);
        }

        [Fact]
        public async Task ActiveCategoryCount_ReturnsOkResult_WithCorrectCount()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.TActiveCategoryCount()).ReturnsAsync(3);

            // Act
            var result = await _controller.ActiveCategoryCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(3, okResult.Value);
        }

        [Fact]
        public async Task PassiveCategoryCount_ReturnsOkResult_WithCorrectCount()
        {
            // Arrange
            _mockCategoryService.Setup(s => s.TPassiveCategoryCount()).ReturnsAsync(2);

            // Act
            var result = await _controller.PassiveCategoryCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(2, okResult.Value);
        }
    }
}
