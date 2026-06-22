using System.Collections.Generic;
using System.Linq;
using Abelardo.Exceptions;
using Abelardo.Models;

namespace Abelardo.Services
{
    public class PedidoService
    {
        private readonly List<Pedido>  _pedidos  = new List<Pedido>();
        private readonly List<Cliente> _clientes = new List<Cliente>();

        public IReadOnlyList<Pedido>  Pedidos  => _pedidos.AsReadOnly();
        public IReadOnlyList<Cliente> Clientes => _clientes.AsReadOnly();

        public void AdicionarCliente(Cliente c) => _clientes.Add(c);

        public void AdicionarPedido(Pedido p) => _pedidos.Add(p);

        public Cliente ObterOuCriarCliente(string nome, string email)
        {
            var existente = string.IsNullOrWhiteSpace(email)
                ? null
                : _clientes.FirstOrDefault(c => c.Email == email.Trim());

            if (existente != null) return existente;

            var novo = new Cliente(nome, email);
            _clientes.Add(novo);
            return novo;
        }

        public Pedido CriarPedido(Cliente cliente)
        {
            var pedido = new Pedido(cliente);
            _pedidos.Add(pedido);
            return pedido;
        }

        public Pedido BuscarPedido(int id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido == null) throw new PedidoNaoEncontradoException(id);
            return pedido;
        }
    }
}