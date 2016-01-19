using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compilador.Model;
using Compilador.Exceptions;
using System.Globalization;

namespace Compilador.Business
{
    public class Lexico
    {
        private string caminho;
        private char caracter;
        private Arquivo arquivo;
        private int linha, coluna;
       // private List<Token> tokens;
        private Token token;

        /// <summary>
        /// inicializa o léxico
        /// </summary>
        /// <param name="dados"></param>
        ///  public Lexico(string dados, ref List<Token> tokens)
        public Lexico(string dados)
        {
            this.caminho = dados;
            this.linha = 1;
            this.coluna = 0;

            //this.tokens = tokens;
            this.token = new Token();
            this.arquivo = new Arquivo();

        }

        /// <summary>
        /// método destinado a chamar a abertura do arquivo para ser usado no lexico
        /// </summary>
        public void AbreArquivo()
        {
            //abre arquivo fonte
            arquivo.AbrirModoLeitura(this.caminho);
            LerCaracter();
        }

        /// <summary>
        /// método destinado a chamar o fechamento do arquivoque foi usado no lexico
        /// </summary>
        public void FechaArquivo()
        {
            arquivo.FechaArquivo();
        }

        /// <summary>
        /// método que obtem os tokens a partir do arquivo
        /// </summary>
        /// <param name="tokens"></param>
        public Token AnalisadorLexico()
        {
            
            if (!arquivo.FimArquivo()) // || caracter != '\0'
            {
                /*Remove comentários, espaços, enter, carriage return e tabulação */
                while ((caracter == '{' || caracter == ' ' || caracter == '\n' || caracter == '\r' || caracter == '\t') && !arquivo.FimArquivo())
                {
                    if (caracter == '{')
                    {
                        while (caracter != '}' && !arquivo.FimArquivo())
                        {
                            if (caracter == '\n')
                            {
                                linha++;
                                coluna = 0;
                            }
                            LerCaracter();
                        }
                        LerCaracter();
                    }
                    while ((caracter == ' ' || caracter == '\n' || caracter == '\r' || caracter == '\t') && !arquivo.FimArquivo())
                    {
                        if (caracter == '\n')
                        {
                            linha++;
                            coluna = 0;
                        }
                        LerCaracter();
                    }
                }
                if (caracter != '\0')
                {
                    PegaToken();
                    Token teste = new Token();
                    teste.Lexema = token.Lexema;
                    teste.Simbolo = token.Simbolo;
                    //tokens.Add(teste);
                }
                else
                {
                    token.Lexema = null;
                    token.Simbolo = Simbolo.Ssemtoken;
                    token.Linha = linha;
                }
            }
            else
            {
                token.Lexema = null;
                token.Simbolo = Simbolo.Ssemtoken;
                token.Linha = linha;
            }
            return token;
        }

