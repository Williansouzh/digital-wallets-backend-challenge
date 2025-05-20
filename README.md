# Digital Wallet API Documentation

## Table of Contents

1. [Project Overview](#project-overview)
2. [Key Features](#key-features)
3. [Technology Stack](#technology-stack)
4. [System Architecture](#system-architecture)
5. [Use Cases](#use-cases)
6. [API Reference](#api-reference)
7. [Data Models](#data-models)
8. [Authentication](#authentication)
9. [Database Schema](#database-schema)
10. [Getting Started](#getting-started)
11. [Testing Approach](#testing-approach)
12. [Roadmap](#roadmap)

## Project Overview

A secure, high-performance API for digital wallet management built with .NET 8 following Clean Architecture principles. The system enables:

- ✅ User account management
- 💰 Wallet balance operations
- 🔄 Peer-to-peer transactions
- 📊 Financial reporting
- 🔐 End-to-end encrypted operations

## Key Features

### Core Functionality
| Feature | Description | Endpoint |
|---------|-------------|----------|
| User Registration | Secure account creation | `POST /api/auth/register` |
| Wallet Funding | Deposit money to wallet | `POST /api/wallet/deposit` |
| Money Transfer | Send funds to other users | `POST /api/wallet/transfer` |
| Transaction History | View all transactions | `GET /api/transactions` |

### Security Features
- JWT Authentication with refresh tokens
- PBKDF2 password hashing
- Transaction audit logging
- Rate limiting (100 requests/minute)
- Input validation at all layers

## Technology Stack

### Backend
| Component | Technology |
|-----------|------------|
| Framework | .NET 8 |
| Web Server | Kestrel |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Tokens |

### Infrastructure
| Component | Technology |
|-----------|------------|
| Database | PostgreSQL 15 |
| Caching | Redis |
| Containerization | Docker |
| CI/CD | GitHub Actions |

### Quality Assurance
| Type | Tools |
|------|-------|
| Unit Testing | xUnit, Moq |
| Integration Testing | TestContainers |
| Code Analysis | SonarQube |
| API Docs | Swagger UI |

## System Architecture

### Clean Architecture Layers
```mermaid
graph TD
    A[API Layer] --> B[Application Layer]
    B --> C[Domain Layer]
    C --> D[Infrastructure Layer]
    D --> E[(Database)]
```

### Component Diagram
```mermaid
graph LR
    U[User] -->|HTTP| A[API Gateway]
    A --> B[Auth Service]
    A --> C[Wallet Service]
    A --> D[Transaction Service]
    B --> E[(User DB)]
    C --> F[(Wallet DB)]
    D --> G[(Transaction DB)]
```

## Use Cases

### Authentication Flow
```mermaid
sequenceDiagram
    User->>+API: POST /auth/login
    API->>+AuthService: Verify credentials
    AuthService->>+Database: Validate user
    Database-->>-AuthService: User data
    AuthService-->>-API: JWT Token
    API-->>-User: Return token
```

### Fund Transfer Process
1. **Initiation**: User submits transfer request
2. **Validation**: System checks:
   - Sufficient balance
   - Valid recipient
   - Amount > 0
3. **Execution**:
   - Debit sender's wallet
   - Credit recipient's wallet
   - Record transaction
4. **Confirmation**: Return transaction receipt

## API Reference

### Authentication
| Endpoint | Method | Description | Parameters |
|----------|--------|-------------|------------|
| `/auth/register` | POST | Create new account | `email`, `password` |
| `/auth/login` | POST | Get access token | `email`, `password` |

### Wallet Operations
| Endpoint | Method | Description | Request Body |
|----------|--------|-------------|--------------|
| `/wallet/balance` | GET | Check balance | - |
| `/wallet/deposit` | POST | Add funds | `{ amount: decimal }` |
| `/wallet/transfer` | POST | Send money | `{ recipientEmail: string, amount: decimal }` |

## Data Models

### User Entity
```csharp
public class User {
    public Guid Id { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation properties
    public Wallet Wallet { get; set; }
}
```

### Transaction DTO
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 100.50,
  "description": "Fund transfer",
  "timestamp": "2023-08-20T12:34:56Z",
  "status": "Completed"
}
```

## Authentication

### JWT Specification
| Claim | Description | Example |
|-------|-------------|---------|
| sub | User ID | `"sub": "123e4567-e89b-12d3-a456-426614174000"` |
| email | User email | `"email": "user@example.com"` |
| exp | Expiration time | `"exp": 1692546896` |

### Security Headers
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-Request-ID: 550e8400-e29b-41d4-a716-446655440000
```

## Database Schema

### Entity Relationship Diagram
```mermaid
erDiagram
    USER ||--o{ WALLET : has
    USER ||--o{ TRANSACTION : sends
    USER ||--o{ TRANSACTION : receives
    WALLET {
        Guid Id PK
        decimal Balance
        Guid UserId FK
    }
```

### Index Strategy
| Table | Indexed Columns | Type |
|-------|-----------------|------|
| Users | email | Unique |
| Transactions | timestamp | B-tree |
| Wallets | user_id | Hash |

## Getting Started

---

### ⚙️ Development Setup

#### 🔧 Variáveis de Ambiente

Essas variáveis são necessárias para configurar corretamente o ambiente de desenvolvimento da aplicação:

| Variável                 | Descrição                                   | Exemplo                                                      |
| ------------------------ | ------------------------------------------- | ------------------------------------------------------------ |
| `ASPNETCORE_ENVIRONMENT` | Define o ambiente de execução da aplicação  | `Development`                                                |
| `DB_CONNECTION_STRING`   | String de conexão com o banco de dados      | `Host=db;Database=wallets;Username=postgres;Password=123456` |
| `JWT_SECRET`             | Chave secreta usada para assinar tokens JWT | `your-secure-key-here`                                       |

#### 🧪 Exemplo de `appsettings.json`

Se estiver usando o `appsettings.json` para configurar sua aplicação, aqui está um exemplo completo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pethelp_db;Username=pethelp_user;Password=secret123"
  },
  "Jwt": {
    "SecretKey": "ThisIsASecretKeyForJwtTokenDontShare",
    "Issuer": "YourApp",
    "Audience": "YourAppUsers"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

#### 🧰 Passo a Passo para Executar Localmente

```bash
# 1. Clone o repositório
git clone https://github.com/Williansouzh/digital-wallets-backend-challenge.git
cd digital-wallets-backend-challenge

# 2. Configure as variáveis de ambiente
cp .env.example .env
nano .env  # Atualize os valores conforme necessário

# 3. Inicie os serviços (banco de dados, app etc.)
docker-compose up -d

# 4. Aplique as migrations no banco de dados
dotnet ef database update
```
### Deployment Options
1. **Docker Swarm**
   ```bash
   docker stack deploy -c docker-compose.prod.yml wallet
   ```
2. **Kubernetes**
   ```bash
   kubectl apply -f k8s/deployment.yaml
   ```

## Testing Approach

### Test Coverage
| Layer | Coverage Target | Tools |
|-------|-----------------|-------|
| Unit | 80%+ | xUnit |
| Integration | 70%+ | Postman |
| E2E | Critical Paths | Cypress |

```

## Roadmap

### Upcoming Features
- Q3 2023: Recurring payments
- Q4 2023: Currency exchange
- Q1 2024: Merchant API

### Optimization Goals
- Reduce transaction latency to <200ms
- Achieve 99.99% availability
- Support 10,000 TPS

---
