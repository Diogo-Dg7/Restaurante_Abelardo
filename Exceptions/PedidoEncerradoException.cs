namespace Abelardo.Exceptions
{
    public class PedidoEncerradoException : AbelardoException
    {
        public PedidoEncerradoException(int pedidoId)
            : base($"O pedido #{pedidoId} já foi encerrado e não pode ser alterado.") { }
    }
}