        /// <summary>
        /// método que obtem token a token
        /// </summary>
        /// <returns></returns>
        private void PegaToken()
        {
           
            //se caracter é digito
            if (char.IsNumber(caracter))
            {
                TrataDigito();
            }
            else
            { 
                //se caracter é letra
                if (ChecarLetra(caracter))
                {
                    TrataIdentificadorPalavraReservada();
                }
                else
                {
                    //se caracter é :
                    if (caracter == ':')
                    {
                        TrataAtribuicao();
                    }
                    else
                    {
                        //se caracter é + - *
                        if (caracter == '+' || caracter == '-' || caracter == '*')
                        {
                            TrataOperadorAritmetico();
                        }
                        else
                        {
                            //se caracter é < > = !
                            if (caracter == '<' || caracter == '>' || caracter == '=' || caracter == '!')
                            {
                                TrataOperadorRelacional();
                            }
                            else
                            {
                                //se caracter é ; , ( ) .
                                if (caracter == ';' || caracter == ',' || caracter == '(' || caracter == ')' || caracter == '.')
                                {
                                    TrataPontuacao();
                                }
                                else
                                {
                                    arquivo.FechaArquivo();
                                    if (caracter == '}')
                                        throw new ErroException(String.Format("Linha: {0} - Coluna: {1}. Erro Léxico encontrado! Abertura de comentário inexistente!", linha, coluna), token.Linha);

                                    if (caracter == '!')
                                        throw new ErroException(String.Format("Linha: {0} - Coluna: {1}. Erro Léxico encontrado!  Caracter: \"{2}\" utilizado de forma incorreta!", linha, coluna, caracter.ToString()), token.Linha);

                                    throw new ErroException(String.Format("Linha: {0} - Coluna: {1}. Erro Léxico encontrado! Caracter: \"{2}\" inválido!", linha, coluna, caracter.ToString()), token.Linha);
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Método que obtem os dígitos lidos do arquivo
        /// </summary>
        private void TrataDigito()
        {
            string num = caracter.ToString();
            
            LerCaracter();
            
            while (char.IsNumber(caracter))
            {
                num += caracter;
                LerCaracter();
            }
            token.Lexema = num;
            token.Simbolo = Simbolo.Snumero;
            token.Linha = linha;

        }

        /// <summary>
        /// Método que obtem os identificador ou palavra reservada lidos do arquivo
        /// </summary>
        private void TrataIdentificadorPalavraReservada()
        {
            string id = caracter.ToString();

            LerCaracter();
            while (ChecarLetra(caracter) || char.IsNumber(caracter) || caracter == '_')
            {
                id += caracter;
                LerCaracter();
            }
            token.Lexema = id;
            token.Linha = linha;
            switch (id)
            {
                case "programa":
                    token.Simbolo = Simbolo.Sprograma;
                    break;
                case "se":
                    token.Simbolo = Simbolo.Sse;
                    break;
                case "entao":
                    token.Simbolo = Simbolo.Sentao;
                    break;
                case "senao":
                    token.Simbolo = Simbolo.Ssenao;
                    break;
                case "enquanto":
                    token.Simbolo = Simbolo.Senquanto;
                    break;
                case "faca":
                    token.Simbolo = Simbolo.Sfaca;
                    break;
                case "inicio":
                    token.Simbolo = Simbolo.Sinicio;
                    break;
                case "fim":
                    token.Simbolo = Simbolo.Sfim;
                    break;
                case "escreva":
                    token.Simbolo = Simbolo.Sescreva;
                    break;
                case "leia":
                    token.Simbolo = Simbolo.Sleia;
                    break;
                case "var":
                    token.Simbolo = Simbolo.Svar;
                    break;
                case "inteiro":
                    token.Simbolo = Simbolo.Sinteiro;
                    break;
                case "booleano":
                    token.Simbolo = Simbolo.Sbooleano;
                    break;
                case "verdadeiro":
                    token.Simbolo = Simbolo.Sverdadeiro;
                    break;
                case "falso":
                    token.Simbolo = Simbolo.Sfalso;
                    break;
                case "procedimento":
                    token.Simbolo = Simbolo.Sprocedimento;
                    break;
                case "funcao":
                    token.Simbolo = Simbolo.Sfuncao;
                    break;
                case "div":
                    token.Simbolo = Simbolo.Sdiv;
                    break;
                case "e":
                    token.Simbolo = Simbolo.Se;
                    break;
                case "ou":
                    token.Simbolo = Simbolo.Sou;
                    break;
                case "nao":
                    token.Simbolo = Simbolo.Snao;
                    break;
                default:
                    token.Simbolo = Simbolo.Sidentificador;
                    break;
            }
        }

        /// <summary>
        /// Método que obtem a atribuição lida do arquivo
        /// </summary>
        private void TrataAtribuicao()
        {
            string id = caracter.ToString();

            LerCaracter();

            if (caracter == '=')
            {
                id += caracter.ToString();
                token.Simbolo = Simbolo.Satribuicao;

                LerCaracter();
            }
            else
            { // caso não seja atribuição é dois pontos
                token.Simbolo = Simbolo.Sdoispontos;
            }
            token.Lexema = id;
            token.Linha = linha;
        }

        /// <summary>
        /// Método que obtem o operador aritmético lido do arquivo
        /// </summary>
        private void TrataOperadorAritmetico()
        {
            string id = caracter.ToString();

            if (id == "+")
                token.Simbolo = Simbolo.Smais;
            else
            {
                if (id == "-")
                    token.Simbolo = Simbolo.Smenos;
                else
                    token.Simbolo = Simbolo.Smult;
            }

            token.Lexema = id;
            token.Linha = linha;
            LerCaracter();
        }

        /// <summary>
        /// Método que obtem o operador relacional lido do arquivo
        /// </summary>
        private void TrataOperadorRelacional()
        {
            string id = caracter.ToString();

            LerCaracter();

            if (id == "=")
                token.Simbolo = Simbolo.Sig;
            else
            {
                if (caracter == '=')
                {
                    if (id == ">")
                        token.Simbolo = Simbolo.Smaiorig;
                    else
                    {
                        if (id == "<")
                            token.Simbolo = Simbolo.Smenorig;
                        else
                            token.Simbolo = Simbolo.Sdif;
                    }

                    id += caracter;
                    LerCaracter();
                }
                else
                {
                    if (id == ">")
                        token.Simbolo = Simbolo.Smaior;
                    else
                    {
                        if (id == "<")
                            token.Simbolo = Simbolo.Smenor;
                        else
                            if (id == "!")
                            {
                                arquivo.FechaArquivo();
                                throw new ErroException(String.Format("Linha: {0} - Coluna: {1}. Erro Léxico encontrado! Caracter: \"{2}\" utilizado de forma incorreta!", linha, coluna-1, id), token.Linha);
                        }       
                    }
                }
            }
            token.Lexema = id;
            token.Linha = linha;
        }

        /// <summary>
        /// Método que obtem a pontuação
        /// </summary>
        private void TrataPontuacao()
        {
            string id = caracter.ToString();

            token.Lexema = id;
            token.Linha = linha;
            if (id == ";")
                token.Simbolo = Simbolo.Spontovirgula;
            else
            {
                if (id == ",")
                    token.Simbolo = Simbolo.Svirgula;
                else
                {
                    if (id == "(")
                        token.Simbolo = Simbolo.Sabreparenteses;
                    else
                    {
                        if (id == ")")
                            token.Simbolo = Simbolo.Sfechaparenteses;
                        else
                            token.Simbolo = Simbolo.Sponto;
                    }
                }
            }

            LerCaracter();
        }

        /// <summary>
        /// método responsável por chamar a leitura de 1 caracter no arquivo
        /// </summary>
        private void LerCaracter()
        {
            caracter = arquivo.LerCaracter();
            coluna++;
        }

        /// <summary>
        /// Método que remove a acentuação, da seguinte maneira:
        /// representa a string original de forma que marcas como acentuação, 
        /// cedilha, entre outras, sejam separadas em caracteres distintos: 
        /// o caractere base, que é a letra, e o caractere da marcação.
        /// O caractere de acentuação, neste caso é chamado de NonSpacingMark, 
        /// ou seja, marcador sem espaço, significa que é um marcador que não ocupa nenhum espaço, 
       ///  e será aplicado ao caractere anterior.
        /// </summary>
        /// <param name="text"></param>
        /// <returns> string </returns>
        private string RemoverAcentuacao(string text)
        {
            return new string(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        /// <summary>
        /// Método que verifica se apresenta apenas letras do alfabeto, sem acentuação e cedilha
        /// </summary>
        /// <param name="c"></param>
        /// <returns> bool </returns>
        private bool ChecarLetra (char c)
        {
            if (char.IsLetter(c))
            {
                string letraSemAcento = RemoverAcentuacao(c.ToString());
                if (c.ToString() == letraSemAcento)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

    }
}