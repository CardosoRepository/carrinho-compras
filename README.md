# Carrinho de Compras

Aplicação full stack desenvolvida para simular o fluxo de um carrinho de compras, com catálogo de produtos, controle de quantidade e estoque, cupons de desconto, cálculo de totais e finalização do carrinho.

O projeto foi organizado em camadas para manter as regras de negócio separadas da API, da persistência e da interface.

## Funcionalidades

- Listagem de produtos persistidos no PostgreSQL
- Criação e consulta de carrinhos
- Adição de produtos
- Soma de quantidade ao adicionar um produto já existente
- Alteração da quantidade exata de um item
- Remoção de itens
- Validação de quantidade e estoque disponível
- Aplicação, troca e remoção de cupom
- Cupons disponíveis: `10OFF` e `15OFF`
- Cálculo automático de subtotal, desconto e total
- Finalização do carrinho
- Bloqueio de alterações em carrinhos finalizados
- Recuperação do carrinho atual no navegador
- Erros padronizados com Problem Details
- Documento OpenAPI
- Execução completa com Docker Compose

## Tecnologias

### Back-end

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL 17
- Npgsql
- xUnit

### Front-end

- React 19
- TypeScript
- Vite
- ESLint
- Nginx para servir o build no Docker

### Infraestrutura

- Docker
- Docker Compose

## Estrutura do projeto

```text
carrinho-compras
├── frontend
│   ├── src
│   │   ├── components
│   │   ├── services
│   │   ├── types
│   │   └── utils
│   ├── Dockerfile
│   └── nginx.conf
├── src
│   ├── CarrinhoCompras.Api
│   ├── CarrinhoCompras.Application
│   ├── CarrinhoCompras.Domain
│   └── CarrinhoCompras.Infrastructure
├── tests
│   └── CarrinhoCompras.Tests
├── compose.yml
└── CarrinhoCompras.sln
```

### Responsabilidade das camadas

- `CarrinhoCompras.Domain`: entidades e regras de negócio do carrinho, itens, produtos e cupons.
- `CarrinhoCompras.Application`: casos de uso, DTOs, interfaces de repositórios e montagem das respostas.
- `CarrinhoCompras.Infrastructure`: Entity Framework Core, PostgreSQL, repositórios, migrations e seed.
- `CarrinhoCompras.Api`: endpoints HTTP, injeção de dependência e tratamento global de erros.
- `frontend`: interface React e comunicação com a API.
- `CarrinhoCompras.Tests`: testes unitários do domínio e dos casos de uso.

A camada de domínio não depende de ASP.NET Core, Entity Framework Core, PostgreSQL ou React.

## Como executar com Docker

Esta é a forma recomendada para executar o projeto completo.

### Pré-requisito

- Docker Desktop instalado e em execução

### Iniciar a aplicação

Na raiz do repositório:

```bash
docker compose up --build
```

Após a inicialização:

- Front-end: `http://localhost:5173`
- API: `http://localhost:5008`
- OpenAPI: `http://localhost:5008/openapi/v1.json`

O Docker Compose inicia três serviços:

- `postgres`: banco PostgreSQL
- `api`: API .NET
- `frontend`: aplicação React servida pelo Nginx

### Parar a aplicação

```bash
docker compose down
```

Esse comando preserva os dados do PostgreSQL.

Para remover também o volume do banco e iniciar novamente com uma base vazia:

```bash
docker compose down -v
```

## Como executar localmente

### Pré-requisitos

- .NET SDK 10
- Node.js 22 ou superior
- npm
- Docker Desktop ou uma instalação local do PostgreSQL

### 1. Iniciar apenas o PostgreSQL

Na raiz do projeto:

```bash
docker compose up -d postgres
```

A configuração utilizada no ambiente de desenvolvimento é:

```text
Host: localhost
Porta: 5432
Banco: carrinho_compras
Usuário: carrinho
Senha: carrinho_dev
```

A string de conexão está configurada em:

```text
src/CarrinhoCompras.Api/appsettings.Development.json
```

### 2. Executar a API

Na raiz do projeto:

```bash
dotnet restore
dotnet run --project src/CarrinhoCompras.Api --launch-profile http
```

A API será iniciada em:

```text
http://localhost:5008
```

No ambiente `Development`, a aplicação aplica as migrations automaticamente e insere os produtos e cupons quando as tabelas correspondentes estão vazias.

### 3. Executar o front-end

Em outro terminal:

```bash
cd frontend
npm ci
npm run dev
```

O front-end será iniciado em:

```text
http://localhost:5173
```

Durante o desenvolvimento, o Vite encaminha as requisições iniciadas por `/api` para a API em `http://localhost:5008`.

## Banco de dados e migrations

As migrations ficam em:

```text
src/CarrinhoCompras.Infrastructure/Persistence/Migrations
```

A ferramenta `dotnet-ef` está configurada como ferramenta local do repositório.

Para restaurar a ferramenta:

```bash
dotnet tool restore
```

Para aplicar a migration manualmente:

```bash
dotnet ef database update \
  --project src/CarrinhoCompras.Infrastructure \
  --startup-project src/CarrinhoCompras.Api
```

No PowerShell, o mesmo comando pode ser executado em uma linha:

```powershell
dotnet ef database update --project src/CarrinhoCompras.Infrastructure --startup-project src/CarrinhoCompras.Api
```

## Dados iniciais

Os dados de produtos e cupons são carregados a partir de recursos JSON incorporados ao projeto de infraestrutura:

