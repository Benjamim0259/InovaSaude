# API Documentation - InovaSaúde

## Base URL

```
http://localhost:4000/api
```

## Autenticação

A maioria dos endpoints requer autenticação via JWT. Após o login, inclua o token no header:

```
Authorization: Bearer <token>
```

## Endpoints

### 🔐 Autenticação

#### Login

```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@inovasaude.com.br",
  "senha": "admin123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "id": "uuid",
    "nome": "Administrador",
    "email": "admin@inovasaude.com.br",
    "perfil": "ADMIN",
    "ubs": null
  }
}
```

#### Registro (apenas ADMIN)

```http
POST /auth/register
Authorization: Bearer <token>
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@example.com",
  "senha": "senha123",
  "perfil": "COORDENADOR",
  "ubsId": "uuid-da-ubs"
}
```

#### Logout

```http
POST /auth/logout
Authorization: Bearer <token>
```

#### Recuperar Senha

```http
POST /auth/forgot-password
Content-Type: application/json

{
  "email": "usuario@example.com"
}
```

### 💰 Despesas

#### Listar Despesas

```http
GET /despesas?page=1&limit=10&ubsId=uuid&status=PENDENTE
Authorization: Bearer <token>
```

**Query Parameters:**
- `page` (opcional): Página (padrão: 1)
- `limit` (opcional): Itens por página (padrão: 10, máx: 100)
- `ubsId` (opcional): Filtrar por UBS
- `categoriaId` (opcional): Filtrar por categoria
- `fornecedorId` (opcional): Filtrar por fornecedor
- `status` (opcional): PENDENTE, APROVADA, PAGA, REJEITADA, CANCELADA
- `dataInicio` (opcional): Data inicial (ISO 8601)
- `dataFim` (opcional): Data final (ISO 8601)

**Resposta:**
```json
{
  "despesas": [...],
  "total": 100,
  "page": 1,
  "limit": 10,
  "totalPages": 10
}
```

#### Buscar Despesa por ID

```http
GET /despesas/:id
Authorization: Bearer <token>
```

#### Criar Despesa

```http
POST /despesas
Authorization: Bearer <token>
Content-Type: application/json

{
  "descricao": "Material de limpeza",
  "valor": 1500.00,
  "dataVencimento": "2024-01-31T00:00:00.000Z",
  "categoriaId": "uuid",
  "tipo": "VARIAVEL",
  "ubsId": "uuid",
  "fornecedorId": "uuid",
  "numeroNota": "NF-001",
  "observacoes": "Compra mensal"
}
```

**Permissões:** ADMIN, COORDENADOR, GESTOR

#### Atualizar Despesa

```http
PUT /despesas/:id
Authorization: Bearer <token>
Content-Type: application/json

{
  "descricao": "Material de limpeza - Atualizado",
  "valor": 1600.00,
  "status": "APROVADA"
}
```

**Permissões:** ADMIN, COORDENADOR, GESTOR

#### Deletar Despesa

```http
DELETE /despesas/:id
Authorization: Bearer <token>
```

**Permissões:** ADMIN, COORDENADOR

#### Aprovar Despesa

```http
POST /despesas/:id/aprovar
Authorization: Bearer <token>
```

**Permissões:** ADMIN, GESTOR

#### Rejeitar Despesa

```http
POST /despesas/:id/rejeitar
Authorization: Bearer <token>
Content-Type: application/json

{
  "observacao": "Motivo da rejeição"
}
```

**Permissões:** ADMIN, GESTOR

#### Marcar como Paga

```http
POST /despesas/:id/pagar
Authorization: Bearer <token>
Content-Type: application/json

{
  "dataPagamento": "2024-01-15T00:00:00.000Z"
}
```

**Permissões:** ADMIN, GESTOR

### 🏥 UBS

#### Listar UBS

```http
GET /ubs?page=1&limit=10&status=ATIVA
Authorization: Bearer <token>
```

#### Buscar UBS por ID

```http
GET /ubs/:id
Authorization: Bearer <token>
```

#### Criar UBS

```http
POST /ubs
Authorization: Bearer <token>
Content-Type: application/json

{
  "nome": "UBS Centro",
  "codigo": "UBS-001",
  "endereco": "Rua Principal, 100",
  "bairro": "Centro",
  "cep": "12345-678",
  "telefone": "(11) 3333-1111",
  "email": "ubs.centro@municipio.gov.br",
  "coordenadorId": "uuid",
  "capacidadeAtendimento": 1000
}
```

**Permissões:** ADMIN, GESTOR

#### Atualizar UBS

```http
PUT /ubs/:id
Authorization: Bearer <token>
Content-Type: application/json

{
  "nome": "UBS Centro - Atualizado",
  "status": "ATIVA"
}
```

**Permissões:** ADMIN, GESTOR

#### Deletar UBS

```http
DELETE /ubs/:id
Authorization: Bearer <token>
```

**Permissões:** ADMIN

### 👥 Usuários

#### Listar Usuários

```http
GET /usuarios?page=1&limit=10&perfil=COORDENADOR
Authorization: Bearer <token>
```

**Permissões:** ADMIN

#### Buscar Usuário por ID

