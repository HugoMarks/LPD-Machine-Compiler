using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maquina_Virutal
{
     public class LinhaComandoView
    {
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public LinhaComandoView (LinhaComando linha)
        {
            switch (linha.Tipo)
            {
                case 1: // comandos 2 parametros
                    this.Column1 = linha.Linha.ToString();
                    this.Column3 = linha.Comando;
                    this.Column4 = linha.Atributo1.ToString();
                    this.Column5 = linha.Atributo2.ToString();

                    break;
                case 2: // comandos com 1 parametro
                    this.Column1 = linha.Linha.ToString();
                    this.Column3 = linha.Comando;
                    this.Column4 = linha.Atributo1.ToString();
                    break;
                case 3: //comandos JMP JMPF CALL
                    if (linha.Comando == "CALL" || linha.Comando == "JMP" || linha.Comando == "JMPF")
                    {
                        this.Column1 = linha.Linha.ToString();
                        this.Column3 = linha.Comando;
                        this.Column4 = linha.Label;
                    }
                    else //LABEL
                    {
                        this.Column1 = linha.Linha.ToString();
                        this.Column2 = linha.Label;
                        this.Column3 = linha.Comando;
                    }
                    break;
                case 4: // comandos sem parametro
                    this.Column1 = linha.Linha.ToString();
                    this.Column3 = linha.Comando;
                    break;
            }
        }
    }
}