```text
src/CarrinhoCompras.Infrastructure/Persistence/SeedData/produtos.json
src/CarrinhoCompras.Infrastructure/Persistence/SeedData/cupons.json
```

O catálogo possui dez produtos.

Os cupons disponíveis são:

- `10OFF`: 10% de desconto
- `15OFF`: 15% de desconto

## Endpoints principais

### Produtos

```text
GET /api/produtos
```

Lista os produtos disponíveis, incluindo preço líquido e quantidade em estoque.

### Carrinhos

```text
POST /api/carrinhos
GET  /api/carrinhos/{carrinhoId}
```

### Itens

```text
POST   /api/carrinhos/{carrinhoId}/itens
PUT    /api/carrinhos/{carrinhoId}/itens/{produtoId}
DELETE /api/carrinhos/{carrinhoId}/itens/{produtoId}
```

Exemplo para adicionar um item:

```json
{
  "produtoId": "00000000-0000-0000-0000-000000000001",
  "quantidade": 1
}
```

Exemplo para substituir a quantidade:

```json
{
  "quantidade": 3
}
```

### Cupom

```text
PUT    /api/carrinhos/{carrinhoId}/cupom
DELETE /api/carrinhos/{carrinhoId}/cupom
```

Exemplo:

```json
{
  "codigoCupom": "10OFF"
}
```

### Finalização

```text
POST /api/carrinhos/{carrinhoId}/finalizar
```

Também existe um arquivo com exemplos de chamadas:

```text
src/CarrinhoCompras.Api/CarrinhoCompras.Api.http
```

## Regras de negócio implementadas

- Quantidades menores ou iguais a zero são rejeitadas.
- A quantidade de um item não pode ultrapassar o estoque disponível.
- Ao adicionar novamente um produto existente, a quantidade enviada é somada à quantidade atual.
- A alteração de quantidade substitui o valor anterior.
- A remoção de um produto inexistente retorna erro tratado.
- Apenas um cupom pode permanecer aplicado por vez.
- Aplicar outro cupom substitui o cupom anterior.
- Cupons inexistentes retornam erro tratado.
- Subtotal, desconto e total são recalculados a partir do estado atual do carrinho.
- Um carrinho finalizado não pode receber novas alterações.

## Decisões de design

### Regras concentradas no domínio

As validações principais ficam nas entidades de domínio. Os controllers apenas recebem as requisições e encaminham a execução aos casos de uso.

Essa decisão evita duplicação de regras entre API, persistência e interface.

### Casos de uso na camada Application

Cada operação relevante possui uma classe própria, como:

- `AdicionarItemUseCase`
- `AlterarQuantidadeItemUseCase`
- `AplicarCupomUseCase`
- `FinalizarCarrinhoUseCase`

Os casos de uso coordenam carregamento, execução da regra de domínio, persistência e criação da resposta.

### Repositórios específicos

Foram criadas abstrações específicas para produtos, cupons e carrinhos. Não foi utilizado um repositório genérico, pois as consultas e necessidades de cada agregado são diferentes.

### Valores monetários com `decimal`

Os preços, percentuais e totais usam `decimal`. No PostgreSQL, os preços são armazenados com precisão `numeric(18,2)`.

### Preço unitário armazenado no item

O item guarda o preço unitário utilizado no momento da inclusão. Assim, uma alteração posterior no catálogo não modifica silenciosamente o valor de um item que já está no carrinho.

### Totais calculados

Subtotal, desconto e total não são armazenados em colunas próprias. Eles são calculados a partir dos itens e do cupom aplicado, evitando inconsistência entre valores persistidos.

### Tratamento de erros

Exceções de regra de negócio e recursos não encontrados são convertidas para respostas HTTP padronizadas com Problem Details.

### Estado do carrinho no front-end

O identificador do carrinho atual é armazenado no `localStorage`. Ao atualizar a página, o front-end consulta novamente a API e recupera o estado persistido.

As operações que alteram o carrinho são bloqueadas enquanto outra operação está em andamento. Isso evita requisições concorrentes iniciadas por cliques rápidos na interface.

## Premissas assumidas

- Como os arquivos originais de produtos e cupons não estavam presentes no material utilizado durante a implementação, foram criados arquivos JSON equivalentes, mantendo os campos e as regras descritas no desafio.
- Na operação de adição, a API respeita a quantidade positiva enviada. O front-end envia uma unidade por clique. Quando o produto já existe, a quantidade enviada é somada ao valor atual.
- O estoque é utilizado para validar a quantidade máxima permitida no carrinho. O desafio não definiu reserva ou baixa definitiva do estoque durante o checkout, por isso a finalização não altera o catálogo.
- O projeto representa um cenário sem autenticação. Os carrinhos são identificados por `Guid` e o front-end mantém o identificador atual no navegador.
- A aplicação aplica migrations e seed automaticamente somente no ambiente `Development`, adequado à execução local e ao desafio técnico.

## Testes e validações

A suíte atual possui 47 testes unitários cobrindo regras do domínio e casos de uso.

Na raiz do projeto:

```bash
dotnet format --verify-no-changes
dotnet build
dotnet test
```

Para validar o front-end:

```bash
cd frontend
npm run lint
npm run build
```

## Possíveis evoluções

- Testes de integração da API com PostgreSQL
- Testes automatizados do front-end
- Controle de concorrência no servidor por versão do agregado
- Autenticação e associação do carrinho a um usuário
- Reserva ou baixa de estoque no checkout
- Pipeline de integração contínua
