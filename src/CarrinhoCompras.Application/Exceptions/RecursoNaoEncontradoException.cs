namespace CarrinhoCompras.Application.Exceptions;

public sealed class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}