using System;
using System.Globalization;
using System.Resources;

namespace Abelardo.Services
{
    public class Idioma
    {
        private ResourceManager _rm;
        private CultureInfo _cultura;

        // "pt", "en", "es", "fr" ou "ja"
        public string Codigo { get; private set; }

        public bool IsIngles => Codigo == "en";
        public bool IsEspanhol => Codigo == "es";
        public bool IsFrances => Codigo == "fr";
        public bool IsJapones => Codigo == "ja";
        public CultureInfo CulturaAtual => _cultura;
        public string FormatoData => IsIngles ? "MM/dd/yyyy" : "dd/MM/yyyy";

        public Idioma(int opcao) => Aplicar(opcao);

        public void Trocar(int opcao) => Aplicar(opcao);

        private void Aplicar(int opcao)
        {
            switch (opcao)
            {
                case 2:
                    Codigo = "en";
                    _cultura = new CultureInfo("en");
                    break;
                case 3:
                    Codigo = "es";
                    _cultura = new CultureInfo("es");
                    break;
                case 4:
                    Codigo = "fr";
                    _cultura = new CultureInfo("fr");
                    break;
                case 5:
                    Codigo = "ja";
                    _cultura = new CultureInfo("ja");
                    break;
                default:
                    Codigo = "pt";
                    _cultura = new CultureInfo("pt-BR");
                    break;
            }

            _rm = new ResourceManager(
                "Abelardo.Resources.Textos",
                System.Reflection.Assembly.GetExecutingAssembly());
        }

        public string Get(string chave) =>
            _rm?.GetString(chave, _cultura) ?? $"[{chave}]";

        public string Get(string chave, params object[] args) =>
            string.Format(Get(chave), args);

        public bool SimOuNao(string resposta)
        {
            if (string.IsNullOrWhiteSpace(resposta)) return false;
            resposta = resposta.Trim().ToUpper();
            return resposta == "S" || resposta == "SIM"
                || resposta == "Y" || resposta == "YES"
                || resposta == "SI" || resposta == "SÍ"
                || resposta == "O" || resposta == "OUI"
                || resposta == "はい" || resposta == "HAI";
        }

        public string FormatarData(DateTime dt)
        {
            string fmt = IsIngles
                ? "MM/dd/yyyy hh:mm tt"
                : "dd/MM/yyyy HH:mm";
            return dt.ToString(fmt, _cultura);
        }
    }
}