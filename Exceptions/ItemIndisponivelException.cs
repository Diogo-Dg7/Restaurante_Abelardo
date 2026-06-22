namespace Abelardo.Exceptions
{
    public class ItemIndisponivelException : AbelardoException
    {
        public ItemIndisponivelException(string codigo)
            : base($"O item '{codigo}' está indisponível ou não existe.") { }
    }
}