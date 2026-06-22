using System;
using Abelardo.Mock;
using Abelardo.Services;
using Abelardo.UI;

namespace Abelardo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Unico trecho inevitavelmente hardcoded: precisamos do idioma antes de carregar recursos
            Console.WriteLine("Selecione o idioma / Select language / Seleccione el idioma:");
            Console.WriteLine("(1) Portugues   (2) English   (3) Espanol");
            Console.Write("> ");
            string opcaoStr = Console.ReadLine();
            int opcao = opcaoStr == "2" ? 2 : opcaoStr == "3" ? 3 : 1;

            var idioma           = new Idioma(opcao);
            var menuService      = new MenuService(idioma);
            var pedidoService    = new PedidoService();
            var relatorioService = new RelatorioService(pedidoService);
            var persistencia     = new PersistenciaService();

            // Tenta carregar dados salvos; se nao existir usa o mock
            if (persistencia.ExisteArquivo())
            {
                bool carregou = persistencia.Carregar(menuService, pedidoService);
                if (carregou)
                    Console.WriteLine(idioma.Get("DadosCarregados"));
                else
                {
                    Console.WriteLine(idioma.Get("ErroDados"));
                    new DadosMock().Carregar(menuService, pedidoService);
                }
            }
            else
            {
                new DadosMock().Carregar(menuService, pedidoService);
                Console.WriteLine(idioma.Get("DadosMockCarregados"));
            }

            Console.ReadLine(); // pausa para o usuario ler a mensagem

            var menus = new Menus(menuService, pedidoService, relatorioService, idioma);
            menus.ExibirMenuPrincipal();

            // Salva os dados ao fechar
            persistencia.Salvar(menuService, pedidoService);

            Console.Clear();
            Console.WriteLine(idioma.Get("AppTitulo"));
            Console.WriteLine(idioma.Get("AteLogo"));
        }
    }
}