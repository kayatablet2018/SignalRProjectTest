using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using SignalR.DataAccessLayer.Abstract; // IGenericDal interface'inin burada olduğunu varsayıyoruz
using SignalR.DataAccessLayer.Concrete; // SignalRContext ve GenericRepository sınıflarının burada olduğunu varsayıyoruz
using SignalR.EntityLayer.Entities; // Category varlığının burada olduğunu varsayıyoruz
using System.Linq;
using System.Collections.Generic;

namespace SignalR.DataAccessLayer.Tests
{
    public class GenericRepositoryTests
    {
        // DbContext'in public parametresiz bir yapıcısı olduğunu veya Moq'un bir tane oluşturabileceğini varsayarak
        // somut DbContext'i mockluyoruz.
        // Eğer DbContextOptions alıyorsa, daha karmaşık bir kurulum veya bellek içi bir veritabanı
        // gerçek bir test için gerekli olabilir. Saf birim testi için Moq ile bağımlılıkları doğrudan mocklarız.
        private readonly Mock<SignalRContext> _mockContext;
        private readonly Mock<DbSet<Category>> _mockSet;
        private readonly GenericRepository<Category> _repository;
        private List<Category> _categories; // DbSet için bellek içi verileri simüle ediyoruz

        public GenericRepositoryTests()
        {
            // Category varlıkları için veritabanı tablosunu simüle etmek üzere bir liste başlatıyoruz.
            _categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Kategori 1", Status = true },
                new Category { CategoryID = 2, CategoryName = "Kategori 2", Status = true },
                new Category { CategoryID = 3, CategoryName = "Kategori 3", Status = false }
            };

            // DbSet ve DbContext için mocklar oluşturuyoruz
            _mockSet = new Mock<DbSet<Category>>();
            _mockContext = new Mock<SignalRContext>();

            // GetList tarafından kullanılan LINQ işlemlerine izin vermek için DbSet mock'u için IQueryable ayarlıyoruz.
            var queryableCategories = _categories.AsQueryable();
            _mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(queryableCategories.Provider);
            _mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(queryableCategories.Expression);
            _mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(queryableCategories.ElementType);
            _mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => queryableCategories.GetEnumerator());

            // Set<Category>() çağrıldığında mocklanmış DbSet'imizi döndürmesi için context'i ayarlıyoruz.
            _mockContext.Setup(c => c.Set<Category>()).Returns(_mockSet.Object);

            // Mocklanmış context ile GenericRepository'yi örneklendiriyoruz.
            _repository = new GenericRepository<Category>(_mockContext.Object);
        }

        [Fact]
        public void Add_ShouldAddCategoryToContextAndSave()
        {
            // Arrange
            var newCategory = new Category { CategoryID = 4, CategoryName = "Kategori 4", Status = true };
            
            // Mock DbSet'in Add metodunu yerel listemize eklemeyi simüle etmek üzere ayarlıyoruz
            _mockSet.Setup(m => m.Add(It.IsAny<Category>())).Callback<Category>((entity) => _categories.Add(entity));

            // Act
            _repository.Add(newCategory);

            // Assert
            // Add metodunun DbSet üzerinde bir kez çağrıldığını doğruluyoruz
            _mockSet.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            // SaveChanges metodunun context üzerinde bir kez çağrıldığını doğruluyoruz
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            // Kategorinin mantıksal olarak simüle edilmiş verilerimize eklendiğini doğruluyoruz
            Assert.Contains(newCategory, _categories);
        }

        [Fact]
        public void Delete_ShouldRemoveCategoryFromContextAndSave()
        {
            // Arrange
            var categoryToDelete = _categories.First(c => c.CategoryID == 1); // Mevcut bir kategori alıyoruz
            
            // Mock DbSet'in Remove metodunu yerel listemizden çıkarmayı simüle etmek üzere ayarlıyoruz
            _mockSet.Setup(m => m.Remove(It.IsAny<Category>())).Callback<Category>((entity) => _categories.Remove(entity));

            // Act
            _repository.Delete(categoryToDelete);

            // Assert
            // Remove metodunun DbSet üzerinde bir kez çağrıldığını doğruluyoruz
            _mockSet.Verify(m => m.Remove(It.IsAny<Category>()), Times.Once());
            // SaveChanges metodunun context üzerinde bir kez çağrıldığını doğruluyoruz
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            // Kategorinin mantıksal olarak simüle edilmiş verilerimizden kaldırıldığını doğruluyoruz
            Assert.DoesNotContain(categoryToDelete, _categories);
        }

        [Fact]
        public void Update_ShouldUpdateCategoryInContextAndSave()
        {
            // Arrange
            var categoryToUpdate = _categories.First(c => c.CategoryID == 2);
            categoryToUpdate.CategoryName = "Güncellenmiş Kategori 2";
            categoryToUpdate.Status = false;

            // Mock DbSet'in Update metodunu ayarlıyoruz. Update tipik olarak koleksiyonu değiştirmez,
            // ancak varlığın context'teki durumunu değiştirir. Sadece çağrıyı doğruluyoruz.
            _mockSet.Setup(m => m.Update(It.IsAny<Category>()));

            // Act
            _repository.Update(categoryToUpdate);

            // Assert
            // Update metodunun DbSet üzerinde bir kez çağrıldığını doğruluyoruz
            _mockSet.Verify(m => m.Update(It.IsAny<Category>()), Times.Once());
            // SaveChanges metodunun context üzerinde bir kez çağrıldığını doğruluyoruz
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            // Simüle edilmiş verilerimizdeki kategorinin güncellemeyi yansıttığını doğruluyoruz
            var updatedCategory = _categories.FirstOrDefault(c => c.CategoryID == 2);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Güncellenmiş Kategori 2", updatedCategory.CategoryName);
            Assert.False(updatedCategory.Status);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectCategory()
        {
            // Arrange
            var categoryId = 2;
            
            // Mock DbSet'in Find metodunu ayarlıyoruz. Bu, `params object[] keyValues` ile karmaşıktır.
            // Davranışını, yerel listemizden doğru kategoriyi döndürmek üzere simüle ediyoruz.
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                    .Returns<object[]>(ids => _categories.FirstOrDefault(c => c.CategoryID == (int)ids[0]));

            // Act
            var result = _repository.GetById(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.CategoryID);
            Assert.Equal("Kategori 2", result.CategoryName);
        }

        [Fact]
        public void GetList_ShouldReturnAllCategories()
        {
            // Arrange (zaten constructor'da _categories ve _mockSet kurulumu yapıldı)

            // Act
            var result = _repository.GetList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_categories.Count, result.Count());
            Assert.Collection(result,
                item => Assert.Equal("Kategori 1", item.CategoryName),
                item => Assert.Equal("Kategori 2", item.CategoryName),
                item => Assert.Equal("Kategori 3", item.CategoryName)
            );
        }
    }
}
