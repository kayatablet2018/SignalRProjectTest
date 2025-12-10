using Moq;
using SignalR.BusinessLayer.Abstract;
using SignalR.BusinessLayer.Concrete;
using SignalR.DataAccessLayer.Abstract;
using SignalR.EntityLayer.Concrete;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.BusinessLayer.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryDal> _mockCategoryDal;
        private readonly ICategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryDal = new Mock<ICategoryDal>();
            _categoryService = new CategoryManager(_mockCategoryDal.Object);
        }

        [Fact]
        public async Task T1_AddCategory_ShouldCallDalAdd()
        {
            // Arrange
            var category = new Category { CategoryID = 1, CategoryName = "Test Category", Status = true };

            // Act
            _categoryService.TAdd(category);

            // Assert
            _mockCategoryDal.Verify(dal => dal.Add(category), Times.Once);
        }

        [Fact]
        public async Task T2_DeleteCategory_ShouldCallDalDelete()
        {
            // Arrange
            var category = new Category { CategoryID = 1, CategoryName = "Test Category", Status = true };

            // Act
            _categoryService.TDelete(category);

            // Assert
            _mockCategoryDal.Verify(dal => dal.Delete(category), Times.Once);
        }

        [Fact]
        public async Task T3_UpdateCategory_ShouldCallDalUpdate()
        {
            // Arrange
            var category = new Category { CategoryID = 1, CategoryName = "Updated Category", Status = false };

            // Act
            _categoryService.TUpdate(category);

            // Assert
            _mockCategoryDal.Verify(dal => dal.Update(category), Times.Once);
        }

        [Fact]
        public async Task T4_GetByIdCategory_ShouldReturnCategory()
        {
            // Arrange
            var categoryId = 1;
            var expectedCategory = new Category { CategoryID = categoryId, CategoryName = "Drinks", Status = true };
            _mockCategoryDal.Setup(dal => dal.GetByID(categoryId)).Returns(expectedCategory);

            // Act
            var result = _categoryService.TGetByID(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.CategoryName, result.CategoryName);
            _mockCategoryDal.Verify(dal => dal.GetByID(categoryId), Times.Once);
        }

        [Fact]
        public async Task T5_GetListAllCategories_ShouldReturnListOfCategories()
        {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Food", Status = true },
                new Category { CategoryID = 2, CategoryName = "Desserts", Status = true }
            };
            _mockCategoryDal.Setup(dal => dal.GetListAll()).Returns(expectedCategories);

            // Act
            var result = _categoryService.TGetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategories.Count, result.Count());
            _mockCategoryDal.Verify(dal => dal.GetListAll(), Times.Once);
        }
    }
}