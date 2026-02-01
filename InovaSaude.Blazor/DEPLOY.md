# 🚀 Deploy InovaSaúde Blazor

## Opções de Deploy

### 1. IIS (Windows Server)

#### Pré-requisitos
- Windows Server 2016 ou superior
- IIS 10 ou superior
- .NET 8 Hosting Bundle

#### Passos

1. **Instalar .NET 8 Hosting Bundle**
```powershell
# Download do site oficial Microsoft
https://dotnet.microsoft.com/download/dotnet/8.0
```

2. **Publicar a Aplicação**
```bash
cd InovaSaude.Blazor
dotnet publish -c Release -o ./publish
```

3. **Configurar IIS**
- Criar novo Application Pool (.NET CLR Version = No Managed Code)
- Criar novo Site apontando para a pasta `publish`
- Configurar permissões para o Application Pool Identity

4. **Ajustar Connection String**
Edite `appsettings.json` na pasta publish:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=InovaSaude;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True"
  }
}
```

---

### 2. Azure App Service

#### Passos

1. **Criar App Service**
```bash
az webapp create --resource-group InovaSaude-RG --plan InovaSaude-Plan --name inovasaude-app --runtime "DOTNET:8.0"
```

2. **Configurar Connection String**
```bash
az webapp config connection-string set --resource-group InovaSaude-RG --name inovasaude-app --connection-string-type SQLAzure --settings DefaultConnection="Server=..."
```

3. **Deploy**
```bash
dotnet publish -c Release
cd bin/Release/net8.0/publish
az webapp deployment source config-zip --resource-group InovaSaude-RG --name inovasaude-app --src ./publish.zip
```

---

### 3. Docker

#### Dockerfile já está pronto!

**Localização:** `InovaSaude.Blazor/Dockerfile`

#### Build e Run

```bash
# Build da imagem
docker build -t inovasaude-blazor .

# Executar container
docker run -d -p 5000:80 -e ConnectionStrings__DefaultConnection="Server=..." --name inovasaude inovasaude-blazor
```

#### Docker Compose

```yaml
version: '3.8'
services:
  app:
    build: ./InovaSaude.Blazor
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=InovaSaude;User=sa;Password=YourPassword123!
    depends_on:
      - sqlserver
      
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"
```

---

### 4. Linux (Ubuntu/Debian) com Nginx

#### 1. Instalar .NET 8
```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

#### 2. Publicar Aplicação
```bash
dotnet publish -c Release -o /var/www/inovasaude
```

#### 3. Criar Service
```bash
sudo nano /etc/systemd/system/inovasaude.service
```

Conteúdo:
```ini
[Unit]
Description=InovaSaude Blazor App

[Service]
WorkingDirectory=/var/www/inovasaude
ExecStart=/usr/bin/dotnet /var/www/inovasaude/InovaSaude.Blazor.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=inovasaude
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

#### 4. Configurar Nginx
```nginx
server {
    listen 80;
    server_name seu-dominio.com.br;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

#### 5. Iniciar Serviço
```bash
sudo systemctl enable inovasaude
sudo systemctl start inovasaude
sudo systemctl restart nginx
```

---

### 5. AWS (EC2 + RDS)

#### 1. Criar RDS SQL Server
- Instância db.t3.medium ou superior
- SQL Server Express ou Standard
- Anotar endpoint da conexão

#### 2. Criar EC2
- Windows Server 2022 ou Ubuntu 22.04
- t3.medium ou superior
- Instalar .NET 8

#### 3. Deploy
Usar método IIS (Windows) ou Nginx (Linux)

---

## 📝 Checklist Pré-Deploy

### Segurança
- [ ] Alterar senha do admin
- [ ] Configurar HTTPS/SSL
- [ ] Configurar CORS adequadamente
- [ ] Revisar permissões de banco
- [ ] Configurar firewall

### Performance
- [ ] Habilitar compressão
- [ ] Configurar cache
- [ ] Otimizar queries do banco
- [ ] Configurar connection pool

### Monitoramento
- [ ] Configurar logs
- [ ] Configurar alertas
- [ ] Configurar backup automático
- [ ] Configurar health checks

### Configurações
- [ ] Atualizar connection string
- [ ] Configurar SMTP (emails)
- [ ] Configurar storage (uploads)
- [ ] Ajustar timeouts

---

## 🔧 Configurações de Produção

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=InovaSaude;User Id=inovasaude_user;Password=SENHA_FORTE;Encrypt=true;TrustServerCertificate=false"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Limits": {
      "MaxRequestBodySize": 52428800
    }
  }
}
```

---

## 🗄️ Backup do Banco de Dados

### SQL Server
```sql
BACKUP DATABASE InovaSaude
TO DISK = 'C:\Backups\InovaSaude_Full.bak'
WITH FORMAT, MEDIANAME = 'InovaSaudeBackup', NAME = 'Full Backup';
```

### Automatizar Backup (SQL Agent Job)
```sql
USE msdb;
GO

EXEC sp_add_job @job_name = 'InovaSaude Daily Backup';

EXEC sp_add_jobstep
    @job_name = 'InovaSaude Daily Backup',
    @step_name = 'Backup Database',
    @command = 'BACKUP DATABASE InovaSaude TO DISK = ''C:\Backups\InovaSaude_Full.bak'' WITH FORMAT';

EXEC sp_add_schedule
    @schedule_name = 'Daily at 2AM',
    @freq_type = 4,
    @freq_interval = 1,
    @active_start_time = 020000;

EXEC sp_attach_schedule
    @job_name = 'InovaSaude Daily Backup',
    @schedule_name = 'Daily at 2AM';
```

---

## 📊 Monitoramento

### Application Insights (Azure)

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Logs Customizados

```csharp
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    },
    "File": {
      "Path": "logs/inovasaude-.txt",
      "Append": true,
      "MinLevel": "Warning",
      "FileSizeLimitBytes": 10485760,
      "MaxRollingFiles": 10
    }
  }
}
```

---

## 🔐 SSL/HTTPS

### Certificado Let's Encrypt (Linux)

```bash
sudo apt-get install certbot python3-certbot-nginx
sudo certbot --nginx -d seu-dominio.com.br
```

### IIS (Windows)
1. Comprar certificado SSL ou usar Let's Encrypt
2. Importar certificado no IIS
3. Adicionar binding HTTPS no site
4. Forçar redirect HTTP → HTTPS

---

## 🚦 Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

app.MapHealthChecks("/health");
```

---

## 📈 Escalabilidade

### Load Balancer
- Usar Azure Load Balancer ou AWS ELB
- Configurar sticky sessions para Blazor Server
- Múltiplas instâncias da aplicação

### Redis Cache
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis-server:6379";
});
```

---

## ⚠️ Troubleshooting

### Erro: 502 Bad Gateway
**Causa:** Aplicação não está rodando  
**Solução:** Verificar se o serviço está ativo

### Erro: Connection timeout
**Causa:** Firewall bloqueando  
**Solução:** Liberar portas necessárias

### Erro: Out of memory
**Causa:** Memory leak ou carga alta  
**Solução:** Aumentar RAM ou otimizar código

---

## 📞 Contatos

Para suporte com deploy, contate a equipe de DevOps.

---

**Última atualização:** Fevereiro 2026
