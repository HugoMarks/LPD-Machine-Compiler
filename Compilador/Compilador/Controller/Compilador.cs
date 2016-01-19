using Compilador.Business;
using Compilador.Exceptions;
using Compilador.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Compilador.Controller
{
    public class Compilador
    {
        //private List<Token> tokensLexico; //lista de tokens obtidas para averiguação
        private Sintatico sintatico;
        private GeradorCodigo geradorCodigo;
        private string arquivoCompilado;

        // deve receber um arquivo
        public Compilador(string arquivo)
        {
           // this.tokensLexico = new List<Token>();
            this.geradorCodigo = new GeradorCodigo();
            // this.sintatico = new Sintatico(arquivo,geradorCodigo, ref tokensLexico);
            this.sintatico = new Sintatico(arquivo, geradorCodigo);
            this.arquivoCompilado = arquivo;

        }

        public void Compilar()
        {
            sintatico.AnalisadorSintatico();

            SalvarCodigoFonte(geradorCodigo.ObterCodigoFonte());
 
        }

        private void SalvarCodigoFonte(string conteudo)
        {
            Arquivo arquivoFonte = new Arquivo();

            //obtem a pasta de documentos do usuário ativo
            string caminho = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            //cria pasta e o arquivo para armazenar o codigo fonte
            string novoCaminho = arquivoFonte.CriarArquivoAssembly(caminho, Path.GetFileNameWithoutExtension(arquivoCompilado));

            //abre o arquivo no modo escrita
            arquivoFonte.AbrirModoEscrita(novoCaminho);

            arquivoFonte.EscreverTodoConteudo(conteudo);

            arquivoFonte.FechaArquivo();
        }

        //public List<Token> ObterTokens()
        //{
        //    return tokensLexico;
        //}

    }
}
