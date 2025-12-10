using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using SignalR.DataAccessLayer.Abstract;
using SignalR.DataAccessLayer.Concrete;
using SignalR.DataAccessLayer.EntityFramework;
using SignalR.EntityLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.DataAccessLayer.Tests
{
    public class CategoryRepositoryTests
    {
        private readonly Mock<SignalRContext> _mockContext;
        private readonly CategoryRepository _categoryRepository;
        private readonly List<Category> _categories;

        public CategoryRepositoryTests()
        {
            // Test verilerini başlat
            _categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "İçecekler", Status = true },
                new Category { CategoryID = 2, CategoryName = "Ana Yemekler", Status = true },
                new Category { CategoryID = 3, CategoryName = "Tatlılar", Status = false }
            };

            // DbSet'i Mock'la
            var mockDbSet = new Mock<DbSet<Category>>();
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(_categories.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(_categories.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(_categories.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => _categories.GetEnumerator());

            // Ekleme, Kaldırma, Güncelleme işlemlerini simüle et
            mockDbSet.Setup(m => m.Add(It.IsAny<Category>())).Callback<Category>((category) =>
            {
                category.CategoryID = _categories.Any() ? _categories.Max(c => c.CategoryID) + 1 : 1; // Otomatik artışı simüle et
                _categories.Add(category);
            });
            mockDbSet.Setup(m => m.Remove(It.IsAny<Category>())).Callback<Category>((category) => _categories.Remove(category));
            mockDbSet.Setup(m => m.Update(It.IsAny<Category>())).Callback<Category>((category) =>
            {
                var existingCategory = _categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);
                if (existingCategory != null)
                {
                    existingCategory.CategoryName = category.CategoryName;
                    existingCategory.Status = category.Status;
                }
            });

            // DbContext'i Mock'la
            _mockContext = new Mock<SignalRContext>();
            _mockContext.Setup(c => c.Set<Category>()).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1); // Başarılı kaydetmeyi simüle et

            // Repository'yi mock'lanmış context ile başlat
            _categoryRepository = new CategoryRepository(_mockContext.Object);
        }

        [Fact]
        public void Add_ShouldAddCategoryToDatabase()
        {
            // Arrange
            var newCategory = new Category { CategoryName = "Çorbalar", Status = true };
            var initialCount = _categories.Count;

            // Act
            _categoryRepository.Add(newCategory);
            _mockContext.Object.SaveChanges(); // Değişiklikleri kaydetmeyi simüle et

            // Assert
            Assert.Equal(initialCount + 1, _categories.Count);
            Assert.Contains(_categories, c => c.CategoryName == "Çorbalar");
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldRemoveCategoryFromDatabase()
        {
            // Arrange
            var categoryToDelete = _categories.First();
            var initialCount = _categories.Count;

            // Act
            _categoryRepository.Delete(categoryToDelete);
            _mockContext.Object.SaveChanges(); // Değişiklikleri kaydetmeyi simüle et

            // Assert
            Assert.Equal(initialCount - 1, _categories.Count);
            Assert.DoesNotContain(_categories, c => c.CategoryID == categoryToDelete.CategoryID);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectCategory()
        {
            // Arrange
            var expectedCategory = _categories.First();

            // Act
            var result = _categoryRepository.GetById(expectedCategory.CategoryID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.CategoryID, result.CategoryID);
            Assert.Equal(expectedCategory.CategoryName, result.CategoryName);
        }

        [Fact]
        public void GetListAll_ShouldReturnAllCategories()
        {
            // Act
            var result = _categoryRepository.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_categories.Count, result.Count());
            Assert.Equal(_categories.Select(c => c.CategoryName).OrderBy(n => n), result.Select(c => c.CategoryName).OrderBy(n => n));
        }

        [Fact]
        public void Update_ShouldUpdateExistingCategory()
        {
            // Arrange
            var categoryToUpdate = _categories.First();
            var originalName = categoryToUpdate.CategoryName;
            categoryToUpdate.CategoryName = "Güncellenmiş İçecekler";
            categoryToUpdate.Status = false;

            // Act
            _categoryRepository.Update(categoryToUpdate);
            _mockContext.Object.SaveChanges(); // Değişiklikleri kaydetmeyi simüle et

            // Assert
            var updatedCategory = _categories.FirstOrDefault(c => c.CategoryID == categoryToUpdate.CategoryID);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Güncellenmiş İçecekler", updatedCategory.CategoryName);
            Assert.False(updatedCategory.Status);
            Assert.NotEqual(originalName, updatedCategory.CategoryName);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnNullWhenCategoryNotFound()
        {
            // Act
            var result = _categoryRepository.GetById(999); // Mevcut olmayan bir ID

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetListAll_ShouldReturnEmptyListWhenNoCategories()
        {
            // Arrange
            _categories.Clear(); // In-memory listeyi temizle
            // Boş listeyi yansıtacak şekilde mockDbSet'i yeniden ayarla
            var mockEmptyDbSet = new Mock<DbSet<Category>>();
            mockEmptyDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(_categories.AsQueryable().Provider);
            mockEmptyDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(_categories.AsQueryable().Expression);
            mockEmptyDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(_categories.AsQueryable().ElementType);
            mockEmptyDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => _categories.GetEnumerator());

            _mockContext.Setup(c => c.Set<Category>()).Returns(mockEmptyDbSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(mockEmptyDbSet.Object);

            // Act
            var result = _categoryRepository.GetListAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}