namespace Abelardo.Models
{
    public class ItemMenu
    {
        public string Codigo { get; set; }
        public Categoria Categoria { get; set; }
        public bool Disponivel { get; set; }
        public string Descricao { get; set; }       // pt-BR
        public string DescricaoEn { get; set; }     // English
        public string DescricaoEs { get; set; }     // Español
        public decimal Preco { get; set; }

        // Construtor para JSON
        public ItemMenu() { }

        public ItemMenu(string codigo, Categoria categoria, string descricao,
                        decimal preco, bool disponivel = true,
                        string descricaoEn = "", string descricaoEs = "")
        {
            Codigo      = codigo.ToUpper();
            Categoria   = categoria;
            Descricao   = descricao;
            DescricaoEn = string.IsNullOrWhiteSpace(descricaoEn) ? descricao : descricaoEn;
            DescricaoEs = string.IsNullOrWhiteSpace(descricaoEs) ? descricao : descricaoEs;
            Preco       = preco;
            Disponivel  = disponivel;
        }

        // Retorna a descricao no idioma correto
        public string ObterDescricao(string codigoIdioma) => codigoIdioma switch
        {
            "en" => DescricaoEn,
            "es" => DescricaoEs,
            _    => Descricao
        };

        public void Atualizar(Categoria novaCategoria, string novaDescricao,
                              decimal novoPreco, bool novoDisponivel,
                              string novaDescricaoEn = "", string novaDescricaoEs = "")
        {
            Categoria   = novaCategoria;
            Descricao   = novaDescricao;
            DescricaoEn = string.IsNullOrWhiteSpace(novaDescricaoEn) ? novaDescricao : novaDescricaoEn;
            DescricaoEs = string.IsNullOrWhiteSpace(novaDescricaoEs) ? novaDescricao : novaDescricaoEs;
            Preco       = novoPreco;
            Disponivel  = novoDisponivel;
        }
    }
}