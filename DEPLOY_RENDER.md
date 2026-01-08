# 🚀 Guia de Deploy no Render

Este guia explica como fazer deploy da aplicação InovaSaúde no Render free tier.

## Pré-requisitos

- Conta no [Render](https://render.com) (gratuita)
- Repositório Git com o código (GitHub, GitLab, etc)

## ✅ Checklist Antes do Deploy

- [ ] Enviar código para o repositório Git
- [ ] Revisar variáveis de ambiente em `.env.example`
- [ ] Garantir que `render.yaml` está na raiz do projeto
- [ ] Testar localmente com `npm run build` no frontend e backend

## 📋 Passo a Passo

### 1. Preparar o Repositório

```bash
# Certifique-se de que o código está sincronizado
git add .
git commit -m "Preparar para deploy no Render"
git push origin main
```

### 2. Conectar ao Render

1. Acesse [render.com](https://render.com)
2. Faça login ou crie uma conta
3. Clique em "New +" → "Web Service"
4. Selecione seu repositório
5. Preencha as informações:
   - **Name:** `inovasaude-backend`
   - **Branch:** `main`
   - **Root Directory:** `backend`
   - **Build Command:** `npm install && npm run build`
   - **Start Command:** `npm start`
   - **Environment:** `Node`
   - **Plan:** `Free`

### 3. Configurar Variáveis de Ambiente

Acesse a seção "Environment" e adicione:

```
DATABASE_URL=sua-url-aqui (será fornecida automaticamente)
JWT_SECRET=gere-uma-chave-aleatória-segura-aqui
NODE_ENV=production
CORS_ORIGINS=https://seu-frontend-url.onrender.com
```

### 4. Deploy do Frontend

1. Crie outro Web Service para o frontend
2. Preencha:
   - **Name:** `inovasaude-frontend`
   - **Root Directory:** `frontend`
   - **Build Command:** `npm install && npm run build`
   - **Start Command:** `npx serve -s dist -l 3000`

3. Adicione variável de ambiente:
   ```
   VITE_API_URL=https://seu-backend-url.onrender.com/api
   ```

### 5. Criar Banco de Dados

1. Na dashboard do Render, clique em "New +" → "PostgreSQL"
2. Configure:
   - **Name:** `inovasaude-db`
   - **Database:** `inovasaude`
   - **Plan:** `Free`

3. Copie a `Internal Database URL` e use como `DATABASE_URL` no backend

## ⚠️ Limitações do Render Free Tier

- **Recursos limitados:** 0.5 CPU, 512MB RAM
- **Spins down:** Aplicação hiberna após 15 minutos sem tráfego
- **Startup lento:** Primeira requisição pode levar 30 segundos
- **Sem backup automático:** Banco de dados PostgreSQL é deletado a cada 90 dias
- **1GB de disco:**limite de armazenamento

## 🔧 Otimizações Implementadas

Para funcionar bem no free tier:

✅ **Retry Automático:** As requisições têm retry exponencial  
✅ **Loading States:** Indicadores visuais enquanto aguarda  
✅ **Timeout Aumentado:** 30 segundos para requisições  
✅ **Minified Build:** Frontend é otimizado para produção  
✅ **Database Connection Pooling:** Gerencia conexões eficientemente  

## 🧪 Testando Antes de Deploy

### Frontend
```bash
cd frontend
npm install
npm run build
npm run preview
```

### Backend
```bash
cd backend
npm install
npm run build
npm start
```

## 📊 Monitorar Aplicação

1. Acesse a dashboard do Render
2. Clique no serviço para ver logs
3. Verifique a seção "Events" para erros de deployment

## 🆘 Troubleshooting

### Erro: "Build failed"
- Verifique os logs
- Certifique-se de que `package.json` existe na raiz do diretório
- Teste o build localmente

### Erro: "Application failed to start"
- Verifique as variáveis de ambiente
- Certifique-se de que `npm start` é válido
- Verifique as dependências em `package.json`

### Erro: "Cannot connect to database"
- Copie a URL correta do PostgreSQL
- Aguarde 2-3 minutos para o banco ficar pronto
- Verifique se a variável `DATABASE_URL` está definida

### Erro: "CORS error"
- Atualize `CORS_ORIGINS` com a URL correta do frontend
- Reinicie o serviço do backend

## 📱 Acessar Aplicação

Após o deploy bem-sucedido:

- **Frontend:** `https://seu-frontend-url.onrender.com`
- **Backend API:** `https://seu-backend-url.onrender.com/api`
- **Health Check:** `https://seu-backend-url.onrender.com/health`

## 🔐 Segurança

⚠️ **Importante:**
- Nunca commit `.env` com valores reais
- Use senhas aleatórias fortes para `JWT_SECRET`
- Configure SSL/HTTPS (automático no Render)
- Mude as credenciais padrão de admin

## 📞 Suporte

Para mais informações:
- [Documentação Render](https://render.com/docs)
- [Discussões do Projeto](link-aqui)

---

**Versão:** 1.0  
**Data:** 2026-01-08  
**Status:** ✅ Pronto para Producção
