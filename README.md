# Projeto Controle Financeiro

Este é um projeto desenvolvido para atender uma necessidade real de controle financeiro pessoal. 

Aplicação Web construída com ASP.NET Core MVC, utilizando Entity Framework e SQL Server.

## 📋 Funcionalidades

👤 Usuário
- Cadastrar
- Recuperar senha
- Login
- Autenticação / Sessão

📊 Dashboard
- Quadros (Receita, Despesas, Saldo Total, Orçamento - não disponível)
- Metas (Ainda em desenvolvimento)
- Próximos vencimentos
- Gastos por Categoria
- Balanço Semanal

💼 Contas
- Buscar
- Cadastrar
- Editar
- Excluir
- Lançamentos:
    - Buscar, Cadastrar, Editar e Excluir
- Recorrente (conta fixa):
    - Buscar, Cadastrar, Editar e Excluir
- Transferência entre contas:
    - Buscar, Cadastrar e Excluir
- Categorias:
    - Buscar, Cadastrar, Editar e Excluir
- Extrato:
    - Visualização das contas pagas

## 🛠️ Tecnologias Utilizadas

- C# (.NET 8.0)
- Entity Framework Core
- SQL Server
- SMTP Gmail (envio de e-mails)
- Docker

## ⚙️ Pré-requisitos

Certifique-se de ter as seguintes ferramentas instaladas em sua máquina:

- [.NET 8.0] - (https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
- [SQLserver] - (https://www.microsoft.com/pt-br/download/details.aspx?id=104781)
- (Opcional) Docker - caso queira executar via container - (https://www.docker.com/)

## 🚀 Como Rodar o Projeto

### 1. **Clone o Repositório**
```
    git clone https://github.com/Gabriel-Alves-TI/projeto_financeiro_mvc.git
    cd projeto_financeiro_mvc
```
### 2. **Configure a ConnectionString do Banco de dados**
Crie um arquivo appsettings.json conforme exemplo abaixo:

```
    {
        "ConnectionStrings": {
            "DefaultConnection": "Server=db;Database=financeiro_mvc;User Id=sa;Password=SUA_SENHA_AQUI;TrustServerCertificate=True;"
        },

        "EmailSettings": {
            "SmtpServer": "smtp.gmail.com",
            "Port": 587,
            "Remetente": "Suporte Financeiro",
            "RemetenteEmail": "seuemail@gmail.com",
            "RemetenteSenha": "SENHA_DO_APP_AQUI"
        },

        "Logging": {
            "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
            }
        },

        "AllowedHosts": "*"
    }
```
💡 Dica:
Para usar o Gmail, crie uma senha de app (não use sua senha real).
Você pode gerar uma senha de app acessando:
https://myaccount.google.com/apppasswords

No trecho responsável pelo envio do e-mail de recuperação, altere o host conforme o endereço local onde o projeto estiver rodando:
```
    // Mensagem do E-mail
    string mensagem = @$"
        <p>Olá! Você recebeu um link para redefinição da sua senha.</p>
        <br>
        <p>Clique abaixo e crie a nova senha.</p>
        <br>
        <p><a href='http://localhost:0000/Login/RedefinirSenha?token={usuario.Token}' style='color:#1a73e8'>Redefinir senha </a></p>
        <br>
    ";
```
### 3. **Instale as Dependências**
```
    dotnet restore
```
Após instalação, rode o comando:
```
    dotnet build
    dotnet run
```

O projeto estará disponível em:
👉 http://localhost:(porta especificada no console)

ou se quiser definir uma porta manualmente:
```
    dotnet run --urls="http://localhost:5000"
```