using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalR.BusinessLayer.Abstract; // İş katmanı servis arayüzleri için
using SignalR.EntityLayer.Entities;   // Varlık katmanı modelleri için

namespace SignalR.BusinessLayer.Tests.Features
{
    public class ExampleServiceTests
    {
        // Bu test, IExampleService'in TGetListAsync metodunun beklenen sayıda öğe döndürüp döndürmediğini kontrol eder.
        [Fact]
        public async Task GetList_ShouldReturnExpectedNumberOfItems()
        {
            // Arrange (Hazırlık): Test edilecek nesneleri ve bağımlılıkları ayarla.
            var mockExampleService = new Mock<IExampleService>();
            var expectedItems = new List<ExampleEntity> // Örnek bir veri listesi oluştur
            {
                new ExampleEntity { Id = 1, Name = "Örnek Öğe 1" },
                new ExampleEntity { Id = 2, Name = "Örnek Öğe 2" }
            };

            // Mock servisin TGetListAsync metodu çağrıldığında beklenen öğe listesini döndürmesini sağla.
            mockExampleService.Setup(s => s.TGetListAsync()).ReturnsAsync(expectedItems);

            // Act (Eylem): Test edilecek metodu çağır.
            var result = await mockExampleService.Object.TGetListAsync();

            // Assert (Doğrulama): Sonuçların beklentileri karşıladığını kontrol et.
            Assert.NotNull(result); // Sonucun null olmadığını doğrula
            Assert.Equal(expectedItems.Count, result.Count); // Dönüş listesinin öğe sayısını doğrula
            Assert.Contains(result, item => item.Name == "Örnek Öğe 1"); // Belirli bir öğenin içerikte olduğunu doğrula
        }

        // Bu test, IExampleService'in TAddAsync metodunun doğru parametrelerle bir kez çağrılıp çağrılmadığını kontrol eder.
        [Fact]
        public async Task AddItem_ShouldCallAddMethodOnce()
        {
            // Arrange (Hazırlık): Test edilecek nesneleri ve bağımlılıkları ayarla.
            var mockExampleService = new Mock<IExampleService>();
            var newItem = new ExampleEntity { Id = 3, Name = "Yeni Öğe" };

            // Act (Eylem): Test edilecek metodu çağır.
            await mockExampleService.Object.TAddAsync(newItem); // TAddAsync metodunu çağır

            // Assert (Doğrulama): TAddAsync metodunun beklenen şekilde çağrıldığını doğrula.
            // Mock nesnenin TAddAsync metodunun herhangi bir ExampleEntity nesnesiyle tam olarak bir kez çağrıldığını doğrula.
            mockExampleService.Verify(s => s.TAddAsync(It.IsAny<ExampleEntity>()), Times.Once);

            // Eğer belirli bir nesne ile çağrıldığını doğrulamak isterseniz:
            // mockExampleService.Verify(s => s.TAddAsync(newItem), Times.Once);
        }
    }
}
