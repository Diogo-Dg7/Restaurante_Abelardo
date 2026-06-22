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
        public string DescricaoFr { get; set; }     // Français
        public string DescricaoJa { get; set; }     // 日本語
        public decimal Preco { get; set; }

        // Construtor para JSON
        public ItemMenu() { }

        public ItemMenu(string codigo, Categoria categoria, string descricao,
                        decimal preco, bool disponivel = true,
                        string descricaoEn = "", string descricaoEs = "",
                        string descricaoFr = "", string descricaoJa = "")
        {
            Codigo = codigo.ToUpper();
            Categoria = categoria;
            Descricao = descricao;
            DescricaoEn = string.IsNullOrWhiteSpace(descricaoEn) ? descricao : descricaoEn;
            DescricaoEs = string.IsNullOrWhiteSpace(descricaoEs) ? descricao : descricaoEs;
            DescricaoFr = string.IsNullOrWhiteSpace(descricaoFr) ? descricao : descricaoFr;
            DescricaoJa = string.IsNullOrWhiteSpace(descricaoJa) ? descricao : descricaoJa;
            Preco = preco;
            Disponivel = disponivel;
        }

        // Retorna a descricao no idioma correto
        public string ObterDescricao(string codigoIdioma) => codigoIdioma switch
        {
            "en" => DescricaoEn,
            "es" => DescricaoEs,
            "fr" => DescricaoFr,
            "ja" => DescricaoJa,
            _ => Descricao
        };

        public void Atualizar(Categoria novaCategoria, string novaDescricao,
                              decimal novoPreco, bool novoDisponivel,
                              string novaDescricaoEn = "", string novaDescricaoEs = "",
                              string novaDescricaoFr = "", string novaDescricaoJa = "")
        {
            Categoria = novaCategoria;
            Descricao = novaDescricao;
            DescricaoEn = string.IsNullOrWhiteSpace(novaDescricaoEn) ? novaDescricao : novaDescricaoEn;
            DescricaoEs = string.IsNullOrWhiteSpace(novaDescricaoEs) ? novaDescricao : novaDescricaoEs;
            DescricaoFr = string.IsNullOrWhiteSpace(novaDescricaoFr) ? novaDescricao : novaDescricaoFr;
            DescricaoJa = string.IsNullOrWhiteSpace(novaDescricaoJa) ? novaDescricao : novaDescricaoJa;
            Preco = novoPreco;
            Disponivel = novoDisponivel;
        }
    }
}