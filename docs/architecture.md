# Proje Mimari Dokümantasyonu

Bu doküman, `.NET` tabanlı `SignalR` projesinin mimarisini, ana bileşenlerini ve katmanlı yapısını detaylandırmaktadır. Projenin temel amacı, gerçek zamanlı iletişim yetenekleri sunan, ölçeklenebilir ve sürdürülebilir bir uygulama geliştirmektir.

## 1. Genel Bakış

Proje, `ASP.NET Core` ve `SignalR` teknolojilerini kullanarak gerçek zamanlı web uygulamaları geliştirmek için tasarlanmıştır. Uygulama, güçlü bir iş mantığı katmanı, verimli bir veri erişim katmanı ve kullanıcı dostu bir arayüz ile çok katmanlı bir mimariye sahiptir. Hem bir `Web API` hem de bir `Web Kullanıcı Arayüzü` içerir.

## 2. Katmanlı Mimari

Uygulama, sorumlulukların ayrılması prensibine uygun olarak aşağıdaki katmanlara ayrılmıştır:

### 2.1. EntityLayer

*   **Amaç:** Veritabanı tablolarını temsil eden POCO (Plain Old C# Object) sınıflarını içerir. Uygulamanın temel veri yapılarını tanımlar.
*   **Sorumluluklar:** Veritabanı nesnelerinin özelliklerini ve ilişkilerini belirlemek.

### 2.2. DtoLayer (Data Transfer Object Layer)

*   **Amaç:** Katmanlar arasında veri transferi için kullanılan DTO'ları (Veri Transfer Nesneleri) tanımlar. Bu DTO'lar, genellikle EntityLayer varlıklarının belirli bir görünümünü veya birleşimini temsil eder.
*   **Sorumluluklar:** Farklı katmanlar arasında veri alışverişini optimize etmek, hassas verileri gizlemek ve belirli bir senaryo için gerekli veriyi sağlamak.

### 2.3. DataAccessLayer

*   **Amaç:** Veritabanı ile doğrudan etkileşim kuran sınıfları (örneğin, Repository deseninin uygulamaları) ve `Entity Framework Core` bağlamını içerir.
*   **Sorumluluklar:** CRUD (Oluşturma, Okuma, Güncelleme, Silme) operasyonlarını gerçekleştirmek, veritabanı bağlantısını yönetmek ve veri sorgularını yürütmek.

### 2.4. BusinessLayer

*   **Amaç:** Uygulamanın temel iş mantığını ve kurallarını barındırır. `DataAccessLayer`'ı kullanarak veriye erişir ve DTO'lar aracılığıyla `SignalRApi` ve `SignalRWebUI` katmanlarına hizmet verir.
*   **Sorumluluklar:** İş kurallarını uygulamak, işlemleri (transaction) yönetmek, veri doğrulaması yapmak ve iş akışlarını kontrol etmek. Bağımlılık Enjeksiyonu (DI) aracılığıyla `DataAccessLayer` servislerini kullanır.

## 3. Uygulama Bileşenleri

### 3.1. SignalRApi

*   **Amaç:** RESTful API servislerini ve `SignalR Hub`'larını barındırır. Web ve diğer istemcilerin uygulama ile etkileşim kurmasını sağlar.
*   **Sorumluluklar:** İstemci isteklerini işlemek, `BusinessLayer` ile etkileşime geçmek, HTTP yanıtları döndürmek ve gerçek zamanlı bildirimleri veya güncellemeleri `SignalR Hub`'ları aracılığıyla yayınlamak.

### 3.2. SignalRWebUI

*   **Amaç:** Kullanıcıların uygulama ile etkileşimde bulunduğu ana web arayüzünü içerir. `SignalRApi`'deki servisleri ve `SignalR Hub`'larını kullanarak veri alır ve güncellemeleri görüntüler.
*   **Sorumluluklar:** Kullanıcı arayüzünü oluşturmak, kullanıcı girdilerini işlemek, API çağrıları yapmak ve `SignalR` üzerinden gelen gerçek zamanlı güncellemeleri göstermek.

## 4. Teknoloji Yığını

*   **.NET:** Uygulamanın geliştirildiği ana platform.
*   **ASP.NET Core:** Web API ve Web UI katmanlarının geliştirilmesinde kullanılan çerçeve.
*   **SignalR:** Gerçek zamanlı, çift yönlü iletişim için kullanılan kütüphane.
*   **C#:** Uygulamanın ana programlama dili.
*   **Entity Framework Core (Varsayımsal):** Veritabanı etkileşimi için ORM (Object-Relational Mapper) aracı.

## 5. Gerçek Zamanlı İletişim

`SignalR`, sunucu ve istemci arasında kalıcı bağlantılar kurarak anlık veri güncellemelerini ve bildirimleri mümkün kılar. `NotificationHub.cs` gibi özel `Hub` sınıfları aracılığıyla, sunucudan tüm bağlı istemcilere veya belirli istemcilere mesajlar gönderilebilir, bu da kullanıcı deneyimini önemli ölçüde geliştirir.

## 6. Veri Akışı

Genel veri akışı aşağıdaki gibidir:

1.  **Kullanıcı Arayüzü (SignalRWebUI):** Kullanıcı etkileşimleri veya periyodik yenilemeler aracılığıyla `SignalRApi`'ye istek gönderir.
2.  **Web API (SignalRApi):** Gelen isteği alır ve `BusinessLayer`'daki ilgili servisi çağırır.
3.  **İş Mantığı (BusinessLayer):** İş kurallarını uygular, gerekli durumlarda `DataAccessLayer`'ı çağırır.
4.  **Veri Erişimi (DataAccessLayer):** Veritabanı operasyonlarını gerçekleştirir ve `EntityLayer` varlıkları üzerinden veriyi `BusinessLayer`'a döndürür.
5.  **İş Mantığı (BusinessLayer):** Gelen varlıkları `DtoLayer` DTO'larına dönüştürerek `SignalRApi`'ye geri gönderir.
6.  **Web API (SignalRApi):** DTO'ları HTTP yanıtı olarak `SignalRWebUI`'ye veya `SignalR Hub`'ları aracılığıyla bağlı istemcilere gönderir.
7.  **Gerçek Zamanlı Güncellemeler:** `SignalRApi`'deki `Hub`'lar, belirli olaylar veya veri değişiklikleri meydana geldiğinde `SignalRWebUI`'deki veya diğer bağlı istemcilerdeki JavaScript istemcilerine anında bildirimler gönderir.

## 7. Sonuç

Bu mimari, `SignalR` projesinin modüler, esnek ve bakımı kolay olmasını sağlamak üzere tasarlanmıştır. Katmanlı yapı ve açıkça tanımlanmış sorumluluklar, uygulamanın farklı bölümlerinin bağımsız olarak geliştirilmesine ve test edilmesine olanak tanır. Gerçek zamanlı iletişim yetenekleri, kullanıcı deneyimini zenginleştirerek projenin ana hedeflerinden birini gerçekleştirmektedir.