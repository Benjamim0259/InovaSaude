# Contribuindo para InovaSaúde

Obrigado por considerar contribuir para o InovaSaúde! Este documento fornece diretrizes para contribuições.

## 🤝 Como Contribuir

### 1. Fork e Clone

```bash
# Fork o repositório no GitHub
# Clone seu fork
git clone https://github.com/seu-usuario/InovaSaude.git
cd InovaSaude

# Adicione o repositório original como upstream
git remote add upstream https://github.com/Benjamim0259/InovaSaude.git
```

### 2. Crie uma Branch

```bash
# Atualize sua main
git checkout main
git pull upstream main

# Crie uma branch para sua feature
git checkout -b feature/nome-da-feature
```

### 3. Faça suas Alterações

- Escreva código claro e bem documentado
- Siga as convenções de código do projeto
- Adicione testes para novas funcionalidades
- Atualize a documentação quando necessário

### 4. Commit suas Mudanças

Usamos commits semânticos:

```bash
# Formato
tipo(escopo): descrição curta

# Tipos
feat:     Nova funcionalidade
fix:      Correção de bug
docs:     Documentação
style:    Formatação (não afeta código)
refactor: Refatoração
test:     Testes
chore:    Tarefas de manutenção

# Exemplos
git commit -m "feat(despesas): adiciona filtro por categoria"
git commit -m "fix(auth): corrige validação de email"
git commit -m "docs(readme): atualiza instruções de setup"
```

### 5. Push e Pull Request

```bash
# Push para seu fork
git push origin feature/nome-da-feature

# Crie um Pull Request no GitHub
```

## 📋 Checklist do Pull Request

- [ ] Código segue os padrões do projeto
- [ ] Testes foram adicionados/atualizados
- [ ] Documentação foi atualizada
- [ ] Commits seguem convenção semântica
- [ ] Build passa sem erros
- [ ] Testes passam
- [ ] Não há conflitos com a branch main

## 💻 Padrões de Código

### TypeScript

- Use tipos explícitos sempre que possível
- Evite `any`, prefira `unknown`
- Use interfaces para objetos complexos
- Documente funções complexas

### React

- Use componentes funcionais
- Use hooks ao invés de classes
- Extraia lógica complexa em hooks customizados
- Mantenha componentes pequenos e focados

### Backend

- Siga a arquitetura em camadas (Controller → Service → Repository)
- Use DTOs para validação
- Trate erros apropriadamente
- Adicione logs para operações importantes

### Nomenclatura

- **Arquivos:** kebab-case (user-service.ts)
- **Componentes React:** PascalCase (UserCard.tsx)
- **Funções:** camelCase (getUserById)
- **Constantes:** UPPER_SNAKE_CASE (MAX_FILE_SIZE)
- **Interfaces:** PascalCase com I opcional (IUser ou User)

## 🧪 Testes

```bash
# Backend
cd backend
npm run test
npm run test:coverage

# Frontend
cd frontend
npm run test
```

### Escrever Testes

- Teste casos de sucesso e erro
- Use nomes descritivos
- Mantenha testes isolados
- Mock dependências externas

## 📝 Documentação

- Atualize README.md para mudanças significativas
- Documente APIs no código
- Adicione exemplos quando útil
- Mantenha comentários atualizados

## 🐛 Reportar Bugs

Use o template de issue para bugs:

1. Descrição clara do problema
2. Passos para reproduzir
3. Comportamento esperado
4. Comportamento atual
5. Screenshots (se aplicável)
6. Ambiente (OS, Node version, etc.)

## ✨ Sugerir Funcionalidades

Use o template de issue para features:

1. Descrição da funcionalidade
2. Justificativa
3. Casos de uso
4. Mockups (se aplicável)

## 📞 Comunicação

- Issues: Para bugs e features
- Discussions: Para perguntas e ideias
- Pull Requests: Para contribuições de código

## 🔍 Code Review

Espere por:
- Revisão de código
- Feedback construtivo
- Possíveis solicitações de mudanças
- Aprovação antes do merge

## ⚖️ Licença

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a mesma licença do projeto.

## 🙏 Agradecimentos

Obrigado por contribuir para melhorar a gestão de saúde pública! Cada contribuição, grande ou pequena, é valiosa.
