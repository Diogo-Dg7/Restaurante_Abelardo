using System;
using System.Collections.Generic;
using System.Linq;
using Abelardo.Exceptions;

namespace Abelardo.Models
{
    public class Pedido
    {
        private static int _contadorId = 1;

        private List<ItemPedido> _itens;

        public int Id { get; set; }
        public Cliente Cliente { get; set; }
        public DateTime DataHora { get; set; }
        public bool Pago { get; set; }

        public List<ItemPedido> Itens
        {
            get => _itens;
            set => _itens = value;
        }

        public decimal Total => (_itens ?? new List<ItemPedido>()).Sum(i => i.Subtotal);

        // Construtor para JSON
        public Pedido()
        {
            _itens = new List<ItemPedido>();
        }

        public Pedido(Cliente cliente)
        {
            Id       = _contadorId++;
            Cliente  = cliente;
            DataHora = DateTime.Now;
            Pago     = false;
            _itens   = new List<ItemPedido>();
        }

        internal static void DefinirContador(int valor)
        {
            if (valor > _contadorId) _contadorId = valor;
        }

        public void AdicionarItem(ItemMenu item, int quantidade)
        {
            if (Pago) throw new PedidoEncerradoException(Id);

            var existente = _itens.FirstOrDefault(i => i.Item.Codigo == item.Codigo);
            if (existente != null)
                existente.AdicionarQuantidade(quantidade);
            else
                _itens.Add(new ItemPedido(item, quantidade));
        }

        public void RemoverItem(string codigoItem)
        {
            if (Pago) throw new PedidoEncerradoException(Id);

            var item = _itens.FirstOrDefault(i => i.Item.Codigo == codigoItem);
            if (item == null)
                throw new AbelardoException($"Item '{codigoItem}' nao encontrado no pedido.");

            _itens.Remove(item);
        }

        public void Pagar()
        {
            if (Pago) throw new PedidoEncerradoException(Id);
            Pago = true;
        }
    }
}