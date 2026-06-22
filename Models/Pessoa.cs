namespace Abelardo.Models
{
    public abstract class Pessoa
    {
        private static int _contadorId = 1;

        public int Id { get; protected set; }
        public string Nome { get; protected set; }

        protected Pessoa() { }

        protected Pessoa(string nome)
        {
            Id   = _contadorId++;
            Nome = string.IsNullOrWhiteSpace(nome) ? $"Anonimo #{Id}" : nome.Trim();
        }

        // Chamado pela persistencia para restaurar sem incrementar o contador
        internal static void DefinirContador(int valor)
        {
            if (valor > _contadorId) _contadorId = valor;
        }

        public virtual bool Corresponde(string busca) =>
            Nome.Contains(busca, System.StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Nome;
    }
}