using Xunit;
using Moq;
using SignalR.DataAccessLayer.Abstract;
using SignalR.EntityLayer.Entities;
using SignalR.DataAccessLayer.Concrete;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SignalR.DataAccessLayer.Tests.Repositories
{
    public class ExampleRepositoryTests
    {
        private readonly Mock<IExampleRepository> _mockRepository;
        private readonly DbContextOptions<SignalRContext> _dbContextOptions;

        public ExampleRepositoryTests()
        {
            _mockRepository = new Mock<IExampleRepository>();

            // In-memory veritabanı kurulumu (gerçek veritabanı bağlantısı gerektirmeyen testler için)
            _dbContextOptions = new DbContextOptionsBuilder<SignalRContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public void Add_WhenCalled_AddsEntityToDatabase()
        {
            // Arrange
            var entity = new ExampleEntity { Id = 1, Name = "Test Example" };
            
            // In-memory context ile test
            using (var context = new SignalRContext(_dbContextOptions))
            {
                var repository = new ExampleRepository(context); // ExampleRepository'nizin SignalRContext aldığını varsayıyoruz.

                // Act
                repository.Add(entity);
                context.SaveChanges(); // Değişiklikleri kaydet

                // Assert
                var addedEntity = context.Set<ExampleEntity>().FirstOrDefault(e => e.Id == entity.Id);
                Assert.NotNull(addedEntity);
                Assert.Equal(entity.Name, addedEntity.Name);
            }
        }

        [Fact]
        public void GetList_ReturnsAllEntities()
        {
            // Arrange
            var entities = new List<ExampleEntity>
            {
                new ExampleEntity { Id = 1, Name = "Test 1" },
                new ExampleEntity { Id = 2, Name = "Test 2" }
            };

            using (var context = new SignalRContext(_dbContextOptions))
            {
                context.Set<ExampleEntity>().AddRange(entities);
                context.SaveChanges();
                var repository = new ExampleRepository(context);

                // Act
                var result = repository.GetList(); // Varsayımsal GetList metodunu çağırıyoruz.

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.Contains(result, e => e.Name == "Test 1");
            }
        }

        // Gerçek IExampleRepository interface'ini mocklayarak test örneği (Yukarıdaki in-memory db örneğinden farklı bir yaklaşım)
        [Fact]
        public void GetById_WhenEntityExists_ReturnsEntity()
        {
            // Arrange
            var expectedEntity = new ExampleEntity { Id = 10, Name = "Mocked Example" };
            _mockRepository.Setup(r => r.GetById(10)).Returns(expectedEntity);

            // Act
            var result = _mockRepository.Object.GetById(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEntity.Id, result.Id);
            Assert.Equal(expectedEntity.Name, result.Name);
        }
    }

    // Test amaçlı varsayılan entity ve repository sınıfları/interfaceleri
    // Gerçek projede bu sınıflar 'SignalR.EntityLayer' ve 'SignalR.DataAccessLayer' içinde yer almalıdır.
    public class ExampleEntity : BaseEntity // BaseEntity olduğunu varsayıyoruz
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IExampleRepository : IGenericRepository<ExampleEntity>
    {
        // Özel metodlar varsa buraya eklenebilir
    }

    public class ExampleRepository : GenericRepository<ExampleEntity>, IExampleRepository
    {
        public ExampleRepository(SignalRContext context) : base(context)
        {
        }
        
        // Örnek olarak GetList metodu (GenericRepository içinde olmalı veya override edilmeli)
        public IEnumerable<ExampleEntity> GetList()
        {
            return _context.Set<ExampleEntity>().ToList();
        }

        public ExampleEntity GetById(int id)
        {
            return _context.Set<ExampleEntity>().FirstOrDefault(e => e.Id == id);
        }
    }

    // SignalRContext ve BaseEntity'nin gerçek projenizde tanımlı olduğunu varsayıyoruz.
    // Test için basit versiyonlarını ekliyoruz.
    public class SignalRContext : DbContext
    {
        public SignalRContext(DbContextOptions<SignalRContext> options) : base(options)
        {
        }

        public DbSet<ExampleEntity> ExampleEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExampleEntity>().HasKey(e => e.Id);
            base.OnModelCreating(modelBuilder);
        }
    }

    public class BaseEntity
    {
        // Ortak özellikler burada olabilir
    }

    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetList();
        T GetById(int id);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly SignalRContext _context;

        public GenericRepository(SignalRContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public T GetById(int id)
        { 
            // Bu metot, BaseEntity'deki Id özelliğine doğrudan erişim için bir varsayım içeriyor.
            // Gerçek implementasyonda daha esnek bir yapı gerekebilir.
            return _context.Set<T>().Find(id);
        }

        public IEnumerable<T> GetList()
        {
            return _context.Set<T>().ToList();
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}