```http
GET /usuarios/:id
Authorization: Bearer <token>
```

**Permissões:** ADMIN, GESTOR

#### Criar Usuário

```http
POST /usuarios
Authorization: Bearer <token>
Content-Type: application/json

{
  "nome": "Maria Silva",
  "email": "maria@example.com",
  "senha": "senha123",
  "perfil": "COORDENADOR",
  "telefone": "(11) 98888-1111",
  "ubsId": "uuid"
}
```

**Permissões:** ADMIN

#### Atualizar Usuário

```http
PUT /usuarios/:id
Authorization: Bearer <token>
Content-Type: application/json

{
  "nome": "Maria Silva Santos",
  "status": "ATIVO"
}
```

**Permissões:** ADMIN

#### Deletar Usuário

```http
DELETE /usuarios/:id
Authorization: Bearer <token>
```

**Permissões:** ADMIN

### 📊 Relatórios

#### Dashboard

```http
GET /relatorios/dashboard?ubsId=uuid&dataInicio=2024-01-01&dataFim=2024-01-31
Authorization: Bearer <token>
```

**Resposta:**
```json
{
  "totalGeral": {
    "_sum": { "valor": 50000 },
    "_count": { "id": 150 }
  },
  "despesasPorStatus": [...],
  "despesasPorCategoria": [...]
}
```

#### Gastos por UBS

```http
GET /relatorios/gastos-por-ubs?dataInicio=2024-01-01&dataFim=2024-01-31
Authorization: Bearer <token>
```

#### Gastos por Categoria

```http
GET /relatorios/gastos-por-categoria?ubsId=uuid&dataInicio=2024-01-01
Authorization: Bearer <token>
```

#### Comparativo Mensal

```http
GET /relatorios/comparativo-mensal?ubsId=uuid&ano=2024
Authorization: Bearer <token>
```

### 📥 Importação

#### Upload de Arquivo

```http
POST /importacao/upload
Authorization: Bearer <token>
Content-Type: multipart/form-data

file: <arquivo.csv ou arquivo.xlsx>
```

**Permissões:** ADMIN, GESTOR

#### Download Template

```http
GET /importacao/template
Authorization: Bearer <token>
```

#### Listar Lotes

```http
GET /importacao/lotes
Authorization: Bearer <token>
```

**Permissões:** ADMIN, GESTOR

## Status Codes

- `200 OK`: Requisição bem-sucedida
- `201 Created`: Recurso criado com sucesso
- `400 Bad Request`: Dados inválidos
- `401 Unauthorized`: Não autenticado ou token inválido
- `403 Forbidden`: Sem permissão para acessar o recurso
- `404 Not Found`: Recurso não encontrado
- `500 Internal Server Error`: Erro interno do servidor

## Modelos de Dados

### Usuario

```typescript
{
  id: string;
  nome: string;
  email: string;
  perfil: 'ADMIN' | 'COORDENADOR' | 'GESTOR' | 'AUDITOR';
  status: 'ATIVO' | 'INATIVO' | 'BLOQUEADO';
  telefone?: string;
  ubsId?: string;
  ultimoAcesso?: Date;
  createdAt: Date;
  updatedAt: Date;
}
```

### UBS

```typescript
{
  id: string;
  nome: string;
  codigo: string;
  endereco?: string;
  bairro?: string;
  cep?: string;
  telefone?: string;
  email?: string;
  coordenadorId?: string;
  status: 'ATIVA' | 'INATIVA' | 'EM_MANUTENCAO';
  capacidadeAtendimento?: number;
  observacoes?: string;
  createdAt: Date;
  updatedAt: Date;
}
```

### Despesa

```typescript
{
  id: string;
  descricao: string;
  valor: number;
  dataVencimento?: Date;
  dataPagamento?: Date;
  categoriaId: string;
  tipo: 'FIXA' | 'VARIAVEL' | 'EVENTUAL';
  status: 'PENDENTE' | 'APROVADA' | 'PAGA' | 'REJEITADA' | 'CANCELADA';
  ubsId: string;
  fornecedorId?: string;
  usuarioCriacaoId: string;
  usuarioAprovacaoId?: string;
  dataAprovacao?: Date;
  observacoes?: string;
  numeroNota?: string;
  numeroEmpenho?: string;
  createdAt: Date;
  updatedAt: Date;
}
```

## Exemplos de Uso

### Fluxo Completo: Criar e Aprovar Despesa

1. **Login**
```bash
curl -X POST http://localhost:4000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@inovasaude.com.br","senha":"admin123"}'
```

2. **Criar Despesa**
```bash
curl -X POST http://localhost:4000/api/despesas \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "descricao": "Material médico",
    "valor": 2500.00,
    "categoriaId": "uuid",
    "tipo": "VARIAVEL",
    "ubsId": "uuid"
  }'
```

3. **Aprovar Despesa**
```bash
curl -X POST http://localhost:4000/api/despesas/<id>/aprovar \
  -H "Authorization: Bearer <token>"
```

4. **Marcar como Paga**
```bash
curl -X POST http://localhost:4000/api/despesas/<id>/pagar \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"dataPagamento":"2024-01-15T00:00:00.000Z"}'
```
