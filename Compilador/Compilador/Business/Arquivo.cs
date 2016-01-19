using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Business
{
    public class Arquivo
    {
        private StreamReader streamR;
        private StreamWriter streamW;
        private char caracterAtual;

        /// <summary>
        /// Construtor da Classe
        /// </summary>
        /// <param name="caminho"></param>
        public Arquivo()
        {
            
        }

        /// <summary>
        /// Método que abre o arquivo para leitura
        /// </summary>
        /// <param name="caminho"></param>
        public void AbrirModoLeitura(string caminho)
        {
            //abro o arquivo para leitura
            this.streamR = new StreamReader(caminho, Encoding.Default);
        }

        /// <summary>
        /// Método que abre o arquivo para escrita
        /// </summary>
        /// <param name="caminho"></param>
        public void AbrirModoEscrita(string caminho)
        {
            //abro o arquivo para leitura
            this.streamW = new StreamWriter(caminho, false, Encoding.Default);
        }

        /// <summary>
        /// Obtem um caracter lido do arquivo
        /// </summary>
        /// <returns> char </returns>
        public char LerCaracter()
        {
            if(streamR.Peek() >=0)
            {
                
                return caracterAtual = (char)streamR.Read();
            }

            return caracterAtual = '\0';
        }

        /// <summary>
        /// Método que sobre escreve o conteudo no arquivo
        /// </summary>
        /// <param name="conteudo"></param>
        public void EscreverTodoConteudo(string conteudo)
        {
            streamW.Write(conteudo);
        }

        /// <summary>
        /// Método que le todo o conteudo do arquivo
        /// </summary>
        /// <returns></returns>
        public string LerTodoConteudo()
        {
            return streamR.ReadToEnd();
        }

        /// <summary>
        /// verifica se é o fim do arquivo
        /// </summary>
        /// <returns>bool</returns>
        public bool FimArquivo()
        {
            if (caracterAtual != '\0')
                return false;
            else
                return streamR.EndOfStream;
        }
        /// <summary>
        /// fecha o arquivo
        /// </summary>
        public void FechaArquivo()
        {
            if (streamR != null)
                streamR.Close();

            if (streamW != null)
                streamW.Close();
        }

        /// <summary>
        /// Método responsável por criar um arquivo em Assembly
        /// </summary>
        /// <param name="caminho"></param>
        /// <param name="nome"></param>
        /// <returns></returns>
        public string CriarArquivoAssembly(string caminho,string nome)
        {
            string novoArquivo = caminho + "\\" + "Arquivos Compilados no CSD";

            DirectoryInfo dir = new DirectoryInfo(novoArquivo);
            try
            {
                dir.Create();

                novoArquivo += "\\" + nome + ".asmc";
       
                FileStream fs = File.Create(novoArquivo);
                fs.Close();
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Não foi possível criar o  diretório: { 0}", e.ToString()));
            }

            return novoArquivo;
        }

    }
}
