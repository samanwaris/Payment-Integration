# Payment-Integration
Payment Gateway SNAP and NON SNAP Integration (.NET)

## 📌 Overview

Project ini adalah implementasi Payment Gateway API berbasis ASP.NET Core yang terintegrasi dengan standar SNAP (Standar Nasional Open API Pembayaran)
dan pembayaran NON SNAP secara umum.

Project ini menggunakan pendekatan **Multi-Layer Architecture**:
* `payment_gateway` → API Layer (Controller, Filter, Attribute)
* `payment-service` → Business Logic Layer (Service, Model, Interface)

## 🏗️ Project Structure
```
payment-gateway/
│
├── payment_gateway/        # API Layer
│   ├── Controllers/
│   ├── Attributes/
│   ├── Filters/
│   ├── Program.cs
│   ├── appsettings.json
│   └── payment-gateway.http
│
├── payment-service/        # Business Logic Layer
│   ├── Interfaces/
│   ├── Services/
│   ├── Models/
│   │   ├── Request/
│   │   └── Response/
│   ├── Helper/
│   ├── PaymentSetting.cs
│   └── SnapRequestHeader.cs
│
└── README.md
```

## 🚀 Tech Stac
* .NET (ASP.NET Core)
* C#
* Swagger / Swashbuckle
* HttpClientFactory
* RSA SHA256 (Signature SNAP)

## ⚙️ Configuration
Gunakan `appsettings.json` sebagai template untuk secretkey, url dan endpoint ke layanan payment gateway:

```json
"PaymentSettings": {
  "SecretKey": "",
  "BaseUrl": {
    "SnapUrl": "",
    "NoSnapUrl": ""
  },
  "Endpoints": {
    "Register": "",
    "CreateVA": "",
    "Payment": ""
  }
```

## 🔐 SNAP Required Headers
| Header        | Description          |
| ------------- | -------------------- |
| X-SIGNATURE   | Signature RSA SHA256 |
| X-TIMESTAMP   | Timestamp ISO8601    |
| X-PARTNER-ID  | Partner ID           |
| X-EXTERNAL-ID | Unique Request ID    |
| CHANNEL-ID    | Channel Identifier   

## 📌 Example Endpoint
### POST `/api/payment-settlement-snap`
### Headers
```
X-SIGNATURE: <signature>
X-TIMESTAMP: 2026-04-17T10:00:00+07:00
X-PARTNER-ID: your-partner-id
X-EXTERNAL-ID: unique-id
CHANNEL-ID: 6011
```

### Body
```json
{
  "partnerServiceId": "12345",
  "customerNo": "001",
  "virtualAccountNo": "123456789",
  "amount": 450000.00
}
```
## 🧪 Swagger
Jalankan project: ``` dotnet run --project payment_gateway ```
Akses Swagger: ```https://localhost:{port}/swagger```

## 🏗️ How to Run
```bash
git clone https://github.com/your-username/payment-gateway.git
cd payment-gateway
dotnet restore
dotnet run --project payment_gateway
```

## 🔥 Key Features
* ✅ SNAP Header Validation (Custom Attribute)
* ✅ Swagger SNAP Header Support
* ✅ Typed HttpClient (HttpClientFactory)
* ✅ Clean Separation (API vs Service Layer)
* ✅ Ready for Integration

## ⚠️ Notes
* Gunakan raw body saat generate signature
* Jangan ubah format JSON sebelum verifikasi signature
* Pastikan timestamp sesuai ISO8601

## 📄 License
Internal / Learning Purpose

## 👨‍💻 Author
Developed by: **Syamsul Anwar**
