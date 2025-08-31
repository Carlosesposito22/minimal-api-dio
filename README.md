# Minimal API para Gerenciamento de Veículos

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8-512BD4)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-8-512BD4)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-red)
![JWT](https://img.shields.io/badge/JWT-Authentication-green)
![Swagger](https://img.shields.io/badge/Swagger-API_Docs-85EA2D?logo=swagger)

API RESTful desenvolvida com ASP.NET 8 e o padrão Minimal API para o gerenciamento de veículos e administradores. A aplicação implementa um sistema de autenticação e autorização seguro utilizando JSON Web Tokens (JWT) e controle de acesso baseado em papéis (Roles).

## ✨ Funcionalidades

- **Autenticação e Autorização**: Sistema de login seguro com geração de token JWT.
- **Controle de Acesso por Papel (Role-Based)**: Endpoints protegidos com diferentes níveis de acesso para `Admin` e `Editor`.
- **Gerenciamento de Administradores (CRUD)**: Operações completas de criação, leitura, atualização e exclusão de usuários administradores (acessível apenas para `Admin`).
- **Gerenciamento de Veículos (CRUD)**: Operações completas para gerenciar os veículos, com permissões distintas por papel.
- **Documentação da API**: Geração automática de documentação interativa com Swagger/OpenAPI.

## 📍Endpoints da API

### Autenticação
- `POST /login`: Realiza o login e retorna um token JWT.

### Admin
- `GET /admins`: Lista todos os administradores.
- `GET /admin/{id}`: Busca um administrador por ID.
- `POST /admin`: Cria um novo administrador.

### Veículo
- `GET /veiculos`: Lista todos os veículos (Requer Role = "Admin" ou "Editor").
- `GET /veiculo/{id}`: Busca um veículo por ID (Requer Role = "Admin" ou "Editor").
- `POST /veiculo`: Cria um novo veículo (Requer Role = "Admin" ou "Editor").
- `PUT /veiculo/{id}`: Atualiza um veículo existente (Requer Role = "Admin").
- `DELETE /veiculo/{id}`: Exclui um veículo (Requer Role = "Admin").
