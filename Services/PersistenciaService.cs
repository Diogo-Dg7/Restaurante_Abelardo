using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Abelardo.Models;

namespace Abelardo.Services
{
    public class PersistenciaService
    {
        private const string ARQUIVO = "dados.json";

        private readonly JsonSerializerOptions _opcoes = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };

        // DTO que representa o estado completo do sistema
        private class Dados
        {
            public int ProximoIdPessoa { get; set; }
            public int ProximoIdPedido { get; set; }
            public List<ItemMenu> Itens { get; set; } = new();
            public List<ClienteDto> Clientes { get; set; } = new();
            public List<PedidoDto> Pedidos { get; set; } = new();
        }

        private class ClienteDto
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
        }

        private class PedidoDto
        {
            public int Id { get; set; }
            public int ClienteId { get; set; }
            public DateTime DataHora { get; set; }
            public bool Pago { get; set; }
            public List<ItemPedidoDto> Itens { get; set; } = new();
        }

        private class ItemPedidoDto
        {
            public string CodigoItem { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
        }

        // ── Salvar ─────────────────────────────────────────────────────

        public void Salvar(MenuService menuService, PedidoService pedidoService)
        {
            var dados = new Dados
            {
                Itens = menuService.Itens.ToList()
            };

            foreach (var c in pedidoService.Clientes)
                dados.Clientes.Add(new ClienteDto { Id = c.Id, Nome = c.Nome, Email = c.Email });

            foreach (var p in pedidoService.Pedidos)
            {
                var dto = new PedidoDto
                {
                    Id = p.Id,
                    ClienteId = p.Cliente.Id,
                    DataHora = p.DataHora,
                    Pago = p.Pago
                };
                foreach (var ip in p.Itens)
                    dto.Itens.Add(new ItemPedidoDto
                    {
                        CodigoItem = ip.Item.Codigo,
                        Quantidade = ip.Quantidade,
                        PrecoUnitario = ip.PrecoUnitario
                    });
                dados.Pedidos.Add(dto);
            }

            // Salva os proximos IDs para restaurar os contadores
            dados.ProximoIdPessoa = pedidoService.Clientes.Count > 0
                ? pedidoService.Clientes.Max(c => c.Id) + 1 : 1;
            dados.ProximoIdPedido = pedidoService.Pedidos.Count > 0
                ? pedidoService.Pedidos.Max(p => p.Id) + 1 : 1;

            File.WriteAllText(ARQUIVO, JsonSerializer.Serialize(dados, _opcoes));
        }

        // ── Carregar ───────────────────────────────────────────────────

        public bool Carregar(MenuService menuService, PedidoService pedidoService)
        {
            if (!File.Exists(ARQUIVO)) return false;

            try
            {
                string json = File.ReadAllText(ARQUIVO);
                var dados = JsonSerializer.Deserialize<Dados>(json, _opcoes);
                if (dados == null) return false;

                // Restaura itens do menu
                foreach (var item in dados.Itens)
                    menuService.Adicionar(item);

                // Restaura clientes
                var mapaClientes = new Dictionary<int, Cliente>();
                foreach (var dto in dados.Clientes)
                {
                    var c = Cliente.Restaurar(dto.Id, dto.Nome, dto.Email);
                    pedidoService.AdicionarCliente(c);
                    mapaClientes[c.Id] = c;
                }

                // Restaura pedidos
                foreach (var dto in dados.Pedidos)
                {
                    if (!mapaClientes.TryGetValue(dto.ClienteId, out var cliente)) continue;

                    var pedido = new Pedido
                    {
                        Id = dto.Id,
                        Cliente = cliente,
                        DataHora = dto.DataHora,
                        Pago = dto.Pago,
                        Itens = new List<ItemPedido>()
                    };

                    foreach (var ipDto in dto.Itens)
                    {
                        var itemMenu = menuService.Buscar(ipDto.CodigoItem);
                        if (itemMenu == null) continue;

                        pedido.Itens.Add(new ItemPedido
                        {
                            Item = itemMenu,
                            Quantidade = ipDto.Quantidade,
                            PrecoUnitario = ipDto.PrecoUnitario
                        });
                    }

                    pedidoService.AdicionarPedido(pedido);
                    Pedido.DefinirContador(dto.Id + 1);
                }

                // Restaura contadores de ID
                Pessoa.DefinirContador(dados.ProximoIdPessoa);
                Pedido.DefinirContador(dados.ProximoIdPedido);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Aviso] Erro ao carregar dados: {ex.Message}");
                return false;
            }
        }

        public bool ExisteArquivo() => File.Exists(ARQUIVO);
    }
}