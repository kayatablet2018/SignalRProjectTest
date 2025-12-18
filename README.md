# SignalR Projesi Repo Analizi

Bu depo, SignalR teknolojilerini kullanan katmanlı bir .NET projesidir. Proje, iş katmanı, veri erişim katmanı, DTO'lar, varlıklar, bir API projesi ve bir web kullanıcı arayüzü projesi olmak üzere çeşitli katmanlardan oluşmaktadır.

## Teknolojiler

*   .NET
*   C#
*   SignalR

## Özellikler

*   **İş Katmanı (SignalR.BusinessLayer):** İş mantığını ve kurallarını içerir.
*   **Veri Erişim Katmanı (SignalR.DataAccessLayer):** Veritabanı işlemleri ve veri erişimini yönetir.
*   **DTO Katmanı (SignalR.DtoLayer):** Veri Transfer Nesnelerini (DTO'lar) tanımlar.
*   **Varlık Katmanı (SignalR.EntityLayer):** Veritabanı varlıklarını (modelleri) içerir.
*   **SignalR API Projesi (SignalRApi):** Gerçek zamanlı iletişim için SignalR tabanlı API hizmetlerini sunar.
*   **SignalR Web Kullanıcı Arayüzü (SignalRWebUI):** API ile etkileşim kuran ve gerçek zamanlı güncellemeleri gösteren web arayüzü.

## Proje Yapısı

*   `.gitignore`: Git tarafından izlenmeyecek dosyaları belirtir.
*   `README.md`: Projenin genel açıklamalarını ve kurulum bilgilerini içerir.
*   `SignalR.BusinessLayer`: İş mantığı katmanını barındırır.
*   `SignalR.DataAccessLayer`: Veritabanı işlemleri ve veri erişimini yönetir.
*   `SignalR.DtoLayer`: Veri Transfer Nesnelerini (DTO'lar) içerir.
*   `SignalR.EntityLayer`: Veritabanı varlıklarını (modelleri) tanımlar.
*   `SignalRApi`: SignalR tabanlı API hizmetlerini sunan proje.
*   `SignalRProject.sln`: Visual Studio çözüm dosyası.
*   `SignalRWebUI`: Web kullanıcı arayüzü projesini içerir.
*   `Triggers.txt`: Belirli tetikleyiciler veya notlar içeren metin dosyası.