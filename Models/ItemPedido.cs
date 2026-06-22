namespace Abelardo.Models
{
    public class ItemPedido
    {
        public ItemMenu Item { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }  // snapshot do preco no momento do pedido
        public decimal Subtotal => PrecoUnitario * Quantidade;

        // Construtor para JSON
        public ItemPedido() { }

        public ItemPedido(ItemMenu item, int quantidade)
        {
            Item           = item;
            Quantidade     = quantidade;
            PrecoUnitario  = item.Preco;
        }

        public void AdicionarQuantidade(int qtd) => Quantidade += qtd;
    }
}