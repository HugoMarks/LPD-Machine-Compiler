using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maquina_Virutal
{
    public class LinhaComando
    {
        public int Linha { get; set; }
        public string Comando { get; set; }
        public string Label { get; set; }
        public int Atributo1 { get; set; }
        public int Atributo2 { get; set; }
        public string Comentario { get; set; }
        public int Tipo { get; set; }

        public LinhaComando(int linha, string comando, int atr1, int atr2, string comentario )
        {
            this.Linha = linha;
            this.Comando = comando;
            this.Atributo1 = atr1;
            this.Atributo2 = atr2;
            this.Comentario = comentario;
            this.Tipo = 1;
        }
        public LinhaComando(int linha, string comando, int atr1, string comentario)
        {
            this.Linha = linha;
            this.Comando = comando;
            this.Atributo1 = atr1;
            this.Comentario = comentario;
            this.Tipo = 2;
        }

        public LinhaComando(int linha, string comando, string label, string comentario)
        {
            this.Linha = linha;
            this.Comando = comando;
            this.Label = label;
            this.Comentario = comentario;
            this.Tipo = 3;
        }

        public LinhaComando(int linha, string comando, string comentario)
        {
            this.Linha = linha;
            this.Comando = comando;
            this.Comentario = comentario;
            this.Tipo = 4;
        }

    }

    
}
