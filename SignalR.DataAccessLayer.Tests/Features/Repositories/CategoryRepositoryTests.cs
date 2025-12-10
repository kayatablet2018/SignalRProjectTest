using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SignalR.DataAccessLayer.EntityFramework;
using SignalR.EntityLayer.Entities;
using SignalR.DataAccessLayer.Abstract; // ICategoryDal için ekliyoruz

namespace SignalR.DataAccessLayer.Tests.Features.Repositories
{
    public class CategoryRepositoryTests
    {
        private readonly Mock<SignalRContext> _mockContext;
        private readonly EfCategoryDal _categoryRepository;
        private readonly List<Category> _categoryData;

        public CategoryRepositoryTests()
        {
            // Test verilerini hazırla
            _categoryData = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Drinks", Status = true },
                new Category { CategoryID = 2, CategoryName = "Foods", Status = true },
                new Category { CategoryID = 3, CategoryName = "Desserts", Status = false },
                new Category { CategoryID = 4, CategoryName = "Breakfast", Status = true },
                new Category { CategoryID = 5, CategoryName = "Snacks", Status = false }
            };

            // DbSet mock'unu ayarla
            var mockSet = new Mock<DbSet<Category>>();
            mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(_categoryData.AsQueryable().Provider);
            mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(_categoryData.AsQueryable().Expression);
            mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(_categoryData.AsQueryable().ElementType);
            mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => _categoryData.GetEnumerator());

            // DbContext mock'unu ayarla
            _mockContext = new Mock<SignalRContext>();
            _mockContext.Setup(c => c.Categories).Returns(mockSet.Object);

            // Repository'yi mock'lanmış context ile başlat
            _categoryRepository = new EfCategoryDal(_mockContext.Object);
        }

        [Fact]
        public void GetActiveCategoryCount_ShouldReturnCorrectCount()
        {
            // Act
            var activeCount = _categoryRepository.GetActiveCategoryCount();

            // Assert
            Assert.Equal(3, activeCount); // _categoryData'dan beklenen 3 aktif kategori
        }

        [Fact]
        public void GetPassiveCategoryCount_ShouldReturnCorrectCount()
        {
            // Act
            var passiveCount = _categoryRepository.GetPassiveCategoryCount();

            // Assert
            Assert.Equal(2, passiveCount); // _categoryData'dan beklenen 2 pasif kategori
        }

        // Not: Temel CRUD operasyonları (Add, Delete, Update, GetById, GetListAll) 
        // GenericRepositoryTests.cs dosyasında test edilmelidir.
        // Bu dosya, Category entity'sine özel DAL metotlarını test etmeye odaklanmıştır.
    }
}