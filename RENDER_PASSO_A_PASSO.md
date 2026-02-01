# 🎯 PASSO A PASSO VISUAL - CONFIGURAR RENDER

## 1. DELETAR SERVIÇOS ANTIGOS

```
Dashboard → Services → inovasaude-backend-2
└── Settings (⚙️)
    └── Scroll até o final
        └── Delete Service
            └── Digite o nome: inovasaude-backend-2
                └── Confirmar

Dashboard → Services → inovasaude-frontend-static
└── Settings (⚙️)
    └── Scroll até o final
        └── Delete Service
            └── Digite o nome: inovasaude-frontend-static
                └── Confirmar
```

---

## 2. CRIAR SERVIÇO BLAZOR

```
Dashboard → New + → Web Service
└── Connect Repository
    └── GitHub → Benjamim0259/InovaSaude → Connect
        └── Configurar:
            
            Name: inovasaude-blazor
            Region: Oregon (US West)
            Branch: main
            Root Directory: InovaSaude.Blazor  ⬅️ IMPORTANTE!
            Runtime: Docker ⬅️ IMPORTANTE!
            
            Instance Type: Free
            
            Environment Variables:
            ├── ASPNETCORE_ENVIRONMENT = Production
            └── ASPNETCORE_URLS = http://0.0.0.0:5163
            
            Database:
            └── Link to existing: inovasaude-db ⬅️ IMPORTANTE!
            
            └── Create Web Service
```

---

## 3. AGUARDAR BUILD

```
Render vai executar:
├── 📥 Clone do repo
├── 📂 Entrar em InovaSaude.Blazor/
├── 🐳 docker build (usando Dockerfile)
│   ├── dotnet restore
│   ├── dotnet build
│   └── dotnet publish
├── 🚀 Iniciar container
├── 🗄️ Conectar PostgreSQL (variáveis automáticas)
└── ✅ Online em https://inovasaude-blazor.onrender.com
```

**Tempo estimado:** 5-8 minutos (primeira vez)

---

## 4. TESTAR

```
URL: https://inovasaude-blazor.onrender.com

Login:
├── Email: admin@inovasaude.com.br
└── Senha: Admin@123

Páginas funcionais:
├── ✅ Dashboard
├── ✅ Gerenciar UBS
├── ✅ Gerenciar Despesas
└── ✅ Gerar Relatórios (com gráficos e exportação!)
```

---

## ⚠️ PROBLEMAS COMUNS:

### Erro: "Failed to connect to database"
**Solução:** Rodar migrations no Shell do Render:
```bash
dotnet ef database update
```

### Erro: "Root directory not found"
**Solução:** Verificar se Root Directory = `InovaSaude.Blazor` (sem barra no final)

### Erro: "Docker build failed"
**Solução:** Verificar nos logs se tem erro de compilação. Se tiver, me avise!

---

**🎉 PRONTO! Depois disso o sistema vai estar 100% funcional!**
