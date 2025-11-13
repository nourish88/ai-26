# Python Servisleri İçin Ortam Yönetimi Rehberi

Bu doküman, repodaki Python tabanlı servislerin (`AIOrchestrator`, `DataManager`) geliştirme ortamlarını tutarlı ve güvenli şekilde yönetmek için önerilen adımları içerir.

## 1. Python Sürümü ve Yalıtılmış Ortamlar

### 1.1 pyenv ile küresel sürüm yönetimi

```bash
brew update
brew install pyenv
pyenv install 3.12.7
pyenv local 3.12.7  # Servis dizininde çalıştırın
```

### 1.2 Virtualenv/venv oluşturma

```bash
python -m venv .venv
source .venv/bin/activate
python -m pip install --upgrade pip
```

### 1.3 Alternatif: uv

Projede daha hızlı kurulum tercih ederseniz:

```bash
curl -LsSf https://astral.sh/uv/install.sh | sh
uv venv
source .venv/bin/activate
```

## 2. Ortam Değişkenleri

### 2.1 `.env` dosyası oluşturma

Tüm Python servisleri için ortak pratik:

```bash
cp .env-sample .env
```

Dosya içinde Keycloak istemci sırları, veritabanı şifreleri, Redis/MinIO bilgileri yer alır. Üretim dışı ortamlar için güçlü, benzersiz parolalar kullanın.

### 2.2 Örnek Anahtarlar

- `AIOrchestrator`: `ADMIN_BACKEND_URL`, `MONGO_CONN_STRING`, `AUTH_ISSUER`, `AUTH_AUDIENCE`, `REDIS_URL`, `LANGFUSE_*`, `LITE_LLM_API_KEY`
- `DataManager`: `S3_*`, `ADMIN_BACKEND_URL`, `AUTH_*`, `REDIS_URL`, `DEFAULT_CACHE_EXPIRE_IN_MINUTE`, `ES_*`

### 2.3 Gizli bilgileri yönetme

- `.env` dosyalarını Git’e eklemeyin (`.gitignore` zaten engelliyor).
- Paylaşılabilir değillerse `direnv allow` + `.envrc` ile CI/CD dışı ortamlarda şifreleri otomatik yükleyin.
- Takım içi paylaşım için Vault/Doppler gibi gizli yönetim araçlarını değerlendirin.

## 3. Bağımlılık Kurulumu

### 3.1 AIOrchestrator

```bash
cd AIOrchestrator
source .venv/bin/activate
pip install -r requirements.txt
```

### 3.2 DataManager

```bash
cd DataManager
source .venv/bin/activate
pip install -r requirements.txt
```

### 3.3 Bağımlılık kilidi

- `requirements.txt` dosyaları sabit sürümler içeriyor; güncellemeden önce `pip-compile` veya `uv lock` gibi araçlarla test ederek ilerleyin.

## 4. Otomasyon Önerileri

- `direnv` kullanıyorsanız servis köküne `.envrc` ekleyip şunu yazabilirsiniz:
  ```
  layout python3
  source .venv/bin/activate
  ```
  Ardından `direnv allow` komutunu çalıştırın.
- Ortak komutlar için `Makefile` veya `justfile` ekleyerek `make setup-aiorchestrator`, `make run-datamanager` gibi hedefler tanımlayın.

## 5. Çalıştırma

- `AIOrchestrator`: `python main.py` (FastAPI 12000 portu)
- `DataManager`: `python server.py` (FastAPI 18787 portu)
- MinIO `juga` bucket’ının hazır olduğundan emin olun; ilk ingest işlemini `AdminBackend` üzerinden başlatın.

## 6. İleri Seviye

- Test ortamı için `pytest` veya benzeri çatıları adapte edin ve sanal ortam içinden çalıştırın.
- Production deployment için Docker image’ları oluşturmayı düşünüyorsanız `pip install -r requirements.txt` adımını image build aşamasına taşıyın.

Bu rehberi diğer Python servisleri için de temel alabilir, gerektiğinde `requirements.txt` ve `.env-sample` dosyalarını genişletebilirsiniz.
