using System.Collections.Generic;
using System.Linq;
using Abelardo.Models;

namespace Abelardo.Services
{
    public class MenuService
    {
        private readonly List<ItemMenu> _itens = new List<ItemMenu>();
        private readonly Idioma _idioma;

        public MenuService(Idioma idioma) => _idioma = idioma;

        public IReadOnlyList<ItemMenu> Itens            => _itens.AsReadOnly();
        public IReadOnlyList<ItemMenu> ItensDisponiveis => _itens.Where(i => i.Disponivel).ToList().AsReadOnly();

        public void Adicionar(ItemMenu item) => _itens.Add(item);

        public ItemMenu Buscar(string codigo) =>
            _itens.FirstOrDefault(i => i.Codigo == codigo.ToUpper());

        public bool CodigoExiste(string codigo) =>
            _itens.Any(i => i.Codigo == codigo.ToUpper());

        public bool Remover(string codigo)
        {
            var item = Buscar(codigo);
            if (item == null) return false;
            _itens.Remove(item);
            return true;
        }

        public string NomeCategoria(Categoria cat) => cat switch
        {
            Categoria.Entradas         => _idioma.Get("CatEntradas"),
            Categoria.Bebidas          => _idioma.Get("CatBebidas"),
            Categoria.PratosPrincipais => _idioma.Get("CatPratosPrincipais"),
            Categoria.Sobremesas       => _idioma.Get("CatSobremesas"),
            _                          => cat.ToString()
        };
    }
}