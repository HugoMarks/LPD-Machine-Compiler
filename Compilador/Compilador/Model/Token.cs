using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Model
{
    public class Token
    {
        public string Lexema { get; set; }
        public Simbolo Simbolo { get; set; }
        public int Linha { get; set; }

        public Token Clonar ()
        {
            Token cloneToken = new Token();
            cloneToken.Lexema = this.Lexema;
            cloneToken.Simbolo = this.Simbolo;
            cloneToken.Linha = this.Linha;

            return cloneToken;
        }
    }
}
