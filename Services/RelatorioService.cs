using System;
using System.Collections.Generic;
using System.Linq;
using Abelardo.Models;

namespace Abelardo.Services
{
    public class RelatorioService
    {
        private readonly PedidoService _pedidoService;

        public RelatorioService(PedidoService pedidoService) =>
            _pedidoService = pedidoService;

        public List<Pedido> PorPeriodo(DateTime inicio, DateTime fim) =>
            _pedidoService.Pedidos
                .Where(p => p.DataHora.Date >= inicio.Date && p.DataHora.Date <= fim.Date)
                .ToList();

        public List<Pedido> PorCliente(string busca) =>
            _pedidoService.Pedidos
                .Where(p => p.Cliente.Corresponde(busca))
                .ToList();

        public List<Pedido> PorClienteEPeriodo(string busca, DateTime inicio, DateTime fim) =>
            PorCliente(busca)
                .Where(p => p.DataHora.Date >= inicio.Date && p.DataHora.Date <= fim.Date)
                .ToList();

        public List<(Pedido Pedido, ItemPedido ItemPedido)> PorItem(string codigoItem)
        {
            var resultado = new List<(Pedido, ItemPedido)>();
            foreach (var pedido in _pedidoService.Pedidos)
            {
                var ip = pedido.Itens.FirstOrDefault(i => i.Item.Codigo == codigoItem.ToUpper());
                if (ip != null) resultado.Add((pedido, ip));
            }
            return resultado;
        }
    }
}