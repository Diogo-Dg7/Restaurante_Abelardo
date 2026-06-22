using System;

namespace Abelardo.Exceptions
{
    public class AbelardoException : Exception
    {
        public AbelardoException(string mensagem) : base(mensagem) { }
    }
}