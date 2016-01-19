using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Model
{
    public class ProcedimentoSimbolo : ItemSimbolo
    {
        public int Nivel { get; set; }  //nivel usado para a tabela de simbolos e geração de codigo

    }
}