using CarrinhoCompras.Api.ExceptionHandling;
using CarrinhoCompras.Application;
using CarrinhoCompras.Infrastructure;
using CarrinhoCompras.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("PostgreSql")
    ?? throw new InvalidOperationException("A string de conexão 'PostgreSql' não foi configurada.");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.Services.InitializeDatabaseAsync();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();