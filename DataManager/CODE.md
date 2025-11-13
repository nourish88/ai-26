# Job Request
/job endpoint'ine istek yapılan modeldir.

src/models altındaki job_request.py dosyasında bulunmaktadır.
    
- application_id: Uygulamanın Application tablosundaki id bilgisi
- file_id: Dosyanın File tablosundaki id bilgisi 
- file_store_id: Dosyanın tutulduğu depolamanın id bilgisi
- document_id: Her dosya için üretilen rastgele 36 karakterlik id
- file_extension: Dosya uzantısı (pdf, txt, docx vb.)
- bucket_name: Dosyanın tutulduğu depolamanın bucket name bilgisi (S3 uyumlu depo alanları için)
- storage_type: "s3" (Şu an S3 uyumlu depolama alanı desteklenmekte, diğer dosya sistemlerini desteklemek için genişletilebilir)
- extractor_type: "markitdown" (Dosyaların içeriğinin text olarak elde edilmesi için kullanılan tip. Farklı içerik elde etme yöntemleri için genişletilebilir)
- chunk_type: "markdown, fixed-size, html" (Dosyaları parçalara ayırmak için kullanılan tip. Farklı parçalama yöntemlerini desteklemek için genişletilebilir)
- chunk_size: Kullanılan embedding modelin token boyutuna göre dosyanın ne kadar boyutta parçalanması gerektiği
- chunk_overlap: Dosyalar parçalanırken ne kadar karakterin üst üste binmesine izin verileceği

# Ayarlar
Uygulama, environment variables dan ayarlarını alabildiği gibi, tanımlanan ve uygulamanın kök klasöründe
oluşturulmuş bir .env dosyasından da gerekli ayarları alabilir. Örnek .env dosyası kök dizindeki
.env-sample dosyasında bulunabilir.

Bu ayarlar src/configuration altında tanımlı ConfigurationManager sınıfı yolu ile tüm sınıfların erişimine açılır.

# Extraction

PDF, DOCX gibi dosyaların indexlenebilmesi için içeriklerinin anlamlı ve metin tabanlı hale getirilmesi gerekmektedir.
src/extraction altında sınıfları görülebilir.

ExtractorTypes enum'unda şu an için tek bir extractor tanımı yapılmıştır.

