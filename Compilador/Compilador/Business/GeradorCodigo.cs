using Compilador.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Business
{
    public class GeradorCodigo
    {
        private StringBuilder codigoFonte;
        public int BaseVariavel { get; set; }
        public Stack<int> PilhaQtdVariaveisAlocadas { get; set; }

        public int Label { get; set; }

        public GeradorCodigo ()
        {
            codigoFonte = new StringBuilder();
            BaseVariavel = 0;
            Label = 1;
            PilhaQtdVariaveisAlocadas = new Stack<int>();
        }

        /// <summary>
        /// método responsável por criar o código em "assembly" 
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="parametros"></param>
        public void GerarCodigo(string codigo,string[] parametros)
        {
            /// realiza o espaçamento do comando quand não for null
            if (codigo != Codigo.NULL)
                codigoFonte.Append("   ");

            //cria label se for
            if (codigo == Codigo.CALL || codigo == Codigo.NULL ||
                codigo == Codigo.JMPF || codigo == Codigo.JMP)
            {
                parametros[0] = "L" + parametros[0];
            }


           if (codigo == Codigo.LDV || codigo == Codigo.LDC ||
               codigo == Codigo.STR || codigo == Codigo.JMP ||
               codigo == Codigo.JMPF || codigo == Codigo.CALL )
            {
                codigoFonte.Append(codigo);
                codigoFonte.Append(" ");
                codigoFonte.Append(parametros[0]);
                
            }
            else if (codigo == Codigo.NULL)
            {
                codigoFonte.Append(parametros[0]);
                codigoFonte.Append(" ");
                codigoFonte.Append(codigo);

            }
            else if (codigo == Codigo.ALLOC || codigo == Codigo.DALLOC || codigo == Codigo.RETURNF)
            {
                codigoFonte.Append(codigo);
                codigoFonte.Append(" ");
                codigoFonte.Append(parametros[0]);
                codigoFonte.Append(",");
                codigoFonte.Append(parametros[1]);
            }
            else
            {
                codigoFonte.Append(codigo);
            }

            codigoFonte.Append(Environment.NewLine);

        }

        /// <summary>
        /// método responsável por retornar o código fonte gerado
        /// </summary>
        /// <returns></returns>
        public string ObterCodigoFonte()
        {
            return codigoFonte.ToString();
        }
    }
}
