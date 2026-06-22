namespace Abelardo.Exceptions
{
    public class PedidoNaoEncontradoException : AbelardoException
    {
        public PedidoNaoEncontradoException(int pedidoId)
            : base($"Pedido #{pedidoId} não encontrado.") { }
    }
}