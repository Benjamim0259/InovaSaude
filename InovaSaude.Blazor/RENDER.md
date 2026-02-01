# InovaSaúde no Render - Guia Completo

## 🚀 Deploy Rápido no Render

### 1️⃣ Preparar o Projeto

O projeto já está pronto para deploy! Basta seguir os passos abaixo.

### 2️⃣ Criar Conta no Render

1. Acesse: https://render.com
2. Faça login com GitHub
3. Autorize o Render a acessar seus repositórios

### 3️⃣ Criar Banco de Dados PostgreSQL

**Opção 1: Usar PostgreSQL no Render (Recomendado)**

1. No dashboard do Render, clique em **"New +"**
2. Selecione **"PostgreSQL"**
3. Configure:
   - **Name:** `inovasaude-db`
   - **Database:** `inovasaude`
   - **User:** `inovasaude_user`
   - **Region:** Oregon (US West) ou Frankfurt (EU)
   - **Plan:** Free (desenvolvimento) ou Paid (produção)
4. Clique em **"Create Database"**
5. **Anote a "External Database URL"** - você vai precisar!

**Opção 2: Continuar com SQL Server**

Se quiser manter SQL Server, você precisará hospedar em outro lugar (Azure, AWS, etc).

### 4️⃣ Ajustar Código para PostgreSQL

Vou criar os arquivos necessários para usar PostgreSQL no Render.

### 5️⃣ Fazer Deploy no Render

1. No dashboard do Render, clique em **"New +"**
2. Selecione **"Web Service"**
3. Conecte seu repositório GitHub
4. Configure:
   - **Name:** `inovasaude`
   - **Environment:** `Docker` ou `.NET`
   - **Region:** Same as database
   - **Branch:** `main`
   - **Build Command:** `dotnet publish -c Release -o out`
   - **Start Command:** `cd out && dotnet InovaSaude.Blazor.dll`

5. **Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:$PORT
   ConnectionStrings__DefaultConnection=<COLE_A_DATABASE_URL_AQUI>
   ```

6. Clique em **"Create Web Service"**

### 6️⃣ Configurar Domínio (Opcional)

Render te dá um domínio gratuito: `inovasaude.onrender.com`

Para domínio customizado:
1. Vá em Settings → Custom Domain
2. Adicione seu domínio
3. Configure DNS conforme instruções

---

## 📦 Arquivos de Configuração para Render

### render.yaml (Root do projeto)

```yaml
services:
  # Web Service (Blazor App)
  - type: web
    name: inovasaude
    env: docker
    region: oregon
    plan: free
    buildCommand: dotnet publish InovaSaude.Blazor/InovaSaude.Blazor.csproj -c Release -o out
    startCommand: cd out && dotnet InovaSaude.Blazor.dll
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:$PORT
      - key: ConnectionStrings__DefaultConnection
        fromDatabase:
          name: inovasaude-db
          property: connectionString
    healthCheckPath: /health

databases:
  - name: inovasaude-db
    databaseName: inovasaude
    user: inovasaude_user
    region: oregon
    plan: free
```

### Dockerfile Otimizado para Render

Já existe em `InovaSaude.Blazor/Dockerfile` mas vou otimizar:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InovaSaude.Blazor/InovaSaude.Blazor.csproj", "InovaSaude.Blazor/"]
RUN dotnet restore "InovaSaude.Blazor/InovaSaude.Blazor.csproj"
COPY . .
WORKDIR "/src/InovaSaude.Blazor"
RUN dotnet build "InovaSaude.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InovaSaude.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:$PORT
CMD ASPNETCORE_URLS=http://+:$PORT dotnet InovaSaude.Blazor.dll
```

---

## 🔄 Usar PostgreSQL no Render

### 1. Instalar Npgsql

```bash
cd InovaSaude.Blazor
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
```

### 2. Atualizar Program.cs

```csharp
// Detectar ambiente e escolher banco apropriado
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var isPostgres = connectionString?.Contains("postgres") ?? false;

if (isPostgres)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
```

### 3. Criar appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres-host;Database=inovasaude;Username=user;Password=pass;SSL Mode=Require;Trust Server Certificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## ⚡ Deploy Rápido com Docker

### 1. Build Local

```bash
docker build -t inovasaude .
docker run -p 5000:80 -e ConnectionStrings__DefaultConnection="..." inovasaude
```

### 2. Push para Docker Hub (Opcional)

```bash
docker tag inovasaude seuusuario/inovasaude:latest
docker push seuusuario/inovasaude:latest
```

### 3. Configurar no Render

Use a imagem Docker no Render ao invés de build.

---

## 🔐 Variáveis de Ambiente no Render

Configure estas variáveis no dashboard do Render:

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
ConnectionStrings__DefaultConnection=<DATABASE_URL>
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
```

---

## ✅ Checklist de Deploy

- [ ] Conta criada no Render
- [ ] Banco de dados PostgreSQL criado
- [ ] Código ajustado para PostgreSQL
- [ ] Dockerfile configurado
- [ ] render.yaml criado
- [ ] Web Service criado no Render
- [ ] Environment variables configuradas
- [ ] Deploy realizado
- [ ] Health check funcionando
- [ ] Login testado
- [ ] SSL/HTTPS automático ativado

---

## 🐛 Troubleshooting

### App não inicia
- Verificar logs no Render dashboard
- Confirmar variáveis de ambiente
- Testar connection string localmente

### Erro de conexão com banco
- Verificar se database URL está correta
- Confirmar que PostgreSQL está acessível
- Checar se migrations foram aplicadas

### Erro 502 Bad Gateway
- App pode estar demorando para iniciar
- Aumentar timeout no Render
- Verificar se porta está correta ($PORT)

---

## 📊 Custos Render

### Free Tier
- ✅ 750 horas/mês de Web Service
- ✅ 90 dias de PostgreSQL free
- ✅ SSL grátis
- ⚠️ App dorme após 15min inativo

### Paid Plans
- **Starter:** $7/mês (sempre ativo)
- **Standard:** $25/mês (mais recursos)
- **Pro:** $85/mês (produção)

---

## 🚀 Deploy Automático

Configure auto-deploy:
1. Settings → Build & Deploy
2. Ative "Auto-Deploy"
3. Escolha branch (main/master)
4. Cada push faz deploy automático!

---

## 📈 Monitoramento

### Logs
```bash
# Ver logs em tempo real
render logs -f
```

### Métricas
- Dashboard → Metrics
- CPU, memória, requests
- Alertas customizáveis

---

## 🎯 Próximos Passos

1. ✅ Deploy básico funcionando
2. [ ] Configurar CI/CD com GitHub Actions
3. [ ] Adicionar Redis para cache
4. [ ] Configurar CDN para assets
5. [ ] Implementar backup automático
6. [ ] Configurar monitoring (Datadog, etc)

---

## 🔗 Links Úteis

- [Render Docs](https://render.com/docs)
- [Deploy .NET on Render](https://render.com/docs/deploy-dotnet)
- [PostgreSQL on Render](https://render.com/docs/databases)

---

**Pronto para hospedar no Render! 🚀**
