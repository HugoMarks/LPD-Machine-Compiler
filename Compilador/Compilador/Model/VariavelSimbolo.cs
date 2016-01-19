using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Model
{
    public class VariavelSimbolo : ItemSimbolo
    {
        public Tipo TipoVariavel { get; set; }

        public int Memoria { get; set; }

    }
}
