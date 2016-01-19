using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maquina_Virutal
{
    public class DadosView
    {
        public int Endereco { get; set; }
        public int? Valor { get; set; }

        public DadosView (int endereco, int valor)
        {
            this.Endereco = endereco;
            this.Valor = valor;
        }

    }

}
