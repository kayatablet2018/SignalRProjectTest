using Moq;
using SignalR.BusinessLayer.Abstract;
using SignalR.BusinessLayer.Concrete;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.UnitOfWork;
using SignalR.EntityLayer.Entities;
using Xunit;

namespace SignalR.BusinessLayer.Tests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryDal> _mockCategoryRepository;
        private readonly Mock<IUnitOfWorkDal> _mockUnitOfWork;
        private readonly ICategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryDal>();
            _mockUnitOfWork = new Mock<IUnitOfWorkDal>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public void AddCategory_ShouldCallRepositoryAddAndUnitOfWorkSave()
        {
            // Arrange
            var category = new Category { CategoryName = "Test Category", Status = true };

            // Act
            _categoryService.TAdd(category);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.Add(category), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void DeleteCategory_ShouldCallRepositoryDeleteAndUnitOfWorkSave()
        {
            // Arrange
            var category = new Category { CategoryID = 1, CategoryName = "Test Category", Status = true };

            // Act
            _categoryService.TDelete(category);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.Delete(category), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void UpdateCategory_ShouldCallRepositoryUpdateAndUnitOfWorkSave()
        {
            // Arrange
            var category = new Category { CategoryID = 1, CategoryName = "Updated Category", Status = false };

            // Act
            _categoryService.TUpdate(category);

            // Assert
            _mockCategoryRepository.Verify(repo => repo.Update(category), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnCategory()
        {
            // Arrange
            var categoryId = 1;
            var expectedCategory = new Category { CategoryID = categoryId, CategoryName = "Category 1", Status = true };
            _mockCategoryRepository.Setup(repo => repo.GetById(categoryId)).Returns(expectedCategory);

            // Act
            var result = _categoryService.TGetById(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.CategoryID, result.CategoryID);
            Assert.Equal(expectedCategory.CategoryName, result.CategoryName);
            _mockCategoryRepository.Verify(repo => repo.GetById(categoryId), Times.Once);
        }

        [Fact]
        public void GetList_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Category 1", Status = true },
                new Category { CategoryID = 2, CategoryName = "Category 2", Status = false }
            };
            _mockCategoryRepository.Setup(repo => repo.GetListAll()).Returns(categories);

            // Act
            var result = _categoryService.TGetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CategoryName == "Category 1");
            _mockCategoryRepository.Verify(repo => repo.GetListAll(), Times.Once);
        }
    }
}
