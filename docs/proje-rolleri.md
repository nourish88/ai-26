# Proje Rolleri ve Çalıştırma Rehberi

Bu rehber, repoda yer alan her bir servisin amacını, temel bileşenlerini ve geliştirme ortamında ayağa kaldırma adımlarını özetler. Tüm komutlar macOS/Linux shell söz dizimine göre verilmiştir.

---

## 1. DevEnvironment

- **Rol:** Ortak altyapı servislerini (PostgreSQL, Keycloak, Redis, MinIO, MongoDB, LiteLLM, Langfuse, Elastic/Kibana) Docker Compose ile ayağa kaldırır.
- **Nasıl Çalıştırılır:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/DevEnvironment
  cp .env.example .env  # örnek dosya varsa
  docker compose --env-file .env up --build -d
  ```
- **Kontroller:** `docker compose ps`; Keycloak `http://localhost:8080`, Postgres `127.0.0.1:5532`, MinIO `http://localhost:8091`.
- **Notlar:** `data/keycloak/jugaai-realm-export.json` dosyasını Keycloak’a import edin; Langfuse portu Next.js ile çakışıyorsa `.env` içinde değiştirin.

---

## 2. AdminBackend (ASP.NET Core)

- **Rol:** Uygulama ayarlarını, ingest sürecini ve dosya yönetimini sağlayan ana yönetim API’si.
- **Bağımlılıklar:** Postgres `jugaai`, Redis, MinIO, DataManager endpoint’i, Keycloak realm.
- **Çalıştırma:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/AdminBackend/AdminBackend
  dotnet restore
  dotnet run --project src/AdminBackend.Api/AdminBackend.Api.csproj
  ```
- **Port:** `http://localhost:5006`
- **Notlar:** Migrationlar yapılandırmaya göre otomatik çalıştırılır; gerekirse `dotnet ef database update` ekleyin.

---

## 3. AIOrchestrator (FastAPI)

- **Rol:** Agent ve sohbet akışlarını yönetir; AdminBackend’den yapılandırma alır, Redis/Mongo cache kullanır, Langfuse ile gözlemlenebilirlik sağlar.
- **Hazırlık:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/AIOrchestrator
  cp .env-sample .env
  python -m venv .venv && source .venv/bin/activate
  pip install -r requirements.txt
  ```
- **Çalıştırma:** `python main.py` → `http://localhost:12000/docs`
- **Notlar:** `ADMIN_BACKEND_URL`, `AUTH_*`, `REDIS_URL`, `MONGO_CONN_STRING`, `LANGFUSE_*` değerlerini doldurun. LiteLLM servisinin (`http://localhost:4000`) erişilebilir olduğundan emin olun.

---

## 4. DataManager (FastAPI)

- **Rol:** Dosya ingest pipeline’ını yürütür, belgeleri MinIO’ya yazar ve AdminBackend’e durum bildirir.
- **Hazırlık:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/DataManager
  cp .env-sample .env
  python -m venv .venv && source .venv/bin/activate
  pip install -r requirements.txt
  ```
- **Çalıştırma:** `python server.py` → `http://localhost:18787/docs`
- **Notlar:** MinIO `juga` bucket’ı hazır olmalı; `AUTH_*` alanları Keycloak istemcinizle eşleşmeli; Redis ayrı bir DB (`/2`) kullanır.

---

## 5. ToolGateway (ASP.NET Core + MCP)

- **Rol:** MCP uyumlu araçları merkezi bir API üzerinden sunar; AdminBackend ile paylaşılan altyapıyı kullanır.
- **Çalıştırma:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/ToolGateway/ToolGateway
  dotnet restore
  dotnet run --project src/ToolGateway.Api/ToolGateway.Api.csproj
  ```
- **Port:** `http://localhost:5164`
- **Notlar:** `ConnectionStrings.Database` Postgres `toolgateway` veritabanına işaret eder; MCP endpoint’i `/api/mcp` ve JWT ile korunur.

---

## 6. ALIM (Next.js)

- **Rol:** Son kullanıcı portalı; AIOrchestrator API’sini kullanarak uygulama listeleri, sohbet ve dosya yönetimi sağlar.
- **Hazırlık ve Çalıştırma:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/ALIM
  cp .env.example .env.local  # örnek varsa
  pnpm install
  pnpm dev -- -p 3002
  ```
- **Notlar:** `AI_ORCH_URL`, `NEXTAUTH_URL`, `AUTH_KEYCLOAK_*`, `NEXT_PUBLIC_AUTH_*` gibi değerleri doldurun. `pnpm` kullanımı `packageManager` alanında tanımlıdır.

---

## 7. JUGA_ADMIN_UI (Next.js)

- **Rol:** Yönetim odaklı arayüz; Langfuse API’leri ve Admin işlevleri için geliştirme aşamasında.
- **Hazırlık ve Çalıştırma:**
  ```bash
  cd /Users/nuriaktas/Desktop/juga-ai/ai-26/JUGA_ADMIN_UI
  cp .env.example .env.local  # örnek varsa
  npm install
  npm run dev  # varsayılan 3001
  ```
- **Notlar:** Keycloak refresh token akışı için `AUTH_KEYCLOAK_*` değerleri zorunlu; Langfuse entegrasyonu için `LANGFUSE_BASE_URL`, `LANGFUSE_PUBLIC_KEY`, `LANGFUSE_SECRET_KEY`.

---

## Bütün Sistemi Ayağa Kaldırma Sırası

1. **Altyapı:** `DevEnvironment` compose dosyasını çalıştırın ve Keycloak realm importunu yapın.
2. **Backend Servisleri:** `AdminBackend`, `ToolGateway`, `DataManager`, `AIOrchestrator` sırasıyla çalışsın.
3. **Ön Yüzler:** `ALIM` ve `JUGA_ADMIN_UI` dev sunucuları; port çakışması varsa `-p` parametresiyle değiştirin.
4. **Doğrulama:** Her servis için Swagger/Docs URL’lerini kontrol edin, Keycloak üzerinden test kullanıcı girişleri yapın.

---

## Önerilen İyileştirmeler

- Ortak `.env.example` dosyaları oluşturup tüm projeler için güncel tutun.
- `Makefile` veya `justfile` üzerinden `make up`, `make dev-adminbackend` gibi hedefler tanımlayın.
- Docker tabanlı çalışma tercih ediliyorsa Python servisleri için Dockerfile/compose servisleri hazırlayın.
- Production ortamı için reverse proxy ve HTTPS yapılandırmasını değerlendirin.

---

## Tek Komutla Compose Çalıştırma

- Kök dizindeki `docker-compose.yml` tüm servisleri (altyapı + backend + front-end) tek seferde ayağa kaldırır.
- Ortak değişkenler `dev.env` altında tutulur; örnek kullanım:
  ```bash
  docker compose --env-file dev.env up --build
  ```
- İhtiyaç halinde `dev.env` içindeki Keycloak istemci bilgileri ve dış URL’leri kendi ortamınıza göre güncelleyin.
