using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maquina_Virutal.exceptions
{
    public class ErroExecucaoException : Exception
    {
        public ErroExecucaoException (string msg): base(msg)
        {
        }
    }
}
