using Abelardo.Models;
using Abelardo.Services;

namespace Abelardo.Mock
{
    public class DadosMock
    {
        public void Carregar(MenuService menuService, PedidoService pedidoService)
        {
            // Entradas
            menuService.Adicionar(new ItemMenu("E001", Categoria.Entradas,
                "Bruschetta ao Tomate", 18.90m, true,
                "Tomato Bruschetta", "Bruschetta de Tomate"));
            menuService.Adicionar(new ItemMenu("E002", Categoria.Entradas,
                "Bolinho de Bacalhau (6un)", 29.90m, true,
                "Codfish Fritters (6pcs)", "Buñuelos de Bacalao (6un)"));
            menuService.Adicionar(new ItemMenu("E003", Categoria.Entradas,
                "Tabua de Frios", 45.00m, true,
                "Charcuterie Board", "Tabla de Embutidos"));

            // Bebidas
            menuService.Adicionar(new ItemMenu("B001", Categoria.Bebidas,
                "Suco de Laranja Natural", 12.00m, true,
                "Fresh Orange Juice", "Jugo de Naranja Natural"));
            menuService.Adicionar(new ItemMenu("B002", Categoria.Bebidas,
                "Agua Mineral 500ml", 5.00m, true,
                "Still Water 500ml", "Agua Mineral 500ml"));
            menuService.Adicionar(new ItemMenu("B003", Categoria.Bebidas,
                "Caipirinha de Limao", 22.00m, true,
                "Lime Caipirinha", "Caipirinha de Limon"));

            // Pratos Principais
            menuService.Adicionar(new ItemMenu("P001", Categoria.PratosPrincipais,
                "Frango Grelhado com Legumes", 45.90m, true,
                "Grilled Chicken with Vegetables", "Pollo a la Plancha con Verduras"));
            menuService.Adicionar(new ItemMenu("P002", Categoria.PratosPrincipais,
                "File Mignon ao Molho Madeira", 89.90m, true,
                "Beef Tenderloin with Madeira Sauce", "Solomillo al Vino de Madeira"));
            menuService.Adicionar(new ItemMenu("P003", Categoria.PratosPrincipais,
                "Risoto de Camarao", 72.00m, true,
                "Shrimp Risotto", "Risotto de Gambas"));

            // Sobremesas
            menuService.Adicionar(new ItemMenu("S001", Categoria.Sobremesas,
                "Pudim de Leite", 18.00m, true,
                "Milk Pudding", "Flan de Leche"));
            menuService.Adicionar(new ItemMenu("S002", Categoria.Sobremesas,
                "Mousse de Chocolate", 22.00m, true,
                "Chocolate Mousse", "Mousse de Chocolate"));
            menuService.Adicionar(new ItemMenu("S003", Categoria.Sobremesas,
                "Sorvete 2 Bolas", 16.00m, true,
                "2-Scoop Ice Cream", "Helado 2 Bolas"));

            // Clientes
            var c1 = new Cliente("Carlos Silva",   "carlos@email.com");
            var c2 = new Cliente("Maria Oliveira", "maria@email.com");
            var c3 = new Cliente("John Smith",     "john@email.com");
            var c4 = new Cliente("Ana Santos",     "ana@email.com");
            var c5 = new Cliente("Pierre Dupont",  "pierre@email.com");

            pedidoService.AdicionarCliente(c1);
            pedidoService.AdicionarCliente(c2);
            pedidoService.AdicionarCliente(c3);
            pedidoService.AdicionarCliente(c4);
            pedidoService.AdicionarCliente(c5);

            // Pedidos de exemplo
            var p1 = pedidoService.CriarPedido(c1);
            p1.AdicionarItem(menuService.Buscar("P001"), 2);
            p1.AdicionarItem(menuService.Buscar("B002"), 2);
            p1.AdicionarItem(menuService.Buscar("S001"), 1);
            p1.Pagar();

            var p2 = pedidoService.CriarPedido(c3);
            p2.AdicionarItem(menuService.Buscar("P002"), 1);
            p2.AdicionarItem(menuService.Buscar("B003"), 2);
            p2.Pagar();

            // p3 fica aberto para testar edicao
            var p3 = pedidoService.CriarPedido(c2);
            p3.AdicionarItem(menuService.Buscar("E001"), 1);
            p3.AdicionarItem(menuService.Buscar("P003"), 1);
        }
    }
}