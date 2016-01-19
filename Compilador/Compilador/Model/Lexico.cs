using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compilador.Model;

namespace Compilador.Controller
{
    public class Lexico
    {
        private Arquivo arquivo;
        private char caracter;

        public Lexico(string dados)
        {
            arquivo = new Arquivo(dados);
            caracter = new char();
        }

        public void Executar(ref List<Token> Tokens)
        {
            caracter = arquivo.LerCaracter();
            while (!arquivo.FimConteudo())
            {
                /*Remove comentários */
                while ((caracter =='{' || caracter ==' ') && !arquivo.FimConteudo())
                {
                    if(caracter == '{')
                    {
                        while (caracter != '}' && !arquivo.FimConteudo())
                        {
                            caracter = arquivo.LerCaracter(); 
                        }
                        caracter = arquivo.LerCaracter();
                    }
                    while (caracter == ' ' && !arquivo.FimConteudo())
                    {
                        caracter = arquivo.LerCaracter();
                    }
                }
                if (!arquivo.UltimoCaracter())
                {
                    PegaToken(ref Tokens);
                }
            }   
        }
        private void PegaToken(ref List<Token> Tokens)
        {
            //se caracter é digito
            if(caracter >= '0' && caracter <= '9')
            {
                TrataDigito(ref Tokens);
            }
            else
            {
                //se caracter é letra
                if(caracter >= 'a' && caracter <='z')
                {
                    TrataIdentificadorPalavraReservada(ref Tokens);
                }
                else
                {
                    //se caracter é :
                    if(caracter == ':')
                    {
                        TrataAtribuicao(ref Tokens);
                    }
                    else
                    {
                        //se caracter é + - *
                        if(caracter == '+' || caracter == '-' || caracter == '*')
                        {
                            TrataOperadorAritmetico(ref Tokens);
                        }
                        else
                        {
                            //se caracter é < > = !
                            if(caracter == '<' || caracter == '>' || caracter == '=' || caracter == '!')
                            {
                                TrataOperadorRelacional(ref Tokens);
                            }
                            else
                            {
                                //se caracter é ; , ( ) .
                                if(caracter == ';' || caracter == ',' || caracter == '(' || caracter == ')' || caracter == '.')
                                {
                                    TrataPontuacao(ref Tokens);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void TrataDigito(ref List<Token> Tokens)
        {
            string num = caracter.ToString();
            Token novo = new Token();
            caracter = arquivo.LerCaracter();
            while (caracter >= '0' && caracter <= '9')
            {
                num += caracter;
                caracter = arquivo.LerCaracter();
            }
            novo.Lexema = num;
            //novo.Simbolo = "Snumero";
            Tokens.Add(novo);

        }
        private void TrataIdentificadorPalavraReservada(ref List<Token> Tokens)
        {
            string id = caracter.ToString();
            Token novo = new Token();
            caracter = arquivo.LerCaracter();
            while((caracter >= 'a' && caracter <= 'z') || (caracter >= '0' && caracter <= '9') || caracter == '_')
            {
                id += caracter;
                caracter = arquivo.LerCaracter();
            }
            novo.Lexema = id;
            switch (id)
            {
                case "programa":
                    //novo.Simbolo = "Sprograma";
                    break;
                case "se":
                    //novo.Simbolo = "Sse";
                    break;
                case "entao":
                    //novo.Simbolo = "Sentao";
                    break;
                case "senao":
                    //novo.Simbolo = "Ssenao";
                    break;
                case "enquanto":
                    //novo.Simbolo = "Senquanto";
                    break;
                case "faca":
                    //novo.Simbolo = "Sfaca";
                    break;
                case "inicio":
                    //novo.Simbolo = "Sinicio";
                    break;
                case "fim":
                    //novo.Simbolo = "Sfim";
                    break;
                case "escreva":
                    //novo.Simbolo = "Sescreva";
                    break;
                case "leia":
                    //novo.Simbolo = "Sleia";
                    break;
                case "var":
                    //novo.Simbolo = "Svar";
                    break;
                case "inteiro":
                    //novo.Simbolo = "Sinteiro";
                    break;
                case "booleano":
                    //novo.Simbolo = "Sbooleano";
                    break;
                //case "verdadeiro":
                //    novo.Simbolo = "Sverdadeiro";
                //    break;
                //case "falso":
                //    novo.Simbolo = "Sfalso";
                //    break;
                case "procedimento":
                    //novo.Simbolo = "Sprocedimento";
                    break;
                case "funcao":
                    //novo.Simbolo = "Sfuncao";
                    break;
                case "div":
                    //novo.Simbolo = "Sdiv";
                    break;
                case "e":
                    //novo.Simbolo = "Se";
                    break;
                case "ou":
                    //novo.Simbolo = "Sou";
                    break;
                case "nao":
                    //novo.Simbolo = "Snao";
                    break;
                default:
                    //novo.Simbolo = "Sidentificador";
                    break;
            }
            Tokens.Add(novo);
        }
        private void TrataAtribuicao(ref List<Token> Tokens)
        {
            string id = caracter.ToString();
            Token novo = new Token();
            caracter = arquivo.LerCaracter();
            if(caracter == '=')
            {
                //novo.Simbolo = "Satribuicao";
                novo.Lexema = id + caracter;

                caracter = arquivo.LerCaracter();
            }
            else
            {
                //novo.Simbolo = "Sdoispontos";
                novo.Lexema = id;
            }
            Tokens.Add(novo);
        }
        private void TrataOperadorAritmetico(ref List<Token> Tokens)
        {
            string id = caracter.ToString();
            Token novo = new Token();
            novo.Lexema = id;
            if (id == "+")
            {
                //novo.Simbolo = "Smais";
            }
            else
            {
                if(id == "-")
                {
                    //novo.Simbolo = "Smenos";
                }
                else
                {
                    //novo.Simbolo = "Smult";
                }
            }
            Tokens.Add(novo);
            caracter = arquivo.LerCaracter();
        }
        private void TrataOperadorRelacional(ref List<Token> Tokens)
        {
            string id = caracter.ToString();
            Token novo = new Token();
            caracter = arquivo.LerCaracter();
            if (caracter == '=')
            {
                if(id == ">")
                {
                    //novo.Simbolo = "Smaiorig";
                }
                else
                {
                    if(id == "<")
                    {
                        //novo.Simbolo = "Smenorig";
                    }
                    else
                    {
                        //novo.Simbolo = "Sdif";
                    }
                }
                id += caracter;
                caracter = arquivo.LerCaracter();
            }
            else
            {
                if (id == ">")
                {
                    //novo.Simbolo = "Smaior";
                }
                else
                {
                    if (id == "<")
                    {
                        //novo.Simbolo = "Smenor";
                    }
                    else
                    {
                        //novo.Simbolo = "Sig";
                    }
                }
            }
            novo.Lexema = id;
            Tokens.Add(novo);
        }
        private void TrataPontuacao(ref List<Token> Tokens)
        {
            string id = caracter.ToString();
            Token novo = new Token();
            novo.Lexema = id;
            if (id == ";")
            {
                //novo.Simbolo = "Sponto_virgula";
            }
            else
            {
                if (id == ",")
                {
                    //novo.Simbolo = "Svirgula";
                }
                else
                {
                    if (id == "(")
                    {
                        //novo.Simbolo = "Sabre_parenteses";
                    }
                    else
                    {
                        if (id == ")")
                        {
                            //novo.Simbolo = "Sfecha_parenteses";
                        }
                        else
                        {
                            //novo.Simbolo = "Sponto";
                        }
                    }
                }
            }
            Tokens.Add(novo);
            caracter = arquivo.LerCaracter();
        }

    }
}