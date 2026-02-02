# ?? Comandos para Deploy

## 1?? Verificar alterações
```bash
git status
```

## 2?? Adicionar todos os arquivos modificados
```bash
git add .
```

## 3?? Fazer commit com mensagem descritiva
```bash
git commit -m "feat: Corrigir autenticação PostgreSQL e implementar menu dinâmico

- Adicionar conversão automática de DATABASE_URL do Render (postgres:// -> Npgsql format)
- Implementar menu lateral dinâmico baseado em autenticação
- Criar páginas públicas: Sobre e Contato
- Melhorar landing page com conteúdo diferenciado para visitantes e usuários autenticados
- Corrigir warnings de código
- Adicionar estilos melhorados para menu lateral

Fixes: Erro de login no Render (ArgumentException connection string)
Features: Menu contextual, páginas informativas, UX melhorada"
```

## 4?? Enviar para o GitHub
```bash
git push origin main
```

## 5?? Verificar deploy no Render
Acesse: https://dashboard.render.com
- O deploy será iniciado automaticamente
- Acompanhe os logs de build
- Aguarde o deploy ser concluído (status: "Live")

## 6?? Testar a aplicação
Acesse: https://inovasaude-blazor.onrender.com

### Teste o menu antes do login:
- [ ] Home mostra landing page completa
- [ ] Menu mostra: Home, Sobre, Contato, Entrar
- [ ] Página Sobre carrega corretamente
- [ ] Página Contato carrega corretamente
- [ ] Botão "Entrar" está destacado

### Teste o login:
- [ ] Acesse /login
- [ ] Use: admin@inovasaude.com.br / Admin@123
- [ ] Login é bem-sucedido (sem erros de connection string)

### Teste o menu após o login:
- [ ] Menu mostra "ÁREA ADMINISTRATIVA"
- [ ] Opções disponíveis: Dashboard, Despesas, UBS, Relatórios, Workflows, Integrações
- [ ] Botão "Entrar" desapareceu
- [ ] Botão "Sair" apareceu em vermelho
- [ ] Home mostra atalhos rápidos
- [ ] Páginas Sobre e Contato sumiram do menu

### Teste o logout:
- [ ] Clique em "Sair"
- [ ] Sistema redireciona para home
- [ ] Menu volta ao estado público
- [ ] Landing page aparece novamente

---

## ?? Em caso de problemas

### Se o build falhar no Render:
```bash
# Verifique os logs no Render Dashboard
# Procure por erros de:
# - Compilação (.NET)
# - Connection string
# - Migrations
```

### Se o login ainda não funcionar:
1. Verifique a variável `DATABASE_URL` no Render Dashboard
2. Formato esperado: `postgres://user:password@host:port/database`
3. A conversão automática deve aparecer nos logs

### Se o menu não mudar:
- Limpe o cache do navegador (Ctrl + F5)
- Verifique se a autenticação está funcionando
- Inspecione os cookies no navegador

---

## ?? Notas Importantes

?? **Primeira vez que o container iniciar após o deploy:**
- As migrations serão aplicadas automaticamente
- O usuário admin será criado (se não existir)
- Pode levar alguns segundos extras

? **O que foi corrigido:**
- Connection string PostgreSQL agora é convertida automaticamente
- Menu é dinâmico e contextual
- UX melhorada com páginas informativas
- Credenciais de teste visíveis para facilitar onboarding

?? **Próximas melhorias sugeridas:**
- Adicionar página de documentação
- Implementar recuperação de senha
- Adicionar testes automatizados
- Melhorar tratamento de erros de conexão
