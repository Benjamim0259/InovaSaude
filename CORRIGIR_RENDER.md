# 🚨 INSTRUÇÕES URGENTES - CORRIGIR RENDER

## PROBLEMA:
O Render ainda está tentando fazer deploy dos serviços antigos (backend/frontend TypeScript) que foram deletados.

## SOLUÇÃO EM 3 PASSOS:

### 1️⃣ DELETAR SERVIÇOS ANTIGOS NO DASHBOARD:

**Acesse:** https://dashboard.render.com

**Delete estes 2 serviços:**
- `inovasaude-backend-2` (Node.js)
- `inovasaude-frontend-static` (Static)

**Como deletar:**
1. Clique no serviço
2. Vá em "Settings" (engrenagem)
3. Role até o final
4. Clique em "Delete Service"
5. Confirme digitando o nome do serviço

---

### 2️⃣ CRIAR NOVO SERVIÇO BLAZOR:

**No dashboard do Render:**

1. **New +** → **Web Service**

2. **Connect Repository:**
   - Selecione: `Benjamim0259/InovaSaude`
   - Branch: `main`

3. **Configurações do Serviço:**
   ```
   Name: inovasaude-blazor
   Region: Oregon (US West) ou São Paulo (recomendado)
   Branch: main
   Root Directory: InovaSaude.Blazor
   Runtime: Docker
   ```

4. **Build & Deploy:**
   ```
   Docker Command: (deixe vazio, usa Dockerfile)
   ```

5. **Instance Type:**
   ```
   Free
   ```

6. **Environment Variables:** (IMPORTANTE!)
   Clique em "Add Environment Variable" para cada:
   
   ```
   ASPNETCORE_ENVIRONMENT = Production
   ASPNETCORE_URLS = http://0.0.0.0:5163
   ```

7. **Database:**
   - Na seção "Environment Groups"
   - Vincule ao database: `inovasaude-db`
   - Isso adiciona automaticamente variáveis DB_HOST, DB_PORT, etc.

8. **Clique em "Create Web Service"**

---

### 3️⃣ AGUARDAR BUILD (5-10 minutos):

O Render vai:
- ✅ Baixar código (commit be77dd8)
- ✅ Rodar `docker build` usando o Dockerfile
- ✅ Publicar aplicação .NET 8
- ✅ Conectar ao PostgreSQL
- ✅ Aplicação online!

**Logs em tempo real:** https://dashboard.render.com/web/[SEU-SERVICE-ID]

---

## ⚡ ALTERNATIVA RÁPIDA - BLUEPRINT:

Se preferir, pode usar o render.yaml que já está no repo:

1. Delete os 2 serviços antigos
2. No dashboard: **New** → **Blueprint**
3. Selecione o repo `Benjamim0259/InovaSaude`
4. O Render vai ler o `render.yaml` e criar tudo automaticamente!

---

## 🔍 VERIFICAR SE DEU CERTO:

Depois do deploy, teste:
- URL será: `https://inovasaude-blazor.onrender.com`
- Login: `admin@inovasaude.com.br` / `Admin@123`
- Páginas: Dashboard, UBS, Despesas, Relatórios

---

## ⚠️ SE DER ERRO DE POSTGRESQL:

Se aparecer erro de conexão com banco, execute migrations manualmente:

1. No dashboard do Render, abra o Shell do serviço
2. Execute:
   ```bash
   dotnet ef database update
   ```

---

**NÃO PRECISA COMMITAR NADA - O CÓDIGO JÁ ESTÁ CERTO!**
**É SÓ CONFIGURAR O DASHBOARD DO RENDER!**
