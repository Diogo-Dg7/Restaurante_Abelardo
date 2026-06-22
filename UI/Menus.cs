using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Abelardo.Exceptions;
using Abelardo.Models;
using Abelardo.Services;

namespace Abelardo.UI
{
    public class Menus
    {
        private readonly MenuService      _menuService;
        private readonly PedidoService    _pedidoService;
        private readonly RelatorioService _relatorioService;
        private readonly Idioma           _idioma;

        private const string SENHA_FUNCIONARIO = "func123";
        private const string SENHA_ADMIN       = "admin123";
        private const string PASTA_RELATORIOS  = "Relatorios";

        public Menus(MenuService menuService, PedidoService pedidoService,
                     RelatorioService relatorioService, Idioma idioma)
        {
            _menuService      = menuService;
            _pedidoService    = pedidoService;
            _relatorioService = relatorioService;
            _idioma           = idioma;
        }

        // ── Helpers ────────────────────────────────────────────────────

        private void Pausar()
        {
            Console.WriteLine(_idioma.Get("EnterContinuar"));
            Console.ReadLine();
        }

        private void Titulo(string chave)
        {
            Console.Clear();
            Console.WriteLine(_idioma.Get("AppTitulo"));
            Console.WriteLine(new string('=', 45));
            Console.WriteLine(_idioma.Get(chave));
            Console.WriteLine();
        }

        private bool Autenticar(string chavePrompt, string senhaCorreta)
        {
            Console.Write(_idioma.Get(chavePrompt));
            if (Console.ReadLine() == senhaCorreta)
            {
                Console.WriteLine(_idioma.Get("AcessoConcedido"));
                return true;
            }
            Console.WriteLine(_idioma.Get("SenhaIncorreta"));
            Pausar();
            return false;
        }

        // Salva o conteudo em arquivo e exibe no console ao mesmo tempo
        private void SalvarRelatorio(string prefixo, string conteudo)
        {
            Console.WriteLine(conteudo);

            try
            {
                Directory.CreateDirectory(PASTA_RELATORIOS);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string arquivo   = Path.Combine(PASTA_RELATORIOS, $"{prefixo}_{timestamp}.txt");
                File.WriteAllText(arquivo, conteudo, Encoding.UTF8);
                Console.WriteLine(_idioma.Get("RelatorioSalvo", arquivo));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Aviso] Nao foi possivel salvar o relatorio: {ex.Message}");
            }
        }

