# Digital Wallet API - Documentation


## Table of Contents

1. [Project Overview](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
2. [Key Features](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
3. [Technology Stack](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
4. [System Architecture](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
5. [Use Cases](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
6. [API Endpoints](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
7. [Data Schemas](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
8. [Authentication](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
9. [Database Design](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
10. [Setup & Deployment](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
11. [Testing Strategy](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)
12. [Future Roadmap](https://www.notion.so/Digital-Wallet-API-Documentation-1f4c2b9891848013bfb5f1e401920be2?pvs=21)

## Project Overview

A secure, scalable API for managing digital wallets and financial transactions between authenticated users. Built with .NET 8 following Clean Architecture principles.

## Key Features

- User registration and JWT authentication
- Wallet balance management
- Secure fund transfers between users
- Transaction history with filtering
- Comprehensive input validation
- Audit logging for all transactions

## Technology Stack

### Core Technologies

| Category | Technology |
| --- | --- |
| Backend | .NET 8, [ASP.NET](http://asp.net/) Core |
| Database | PostgreSQL 15 |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Tokens |
| API Docs | Swagger/OpenAPI |

### Supporting Tools

| Category | Technology |
| --- | --- |
| Testing | xUnit, Moq, FluentAssertions |
| Code Quality | StyleCop, SonarQube |
| Containerization | Docker, Docker Compose |
| CI/CD | GitHub Actions |
| Monitoring | Prometheus, Grafana |

## System Architecture

### Clean Architecture Layers

```
📦 DigitalWallet
├── 📂 Domain
│   ├── Entities
│   ├── Interfaces
│   ├── Enums
│   └── Exceptions
├── 📂 Application
│   ├── DTOs
│   ├── Services
│   ├── Mappings
│   └── Features
├── 📂 Infrastructure
│   ├── Data
│   ├── Repositories
│   ├── Identity
│   └── ExternalServices
└── 📂 API
    ├── Controllers
    ├── Middleware
    ├── Filters
    └── Configuration

```

### Data Flow

1. HTTP Request → API Layer (Controllers)
2. → Application Layer (Services, DTOs)
3. → Domain Layer (Business Rules)
4. → Infrastructure Layer (Persistence)
5. → Database

## Use Cases

### Authentication

| UC-01 | User Registration |
| --- | --- |
| Actor | New User |
| Preconditions | Email not already registered |
| Flow | 1. User provides email and password<br>2. System validates input<br>3. System creates new user with hashed password<br>4. System creates empty wallet<br>5. System returns success response |
| Postconditions | New user and wallet created |

| UC-02 | User Login |
| --- | --- |
| Actor | Registered User |
| Preconditions | User exists |
| Flow | 1. User provides credentials<br>2. System verifies credentials<br>3. System generates JWT token<br>4. System returns token |
| Postconditions | User receives access token |

### Wallet Operations

| UC-03 | Check Balance |
| --- | --- |
| Actor | Authenticated User |
| Preconditions | User has wallet |
| Flow | 1. User requests balance<br>2. System retrieves wallet<br>3. System returns balance |
| Postconditions | - |

| UC-04 | Deposit Funds |
| --- | --- |
| Actor | Authenticated User |
| Preconditions | User has wallet |
| Flow | 1. User specifies amount<br>2. System validates amount<br>3. System updates balance<br>4. System records transaction<br>5. System returns new balance |
| Postconditions | Wallet balance increased |

| UC-05 | Transfer Funds |
| --- | --- |
| Actor | Authenticated User |
| Preconditions | Sufficient balance, recipient exists |
| Flow | 1. User specifies recipient and amount<br>2. System validates inputs<br>3. System deducts from sender<br>4. System adds to recipient<br>5. System records transaction<br>6. System returns transaction ID |
| Postconditions | Balances updated, transaction recorded |

## API Endpoints

### Authentication

| Method | Endpoint | Description | Request Body |
| --- | --- | --- | --- |
| POST | /api/auth/register | Register new user | `{email, password}` |
| POST | /api/auth/login | Authenticate user | `{email, password}` |

### Wallet

| Method | Endpoint | Description | Parameters/Headers/Body |
| --- | --- | --- | --- |
| GET | /api/wallet/balance | Get current balance | `Authorization: Bearer <token>` |
| POST | /api/wallet/deposit | Add funds to wallet | `{amount: decimal}` |
| POST | /api/wallet/transfer | Transfer to another user | `{recipientEmail: string, amount: decimal}` |

### Transactions

| Method | Endpoint | Description | Parameters |
| --- | --- | --- | --- |
| GET | /api/transactions | Get user transactions | `?from=date&to=date&page=1&size=10` |
| GET | /api/transactions/{id} | Get transaction details | - |

## Data Schemas

### User

```csharp
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Wallet Wallet { get; set; }
    public ICollection<Transaction> SentTransactions { get; set; }
    public ICollection<Transaction> ReceivedTransactions { get; set; }
}

```

### Wallet

```csharp
public class Wallet
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }

    // Foreign key
    public Guid UserId { get; set; }
    public User User { get; set; }
}

```

### Transaction

```csharp
public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public TransactionStatus Status { get; set; }

    // Foreign keys
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }

    // Navigation properties
    public User Sender { get; set; }
    public User Recipient { get; set; }
}

```

## Authentication

### JWT Flow

1. Client sends credentials to `/api/auth/login`
2. Server validates and returns JWT
3. Client includes token in `Authorization: Bearer <token>` header
4. Server validates token for protected routes

### Security Features

- Password hashing with PBKDF2
- JWT expiration (60 minutes)
- Refresh token support
- Rate limiting on auth endpoints

## Database Design

### Tables

**Users**

```
id: UUID (PK)
email: VARCHAR(255) (UNIQUE)
password_hash: TEXT
created_at: TIMESTAMPTZ
updated_at: TIMESTAMPTZ

```

**Wallets**

```
id: UUID (PK)
balance: DECIMAL(18,2)
last_updated: TIMESTAMPTZ
user_id: UUID (FK to Users)

```

**Transactions**

```
id: UUID (PK)
amount: DECIMAL(18,2)
description: TEXT
timestamp: TIMESTAMPTZ
status: SMALLINT
sender_id: UUID (FK to Users)
recipient_id: UUID (FK to Users)

```

### Indexes

- Users.email (UNIQUE)
- Transactions.sender_id
- Transactions.recipient_id
- Transactions.timestamp

## Setup & Deployment

### Development Setup

```bash
# Clone repository
git clone <https://github.com/yourrepo/digital-wallet-api.git>

# Restore dependencies
dotnet restore

# Configure database connection in appsettings.Development.json

# Run migrations
dotnet ef database update

# Start application
dotnet run

```

### Docker Deployment

```
# Build
docker-compose build

# Start
docker-compose up -d

# View logs
docker-compose logs -f api

```

### Environment Variables

| Variable | Required | Default | Description |
| --- | --- | --- | --- |
| DB_CONNECTION_STRING | Yes | - | PostgreSQL connection |
| JWT_SECRET | Yes | - | JWT signing key |
| JWT_EXPIRE_MINUTES | No | 60 | Token validity |

## Testing Strategy
### Test Pyramid
- **Unit Tests**: 70% coverage (Domain/Application layers)
- **Integration Tests**: 20% coverage (API/DB interactions)
- **E2E Tests**: 10% coverage (Critical user flows)