## Markitdown Extractor
MarkItDownExtractor sınıfında tanımlanmıştır. [Microsoft Markitdown](https://github.com/microsoft/markitdown) kütüphanesi kullanılmıştır.
Bu kütüphane, birçok belge tipini desteklemekte ve belgeleri structured Markdown formatına çevirmektedir.

## Yeni extractor sınıflarının oluşturulması
Tüm extractor sınıfları BaseExtractor abstract sınıfından türemelidir.
Yeni sınıfın extractor_types.py içinde tanımlı enum'a eklenmesi gerekir.
aha sonra extractor_factory.py içindeki ExtractorFactory sınıfının constructor metodu olan
__init__() içindeki self.__extractors dictionary objesine eklenmesi gerekir.

# Chunking

AI ingestion işlemlerinde, belgenin, embedding modelin desteklediği token boyutunu 
geçmemesi için daha küçük anlamlı parçalara bölünmesi için kullanılmaktadır.

## Splitters
src/chunking klasöründe, üç farklı türde chunking mekanizması hazırlanmıştır.
Bu türler splitter_types.py dosyasında görülebilir.

### Markdown splitter

Markdown formatındaki belgelerde, header yapılarına göre anlamlı parçalar üretmek 
için kullanılmaktadır. 
Token boyutunun aşılmaması için, ingestion isteğinde gönderilen
chunk_size ve chunk_overlap değerleri de kullanılarak fixed-size olarak tekrar parçalama işlemi yapar.

### Fixed-Size splitter

Bir metni chunk_size ve chunk_overlap parametrelerinde belirtilen boyutlarda parçalar.

### HTML splitter
Bir HTML belgeyi, içindeki h1, h2, h3 vb. taglarını kullanarak anlamlı şekilde parçalar.
Token boyutunun aşılmaması için, ingestion isteğinde gönderilen
chunk_size ve chunk_overlap değerleri de kullanılarak fixed-size olarak tekrar parçalama işlemi yapar.

## Yeni splitter sınıflarının oluşturulması
Tüm splitter sınıfları base_splitter.py'da tanımlı BaseSplitter abstract sınıfından türemelidir.
Yeni sınıfın splitter_types.py içinde tanımlı enum'a eklenmesi gerekir.
Daha sonra splitter_factory.py içindeki SplitterFactory sınıfının constructor metodu olan
__init__() içindeki self.__splitters dictionary objesine eklenmesi gerekir.

# Storage

src/storage klasöründe dosya depolama ile ilgili sınıflar bulunmaktadır.
Backend servislerinden yüklenen dosyalar dosya depolama alanında tutulmaktadır. Bu dosyaların
okunması, metin oluşturulması ve parçalanmış dosyaların yeniden saklanması amacı ile de depolama alanı
kullanılmaktadır.

Şu an sadece s3 depolama desteği bulunmaktadır.

##  S3 depolama
S3Storage sınıfında tanımlanmıştır. Dosya okuma, saklama, silme gibi yöntemleri desteklemektedir.

## Yeni depolama sınıflarının oluşturulması
Tüm depolama sınıfları base_storage.py'da tanımlı BaseStorage abstract sınıfından türemelidir.
Yeni sınıfın storage_types.py içinde tanımlı enum'a eklenmesi gerekir.
storage_factory.py içindeki StorageFactory sınıfının constructor metodu olan
__init__() içindeki self.__storages dictionary objesine eklenmesi gerekir.

# Jobs

src/jobs altında tanımlanmıştır. Ingestion'ın bütün akışını extract_chunk_job.py içinde tanımlı
ExtractChunkJob sınıfındaki execute() metodu içinde görülebilir.

Akış, 

- Idempotency'yi sağlamak için öncelikle depolama alanında {doc_id}/job/completed path'inde bulunan
dosyanın varlığını kontrol eder. Bu dosyanın varlığı halinde işleme devam edilmez.
- Backend servis'ten dosyanın durum bilgisi sorgulanır. Eğer dosyanın durumu ProcessingRequested
değilse işleme devam edilmez.
- Backend'e extraction işleminin başladığı ile ilgili bir istek yapılarak dosyanın durumu, Extracting yapılır.
- Dosya depolama alanında {doc_id}/raw/{doc_id}.{extension} path'ine backend endpoint'i ile yüklenen
dosya okunur
- İstek içinde gönderilen extractor_type alanı kullanılarak ExtractorFactory sınıfından ilgili extractor instance alınır.
- Alınan extractor instance ile dosya markdown'a çevrilir ve {doc_id}/txt/{doc_id}.txt path'i ile depolama alanında saklanır.
- Backend'e chunking işleminin başladığı ile ilgili bir istek yapılarak dosyanın durumu, Chunking yapılır.
- SplitterFactory kullanılarak istek içindeki chunk_type alanından gelen değer ile ilgili Splitter instance alınır.
- Markdown haline getirilmiş dosya splitter instance ile parçalanır ve her bir parçaya UUIDv7 kullanılarak sıralı bir id verilir.
- Her bir chunk, bu id ile {doc_id}/chunks/{chunk_id}.txt path'inde depolama alanında saklanır.
- Bu adıma kadar herhangi bir hata oluşmadıysa, Backend'de indeksleme işleminin başlaaması için bir istek yapılarak dosyanın durumu, Indexing yapılır.
- Herhangi bir hatanın oluşması halinde Backend'e hata bildirilir ve dosyanın durumu ProcessingFailed yapılır.
Ayrıca bütün dosyalar depolama alanından silinir.


