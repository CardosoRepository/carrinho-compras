using CarrinhoCompras.Application.Abstractions.Persistence;
using CarrinhoCompras.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CarrinhoCompras.Infrastructure.Persistence.Repositories;

public sealed class CarrinhoRepository : ICarrinhoRepository
{
    private readonly CarrinhoComprasDbContext _context;

    public CarrinhoRepository(
        CarrinhoComprasDbContext context)
    {
        _context = context;
    }

    public async Task<Carrinho?> ObterPorIdAsync(
        Guid carrinhoId,
        CancellationToken cancellationToken = default)
    {
        return await CriarConsultaCompleta()
            .AsNoTracking()
            .SingleOrDefaultAsync(
                carrinho => carrinho.Id == carrinhoId,
                cancellationToken);
    }

    public async Task<Carrinho?> ObterParaAtualizacaoAsync(
        Guid carrinhoId,
        CancellationToken cancellationToken = default)
    {
        return await CriarConsultaCompleta()
            .SingleOrDefaultAsync(
                carrinho => carrinho.Id == carrinhoId,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Carrinho carrinho,
        CancellationToken cancellationToken = default)
    {
        await _context.Carrinhos.AddAsync(
            carrinho,
            cancellationToken);
    }

    public async Task SalvarAlteracoesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }

    private IQueryable<Carrinho> CriarConsultaCompleta()
    {
        return _context.Carrinhos
            .Include(carrinho => carrinho.CupomAplicado)
            .Include(carrinho => carrinho.Itens);
    }
}