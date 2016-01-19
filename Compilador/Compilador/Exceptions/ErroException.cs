using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Exceptions
{
    public class ErroException : Exception
    {
        public ErroException(string msg, int linhaErro) : base(msg)
        {
            this.Data.Add("linha", linhaErro);
        }
    }
}
