using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Maquina_Virutal
{
    public class Folder
    {
        private StreamReader streamR;
        public String conteudo {get; set; }
        public bool ConteudoLido { get; set; }

        public Folder()
        {
            String filePath = openFolder();

            if (filePath != String.Empty)
            {
                readFile(filePath);
                ConteudoLido = true;
            }
            else
                ConteudoLido = false;
                
        }

        private string openFolder()
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.DefaultExt = "*.asmc";
            dialog.Filter = "Arquivos ASMC (*.asmc)|*.asmc| Todos os arquivos (*.*)|*.*";

            DialogResult result = dialog.ShowDialog();

            if (result.ToString() == "OK")
                return dialog.FileName;
            else
                return String.Empty;
        }

        private void readFile(String filePath)
        {
            using (streamR = new StreamReader(filePath))
            {
                this.conteudo = streamR.ReadToEnd();
            }
        }
     

    }
}