        // Monta o relatorio de pedidos como string e o salva
        private void ImprimirESalvarPedidos(List<Pedido> lista, string prefixoArquivo)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_idioma.Get("RelatorioTitulo"));
            sb.AppendLine($"{_idioma.Get("RelatorioGeradoEm")}: {_idioma.FormatarData(DateTime.Now)}");
            sb.AppendLine();

            if (lista.Count == 0)
            {
                sb.AppendLine(_idioma.Get("NenhumResultado"));
            }
            else
            {
                foreach (var p in lista)
                {
                    string status = p.Pago
                        ? _idioma.Get("StatusPago")
                        : _idioma.Get("StatusAberto");

                    sb.AppendLine(
                        $"{_idioma.Get("ColPedido")} #{p.Id}  |  " +
                        $"{_idioma.Get("ColCliente")}: {p.Cliente.Nome}  |  " +
                        $"{_idioma.Get("ColData")}: {_idioma.FormatarData(p.DataHora)}  |  " +
                        $"{_idioma.Get("ColStatus")}: {status}  |  " +
                        $"{_idioma.Get("ColTotal")}: {p.Total:F2}");

                    foreach (var ip in p.Itens)
                    {
                        string desc = ip.Item.ObterDescricao(_idioma.Codigo);
                        sb.AppendLine($"     {ip.Item.Codigo,-8} {desc,-35} x{ip.Quantidade}  {ip.Subtotal:F2}");
                    }
                    sb.AppendLine(new string('-', 70));
                }
            }

            SalvarRelatorio(prefixoArquivo, sb.ToString());
        }

        // ── Menu Principal ─────────────────────────────────────────────

        public void ExibirMenuPrincipal()
        {
            while (true)
            {
                Titulo("MenuPrincipal");
                Console.WriteLine(_idioma.Get("OpcaoCliente"));
                Console.WriteLine(_idioma.Get("OpcaoFuncionario"));
                Console.WriteLine(_idioma.Get("OpcaoAdmin"));
                Console.WriteLine(_idioma.Get("OpcaoTrocarIdioma"));
                Console.WriteLine(_idioma.Get("OpcaoSair"));
                Console.Write(_idioma.Get("EscolhaOpcao"));

                switch (Console.ReadLine())
                {
                    case "1": MenuCliente();     break;
                    case "2": MenuFuncionario(); break;
                    case "3": MenuAdmin();       break;
                    case "4": TrocarIdioma();    break;
                    case "0": return;
                    default:
                        Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                        Pausar();
                        break;
                }
            }
        }

        private void TrocarIdioma()
        {
            Console.WriteLine();
            Console.WriteLine(_idioma.Get("TrocarIdiomaPrompt"));
            Console.Write("> ");
            string op = Console.ReadLine();

            if (op == "1" || op == "2" || op == "3")
            {
                _idioma.Trocar(int.Parse(op));
                Console.WriteLine(_idioma.Get("IdiomaAlterado"));
            }
            else
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
            }
            Pausar();
        }

        // ── Area do Cliente ────────────────────────────────────────────

        private void MenuCliente()
        {
            while (true)
            {
                Titulo("MenuPedidos");
                Console.WriteLine(_idioma.Get("VisualizarCardapio"));
                Console.WriteLine(_idioma.Get("FazerPedido"));
                Console.WriteLine(_idioma.Get("EditarPedido"));
                Console.WriteLine(_idioma.Get("PagarPedido"));
                Console.WriteLine(_idioma.Get("Voltar"));
                Console.Write(_idioma.Get("EscolhaOpcao"));

                switch (Console.ReadLine())
                {
                    case "1": ExibirCardapio(true); Pausar(); break;
                    case "2": FluxoFazerPedido();             break;
                    case "3": FluxoEditarPedido();            break;
                    case "4": FluxoPagarPedido();             break;
                    case "0": return;
                    default:
                        Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                        Pausar();
                        break;
                }
            }
        }

        private void ExibirCardapio(bool soDisponiveis = false)
        {
            Console.WriteLine();
            var lista = soDisponiveis
                ? _menuService.ItensDisponiveis
                : _menuService.Itens;

            string cCod  = _idioma.Get("ColCodigo");
            string cCat  = _idioma.Get("ColCategoria");
            string cDesc = _idioma.Get("ColDescricao");
            string cPrec = _idioma.Get("ColPreco");
            string cDisp = _idioma.Get("ColDisponivel");

            Console.WriteLine($"{cCod,-8} {cCat,-18} {cDesc,-35} {cPrec,8}  {cDisp}");
            Console.WriteLine(new string('-', 80));

            foreach (var item in lista)
            {
                string cat  = _menuService.NomeCategoria(item.Categoria);
                string desc = item.ObterDescricao(_idioma.Codigo);
                string disp = item.Disponivel
                    ? _idioma.Get("ColSim")
                    : _idioma.Get("ColNao");

                Console.WriteLine($"{item.Codigo,-8} {cat,-18} {desc,-35} {item.Preco,8:F2}  {disp}");
            }
            Console.WriteLine();
        }

        private void FluxoFazerPedido()
        {
            Titulo("MenuPedidos");
            Console.Write(_idioma.Get("NomeCliente"));
            string nome = Console.ReadLine();
            Console.Write(_idioma.Get("EmailCliente"));
            string email = Console.ReadLine();

            var cliente = _pedidoService.ObterOuCriarCliente(nome, email);
            var pedido  = _pedidoService.CriarPedido(cliente);

            AdicionarItensAoPedido(pedido);

            if (pedido.Itens.Count == 0)
            {
                Console.WriteLine(_idioma.Get("PedidoVazio"));
                Pausar();
                return;
            }

            Console.WriteLine(_idioma.Get("PedidoCriado", pedido.Id));
            ExibirResumoPedido(pedido);
            Pausar();
        }

        private void AdicionarItensAoPedido(Pedido pedido)
        {
            ExibirCardapio(true);
            while (true)
            {
                Console.Write(_idioma.Get("CodigoItem"));
                string cod = Console.ReadLine();
                if (cod == "0") break;

                var item = _menuService.Buscar(cod);
                if (item == null || !item.Disponivel)
                {
                    Console.WriteLine(_idioma.Get("ItemIndisponivel"));
                    continue;
                }

                Console.Write(_idioma.Get("QuantidadeItem"));
                if (!int.TryParse(Console.ReadLine(), out int qtd) || qtd <= 0) continue;

                try
                {
                    pedido.AdicionarItem(item, qtd);
                    Console.WriteLine(_idioma.Get("ItemAdicionado"));
                }
                catch (AbelardoException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void FluxoEditarPedido()
        {
            Titulo("MenuPedidos");
            Console.Write(_idioma.Get("NumeroPedido"));

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                Pausar();
                return;
            }

            Pedido pedido;
            try { pedido = _pedidoService.BuscarPedido(id); }
            catch (AbelardoException ex) { Console.WriteLine(ex.Message); Pausar(); return; }

            if (pedido.Pago)
            {
                Console.WriteLine(_idioma.Get("PedidoJaEncerrado"));
                Pausar();
                return;
            }

            while (true)
            {
                ExibirResumoPedido(pedido);
                Console.WriteLine(_idioma.Get("OpcaoAdicionarItem"));
                Console.WriteLine(_idioma.Get("OpcaoRemoverItem"));
                Console.WriteLine(_idioma.Get("Voltar"));
                Console.Write(_idioma.Get("EscolhaOpcao"));

                switch (Console.ReadLine())
                {
                    case "1":
                        AdicionarItensAoPedido(pedido);
                        break;
                    case "2":
                        Console.Write(_idioma.Get("RemoverItem"));
                        string cod = Console.ReadLine();
                        if (cod == "0") break;
                        try
                        {
                            pedido.RemoverItem(cod.ToUpper());
                            Console.WriteLine(_idioma.Get("ItemRemovido"));
                        }
                        catch (AbelardoException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Pausar();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                        Pausar();
                        break;
                }
            }
        }

        private void FluxoPagarPedido()
        {
            Titulo("MenuPedidos");
            Console.Write(_idioma.Get("NumeroPedido"));

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                Pausar();
                return;
            }

            Pedido pedido;
            try { pedido = _pedidoService.BuscarPedido(id); }
            catch (AbelardoException ex) { Console.WriteLine(ex.Message); Pausar(); return; }

            if (pedido.Pago)
            {
                Console.WriteLine(_idioma.Get("PedidoJaEncerrado"));
                Pausar();
                return;
            }

            ExibirResumoPedido(pedido);

            Console.Write(_idioma.Get("DividirConta"));
            if (_idioma.SimOuNao(Console.ReadLine() ?? ""))
            {
                Console.Write(_idioma.Get("NumeroPessoas"));
                if (int.TryParse(Console.ReadLine(), out int pessoas) && pessoas > 1)
                    Console.WriteLine(_idioma.Get("ValorPorPessoa", pedido.Total / pessoas));
            }

            try
            {
                pedido.Pagar();
                Console.WriteLine(_idioma.Get("PedidoPago", pedido.Id, pedido.Total));
            }
            catch (AbelardoException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Pausar();
        }

        private void ExibirResumoPedido(Pedido pedido)
        {
            Console.WriteLine();
            Console.WriteLine(_idioma.Get("PedidoAtual"));
            Console.WriteLine(
                $"{_idioma.Get("ColPedido")} #{pedido.Id}  |  " +
                $"{_idioma.Get("ColCliente")}: {pedido.Cliente.Nome}  |  " +
                $"{_idioma.Get("ColData")}: {_idioma.FormatarData(pedido.DataHora)}");
            Console.WriteLine(new string('-', 65));

            foreach (var ip in pedido.Itens)
            {
                string desc = ip.Item.ObterDescricao(_idioma.Codigo);
                Console.WriteLine($"  {ip.Item.Codigo,-8} {desc,-35} x{ip.Quantidade}  = {ip.Subtotal:F2}");
            }

            Console.WriteLine(new string('-', 65));
            Console.WriteLine(_idioma.Get("Total", pedido.Total));
            Console.WriteLine();
        }

        // ── Area do Funcionario ────────────────────────────────────────

        private void MenuFuncionario()
        {
            if (!Autenticar("SenhaFuncionario", SENHA_FUNCIONARIO)) return;

            while (true)
            {
                Titulo("MenuCardapio");
                Console.WriteLine(_idioma.Get("ListarItens"));
                Console.WriteLine(_idioma.Get("CadastrarItem"));
                Console.WriteLine(_idioma.Get("EditarItem"));
                Console.WriteLine(_idioma.Get("DeletarItem"));
                Console.WriteLine(_idioma.Get("Voltar"));
                Console.Write(_idioma.Get("EscolhaOpcao"));

                switch (Console.ReadLine())
                {
                    case "1": ExibirCardapio(); Pausar(); break;
                    case "2": FluxoCadastrarItem();       break;
                    case "3": FluxoEditarItem();          break;
                    case "4": FluxoDeletarItem();         break;
                    case "0": return;
                    default:
                        Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                        Pausar();
                        break;
                }
            }
        }

        private void FluxoCadastrarItem()
        {
            Console.WriteLine();
            Console.Write(_idioma.Get("ItemCodigo"));
            string cod = Console.ReadLine().ToUpper();

            if (_menuService.CodigoExiste(cod))
            {
                Console.WriteLine(_idioma.Get("CodigoJaExiste"));
                Pausar();
                return;
            }

            Console.Write(_idioma.Get("ItemCategoria"));
            if (!int.TryParse(Console.ReadLine(), out int catNum) || catNum < 1 || catNum > 4)
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                Pausar();
                return;
            }

            Console.Write(_idioma.Get("ItemDescricao"));
            string desc = Console.ReadLine();

            Console.Write(_idioma.Get("ItemDescricaoEn"));
            string descEn = Console.ReadLine();

            Console.Write(_idioma.Get("ItemDescricaoEs"));
            string descEs = Console.ReadLine();

            Console.Write(_idioma.Get("ItemPreco"));
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any,
                    CultureInfo.CurrentCulture, out decimal preco))
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                Pausar();
                return;
            }

            Console.Write(_idioma.Get("ItemDisponivel"));
            bool disp = _idioma.SimOuNao(Console.ReadLine() ?? "S");

            _menuService.Adicionar(new ItemMenu(cod, (Categoria)catNum, desc, preco, disp, descEn, descEs));
            Console.WriteLine(_idioma.Get("ItemCadastrado"));
            Pausar();
        }

        private void FluxoEditarItem()
        {
            Console.WriteLine();
            Console.Write(_idioma.Get("ItemCodigo"));
            string cod = Console.ReadLine().ToUpper();

            var item = _menuService.Buscar(cod);
            if (item == null)
            {
                Console.WriteLine(_idioma.Get("ItemNaoEncontrado"));
                Pausar();
                return;
            }

            Console.Write(_idioma.Get("ItemCategoria"));
            if (!int.TryParse(Console.ReadLine(), out int catNum) || catNum < 1 || catNum > 4)
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                Pausar();
                return;
            }

            Console.Write(_idioma.Get("ItemDescricao"));
            string desc = Console.ReadLine();

            Console.Write(_idioma.Get("ItemDescricaoEn"));
            string descEn = Console.ReadLine();

            Console.Write(_idioma.Get("ItemDescricaoEs"));
            string descEs = Console.ReadLine();

            Console.Write(_idioma.Get("ItemPreco"));
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any,
                    CultureInfo.CurrentCulture, out decimal preco))
            {
                Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                Pausar();
                return;
            }

            Console.Write(_idioma.Get("ItemDisponivel"));
            bool disp = _idioma.SimOuNao(Console.ReadLine() ?? "S");

            item.Atualizar((Categoria)catNum, desc, preco, disp, descEn, descEs);
            Console.WriteLine(_idioma.Get("ItemEditado"));
            Pausar();
        }

        private void FluxoDeletarItem()
        {
            Console.WriteLine();
            Console.Write(_idioma.Get("ItemCodigo"));
            string cod = Console.ReadLine().ToUpper();
            bool ok = _menuService.Remover(cod);
            Console.WriteLine(ok
                ? _idioma.Get("ItemDeletado")
                : _idioma.Get("ItemNaoEncontrado"));
            Pausar();
        }

        // ── Area do Admin ──────────────────────────────────────────────

        private void MenuAdmin()
        {
            if (!Autenticar("SenhaAdmin", SENHA_ADMIN)) return;

            while (true)
            {
                Titulo("MenuRelatorios");
                Console.WriteLine(_idioma.Get("RelatorioPeriodo"));
                Console.WriteLine(_idioma.Get("RelatorioCliente"));
                Console.WriteLine(_idioma.Get("RelatorioClientePeriodo"));
                Console.WriteLine(_idioma.Get("RelatorioItem"));
                Console.WriteLine(_idioma.Get("Voltar"));
                Console.Write(_idioma.Get("EscolhaOpcao"));

                switch (Console.ReadLine())
                {
                    case "1": RelPeriodo();        break;
                    case "2": RelCliente();        break;
                    case "3": RelClientePeriodo(); break;
                    case "4": RelItem();           break;
                    case "0": return;
                    default:
                        Console.WriteLine(_idioma.Get("OpcaoInvalida"));
                        Pausar();
                        break;
                }
            }
        }

        private bool LerDatas(out DateTime inicio, out DateTime fim)
        {
            Console.Write(_idioma.Get("DataInicio", _idioma.FormatoData));
            bool ok1 = DateTime.TryParseExact(Console.ReadLine(), _idioma.FormatoData,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio);

            Console.Write(_idioma.Get("DataFim", _idioma.FormatoData));
            bool ok2 = DateTime.TryParseExact(Console.ReadLine(), _idioma.FormatoData,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fim);

            if (!ok1 || !ok2)
            {
                Console.WriteLine(_idioma.Get("DataInvalida"));
                return false;
            }
            return true;
        }

        private void RelPeriodo()
        {
            if (!LerDatas(out DateTime ini, out DateTime fim)) { Pausar(); return; }
            ImprimirESalvarPedidos(_relatorioService.PorPeriodo(ini, fim), "relatorio_periodo");
            Pausar();
        }

        private void RelCliente()
        {
            Console.Write(_idioma.Get("EmailOuNome"));
            string b = Console.ReadLine();
            ImprimirESalvarPedidos(_relatorioService.PorCliente(b), "relatorio_cliente");
            Pausar();
        }

        private void RelClientePeriodo()
        {
            Console.Write(_idioma.Get("EmailOuNome"));
            string b = Console.ReadLine();
            if (!LerDatas(out DateTime ini, out DateTime fim)) { Pausar(); return; }
            ImprimirESalvarPedidos(
                _relatorioService.PorClienteEPeriodo(b, ini, fim),
                "relatorio_cliente_periodo");
            Pausar();
        }

        private void RelItem()
        {
            Console.Write(_idioma.Get("ItemCodigo"));
            string cod = Console.ReadLine().ToUpper();
            var resultados = _relatorioService.PorItem(cod);

            var sb = new StringBuilder();
            sb.AppendLine(_idioma.Get("RelatorioTitulo"));
            sb.AppendLine($"{_idioma.Get("RelatorioGeradoEm")}: {_idioma.FormatarData(DateTime.Now)}");
            sb.AppendLine();

            if (resultados.Count == 0)
            {
                sb.AppendLine(_idioma.Get("NenhumResultado"));
            }
            else
            {
                sb.AppendLine($"{"#Ped",-6} {"Cliente",-20} {"Data",-18} {"Qtd",4}  {"Subtotal",10}");
                sb.AppendLine(new string('-', 65));

                foreach (var (pedido, ip) in resultados)
                {
                    sb.AppendLine(
                        $"#{pedido.Id,-5} {pedido.Cliente.Nome,-20} " +
                        $"{_idioma.FormatarData(pedido.DataHora),-18} " +
                        $"{ip.Quantidade,4}  {ip.Subtotal,10:F2}");
                }
            }

            SalvarRelatorio("relatorio_item", sb.ToString());
            Pausar();
        }
    }
}