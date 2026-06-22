namespace Abelardo.Models
{
    public class Cliente : Pessoa
    {
        public string Email { get; private set; }

        // Construtor para deserialização JSON
        public Cliente() { }

        public Cliente(string nome, string email = "") : base(nome)
        {
            Email = email?.Trim() ?? string.Empty;
        }

        // Usado pela persistencia para restaurar com ID já definido
        internal static Cliente Restaurar(int id, string nome, string email)
        {
            var c = new Cliente { Id = id, Nome = nome, Email = email };
            DefinirContador(id + 1);
            return c;
        }

        public override bool Corresponde(string busca) =>
            base.Corresponde(busca)
            || (!string.IsNullOrWhiteSpace(Email)
                && Email.Contains(busca, System.StringComparison.OrdinalIgnoreCase));
    }
}