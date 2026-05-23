# HR Management System

Ứng dụng quản lý nhân sự (Human Resource Management) xây dựng bằng **ASP.NET Core 8 MVC**, kiến trúc **Clean/Onion**, database **PostgreSQL**.

Tài liệu này hướng dẫn chạy toàn bộ hệ thống bằng **Docker** — không cần cài .NET SDK hay PostgreSQL trên máy (chỉ cần Docker).

---

## Yêu cầu

| Công cụ | Phiên bản gợi ý |
|---------|------------------|
| [Docker](https://docs.docker.com/get-docker/) | 24+ |
| [Docker Compose](https://docs.docker.com/compose/install/) | v2+ (`docker compose`) |

Kiểm tra:

```bash
docker --version
docker compose version
```

---

## Chạy nhanh (Quick Start)

Clone hoặc giải nén source code, mở terminal tại thư mục chứa file `docker-compose.yml` (thư mục gốc solution):

```bash
cd HR_Management
```

**1. Build và khởi động containers**

```bash
docker compose up -d --build
```

Lần đầu có thể mất vài phút (tải image .NET, PostgreSQL, build web app).

**2. (Tuỳ chọn) Nạp dữ liệu mẫu**

```bash
chmod +x scripts/seed.sh
./scripts/seed.sh
```

**3. Mở trình duyệt**

| Dịch vụ | Địa chỉ |
|---------|---------|
| **Web application** | http://localhost:5080 |
| **Đăng nhập** | http://localhost:5080/Account/Login |

---

## Đăng nhập

### Sau khi chạy `seed.sh` (khuyến nghị)

| Email | Mật khẩu |
|-------|-----------|
| `admin@hr.com` | `Admin@1234` |
| `hr.user@company.com` | `Admin@1234` |

### Không chạy seed — tự đăng ký

1. Vào http://localhost:5080/Account/Register  
2. Tạo tài khoản (email + mật khẩu tối thiểu 6 ký tự)  
3. Đăng nhập tại http://localhost:5080/Account/Login  

---

## Cấu trúc Docker

```
┌─────────────┐     :5080      ┌──────────────────┐
│  Trình duyệt │ ──────────────►│  hr-management-web│
└─────────────┘                │  (ASP.NET Core 8) │
                               └────────┬─────────┘
                                        │ Host=db:5432
                                        ▼
                               ┌──────────────────┐
                               │ hr-management-db │
                               │ PostgreSQL 16    │
                               └──────────────────┘
                                        :5433 (host)
```

| Container | Image / Build | Port (host) |
|-----------|---------------|-------------|
| `hr-management-web` | Build từ `Dockerfile` | **5080** → 8080 |
| `hr-management-db` | `postgres:16-alpine` | **5433** → 5432 |

- Migration database chạy **tự động** khi container `web` khởi động.
- Dữ liệu PostgreSQL lưu trong Docker volume `postgres_data`.

---

## Lệnh Docker thường dùng

```bash
# Khởi động (nền)
docker compose up -d

# Build lại sau khi sửa code
docker compose up -d --build

# Xem log web app
docker compose logs -f web

# Xem log database
docker compose logs -f db

# Trạng thái containers
docker compose ps

# Dừng containers (giữ dữ liệu DB)
docker compose down

# Dừng và XÓA dữ liệu database
docker compose down -v
```

---

## Dữ liệu mẫu (Seed)

Script nạp 4 phòng ban, 7 nhân viên và 2 tài khoản đăng nhập:

```bash
./scripts/seed.sh          # qua container Docker (mặc định)
./scripts/seed.sh --local  # PostgreSQL local port 5433
```

Chạy trực tiếp file SQL:

```bash
docker exec -i hr-management-db psql -U postgres -d HR_Manage < scripts/seed-data.sql
```

> **Lưu ý:** Seed sẽ `TRUNCATE` và ghi lại dữ liệu trong `Department_Tbl`, `Employee_Tbl`, `User_Tbl`.

---

## Cấu hình

### Đổi mật khẩu PostgreSQL

Sửa đồng bộ trong `docker-compose.yml`:

- `POSTGRES_PASSWORD` (service `db`)
- `ConnectionStrings__DefaultConnection` (service `web`)

Sau đó:

```bash
docker compose down -v
docker compose up -d --build
./scripts/seed.sh
```

### Đổi port web (mặc định 5080)

Trong `docker-compose.yml`, service `web`:

```yaml
ports:
  - "5080:8080"   # HOST:CONTAINER
```

Đổi `5080` thành port trống trên máy bạn (ví dụ `8080:8080`).

---

## Xử lý sự cố

| Triệu chứng | Cách xử lý |
|-------------|------------|
| `address already in use` (port 5080) | Đổi port host trong `docker-compose.yml` hoặc tắt process đang chiếm port |
| Web không vào được | `docker compose ps` — đợi `db` **healthy**, xem `docker compose logs web` |
| Đăng nhập sai sau seed | Dùng đúng `admin@hr.com` / `Admin@1234` |
| Database trống | Chạy lại `./scripts/seed.sh` |
| Sửa code không thấy thay đổi | `docker compose up -d --build` |

---

## Chạy không dùng Docker (tuỳ chọn)

Dành cho developer có .NET 8 SDK và PostgreSQL local:

```bash
dotnet restore
dotnet ef database update --project HR_Management.Infrastructure --startup-project HR_Management.Web
dotnet run --project HR_Management.Web
```

Cập nhật connection string trong `HR_Management.Web/appsettings.Development.json`.

---

## Cấu trúc solution

```
HR_Management/
├── HR_Management.Domain/          # Entities, repository interfaces
├── HR_Management.Application/     # DTOs, services, AutoMapper
├── HR_Management.Infrastructure/  # EF Core, PostgreSQL, repositories
├── HR_Management.Web/             # MVC, controllers, views
├── scripts/
│   ├── seed-data.sql              # Dữ liệu mẫu PostgreSQL
│   ├── seed.sh                    # Chạy seed
│   └── tools/PasswordHashGen/     # Tạo hash mật khẩu cho seed
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## Tính năng chính

- Đăng ký / đăng nhập (cookie authentication)
- CRUD phòng ban (Department)
- Thêm nhân viên theo phòng ban
- Báo cáo thống kê nhân sự theo phòng ban

---

## License

Dự án học tập / nội bộ — điều chỉnh theo quy định của tổ chức bạn.
