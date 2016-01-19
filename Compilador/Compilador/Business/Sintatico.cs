using Compilador.Exceptions;
using Compilador.Model;
using System;
using System.Collections.Generic;

namespace Compilador.Business
{
    public class Sintatico
    {
        private Token token;
        private Lexico lexico;
        private TabelaSimbolo tabelaSimbolos;
        private Semantico semantico;
        private GeradorCodigo geradorCodigo;
        private bool retornoFuncaoFlag;

        //public Sintatico(string dados,GeradorCodigo gerador, ref List<Token> tokens)
        public Sintatico(string dados,GeradorCodigo gerador)
        {
            this.token = new Token();

            //this.lexico = new Lexico(dados, ref tokens);
            this.lexico = new Lexico(dados);

            this.tabelaSimbolos = new TabelaSimbolo();

            this.semantico = new Semantico(tabelaSimbolos);

            this.retornoFuncaoFlag = false;

            this.geradorCodigo = gerador;
        }

        /// <summary>
        /// método destinado a receber um token do léxico
        /// </summary>
        private void ObtemTokenLexico()
        {
            token = lexico.AnalisadorLexico();
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do programa
        /// </summary>
        public void AnalisadorSintatico()
        {
            lexico.AbreArquivo();
            //rotulo = 1 -- disable
            token = lexico.AnalisadorLexico();
            if (token.Simbolo == Simbolo.Sprograma)
            {
                geradorCodigo.GerarCodigo(Codigo.START,new string[] { });
                ObtemTokenLexico();
                if (token.Simbolo == Simbolo.Sidentificador)
                {
                    //insere tabela(token.lexema, nome do programa, "","") -- disable
                    tabelaSimbolos.InserirTabela(new ProgramaSimbolo {Lexema = token.Lexema }); // insere na tabela de simbolos o programa                    
                    semantico.PilhaRetornoFuncao.Push(token.Clonar());
                    ObtemTokenLexico();
                    if (token.Simbolo == Simbolo.Spontovirgula)
                    {
                        AnalisaBloco();
                        semantico.PilhaRetornoFuncao.Pop();
                        
                        //gerador de codigo, gera o comando Dalloc
                        int qtdVariaveis = geradorCodigo.PilhaQtdVariaveisAlocadas.Pop();
                        if (qtdVariaveis != 0)
                        { //gera um dalloc
                            geradorCodigo.BaseVariavel -= qtdVariaveis; // decrementa a base
                            geradorCodigo.GerarCodigo(Codigo.DALLOC, new string[] { geradorCodigo.BaseVariavel.ToString(), qtdVariaveis.ToString() });
                        }

                        if (token.Simbolo == Simbolo.Sponto)
                        {
                            geradorCodigo.GerarCodigo(Codigo.HLT, new string[] { });
                            // verifica se acabou arquivo ou é comentário
                            ObtemTokenLexico();
                            if (token.Simbolo != Simbolo.Ssemtoken)
                            {
                                //erro token a mais
                                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Código excedente encontrado!", token.Linha),token.Linha);
                            }
                            //gerador do arquivo
                        }
                        else
                        {
                            //erro falta ponto
                            throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \".\" esperado!", token.Linha), token.Linha);
                        }
                    }
                    else
                    {
                        //erro falta ponto virgula
                        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" esperado!", token.Linha), token.Linha);
                    }
                }
                else
                {
                    //erro falta identificador
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome do programa esperado!", token.Linha), token.Linha);
                }
            }
            else
            {
                //erro falta: sprograma
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"programa\" esperado!", token.Linha), token.Linha);
            }
            lexico.FechaArquivo();
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática  e semantica do bloco
        /// </summary>
        private void AnalisaBloco()
        {
            ObtemTokenLexico();
            AnalisaEtapaVariaveis();
            AnalisaSubRotinas();
            AnalisaComandos();

        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática  e semantica da etapa de variáveis
        /// </summary>
        private void AnalisaEtapaVariaveis()
        {
            int contVariaveisDeclaradas = 0; // gerador de código, conta quantas variaveis foram declaradas
            if (token.Simbolo == Simbolo.Svar)
            {
                ObtemTokenLexico();
                if (token.Simbolo == Simbolo.Sidentificador)
                {
                    while (token.Simbolo == Simbolo.Sidentificador)
                    {

                        contVariaveisDeclaradas+= AnalisaVariaveis(geradorCodigo.BaseVariavel+ contVariaveisDeclaradas);

                        if (token.Simbolo == Simbolo.Spontovirgula)
                        {
                            ObtemTokenLexico();
                        }
                        else
                        {
                            //erro esperado ponto virgula
                            throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" esperado!", token.Linha), token.Linha);
                        }
                    }
                    //gerador de codigo, gera o o comando alloc
                    geradorCodigo.GerarCodigo(Codigo.ALLOC, new string[] { geradorCodigo.BaseVariavel.ToString(), contVariaveisDeclaradas.ToString() });
                    geradorCodigo.BaseVariavel += contVariaveisDeclaradas;
                    
                }
                else
                {
                    //erro esperado identificador (pelo menos um)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da variável esperado!", token.Linha), token.Linha);
                }

            }
            geradorCodigo.PilhaQtdVariaveisAlocadas.Push(contVariaveisDeclaradas);
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da declaração de variáveis
        /// </summary>
        /// <param name="baseLocal"></param>
        /// <returns></returns>
        private int AnalisaVariaveis(int baseLocal)
        {
            int countVariavel = 0; //gerador de código, conta quantas variáveis foi declarada na linha corrente
            do
            {
                if (token.Simbolo == Simbolo.Sidentificador)
                {
                    //pesquisa dupica var tabela(token.lexema) -- disable
                    // verifica se existe duplicidade. Se falso insere, senao gera erro semantico
                    if (tabelaSimbolos.PesquisarVariavel(token.Lexema,true))
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Nome da variável está duplicado!", token.Linha), token.Linha);

                    tabelaSimbolos.InserirTabela(new VariavelSimbolo { Lexema = token.Lexema,
                                                                       Memoria =(baseLocal + countVariavel) }); // insere na tabela de simbolos a variavel sem tipo
                    countVariavel++;

                    ObtemTokenLexico();
                    if ((token.Simbolo == Simbolo.Svirgula) || (token.Simbolo == Simbolo.Sdoispontos))
                    {
                        if (token.Simbolo == Simbolo.Svirgula)
                        {
                            ObtemTokenLexico();
                            if (token.Simbolo == Simbolo.Sdoispontos)
                            {
                                //erro esperado identificador
                                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da variável esperado antes dos dois pontos!", token.Linha), token.Linha);
                            }
                        }
                    }
                    else
                    {
                        //erro esperado virgula ou dois pontos
                        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \",\" ou \":\" esperado!", token.Linha), token.Linha);
                    }
                }
                else
                {
                    //erro esperado identificador
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da variável esperado!", token.Linha), token.Linha);
                }
            } while (token.Simbolo != Simbolo.Sdoispontos);
            ObtemTokenLexico();
            AnalisaTipo();

            return countVariavel;
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da colocação dos tipos de variáveis
        /// </summary>
        private void AnalisaTipo()
        {
            if (token.Simbolo != Simbolo.Sinteiro && token.Simbolo != Simbolo.Sbooleano)
            {
                //erro esperado inteiro ou booleano
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Tipo \"inteiro\" ou \"booleano\" esperado!", token.Linha), token.Linha);
            }

            tabelaSimbolos.ColocarTipo(token.Simbolo); //ColocaTipoTabela(token.Lexema);

            ObtemTokenLexico();
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do bloco de comandos INICIO ... FIM
        /// </summary>
        private void AnalisaComandos()
        {
            if (token.Simbolo == Simbolo.Sinicio)
            {
                ObtemTokenLexico();
                AnalisaComandoSimples();
                while (token.Simbolo != Simbolo.Sfim)
                {
                    if (token.Simbolo == Simbolo.Spontovirgula)
                    {
                        ObtemTokenLexico();
                        if (token.Simbolo != Simbolo.Sfim)
                        {
                            AnalisaComandoSimples();
                        }
                    }
                    else
                    {
                        // quando o token for do tipo sem token, o arquivo já acabou e não encontrou um fim
                        if (token.Simbolo == Simbolo.Ssemtoken)
                            throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"fim\" esperado!", token.Linha), token.Linha);

                        if (token.Simbolo == Simbolo.Sig)
                            throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \":\" esperado!", token.Linha), token.Linha);

                        //erro esperado ponto virgula
                        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" esperado entre dois comandos!", token.Linha), token.Linha);
                    }
                }
                ObtemTokenLexico();
            }
            else
            {
                //erro de ponto e virgula duplicado ou faltando comando
                if (token.Simbolo == Simbolo.Spontovirgula)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" duplicado ou ausência de comando!", token.Linha), token.Linha);

                /*se for "senão" gera um erro da sintaxe do se (antes de TODO "senão" não deve apresentar ponto e virgula)
                * ex: se (expressao) entao
                * escreva(a)
                * senao
                */
                if (token.Simbolo == Simbolo.Ssenao)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"senao\" sem \"se\" anteriormente!", token.Linha), token.Linha);

                //erro gerados pelas expressões no lugar de um comando
                if (token.Simbolo == Simbolo.Smaior || token.Simbolo == Simbolo.Smaiorig || token.Simbolo == Simbolo.Sig ||
                    token.Simbolo == Simbolo.Smenor || token.Simbolo == Simbolo.Smenorig || token.Simbolo == Simbolo.Sdif ||
                    token.Simbolo == Simbolo.Smais  || token.Simbolo == Simbolo.Smenos   || token.Simbolo == Simbolo.Sou ||
                    token.Simbolo == Simbolo.Smult  || token.Simbolo == Simbolo.Sdiv     || token.Simbolo == Simbolo.Se ||
                    token.Simbolo == Simbolo.Sverdadeiro || token.Simbolo == Simbolo.Sfalso || token.Simbolo == Simbolo.Snao ||
                    token.Simbolo == Simbolo.Snumero)
                {
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Espera-se um comando!", token.Linha), token.Linha);
                }

                // quando for ponto, é porque gerou um erro onde não apresenta fim
                if (token.Simbolo == Simbolo.Sponto)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \".\" esperado somente no fim do programa precedido com o \"fim\"!", token.Linha), token.Linha);

                // quando for dois pontos sozinhos é porque gerou um erro: onde se usa apenas na declaração do tipo
                if (token.Simbolo == Simbolo.Sdoispontos)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \":\" somente pode ser usado para declarar o tipo!", token.Linha), token.Linha);

                // quando for virgula é porque gerou um erro que deve ser usado somene na declaração de variáveis
                if (token.Simbolo == Simbolo.Svirgula)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \",\" somente pode ser usado para a declaração de variáveis!", token.Linha), token.Linha);

                // quando for um faça, falta enquanto e expressao
                if (token.Simbolo == Simbolo.Sfaca)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"faca\" sem o \"enquanto\" e expressão!", token.Linha), token.Linha);

                /* quando o token for o fim, pode ser:
                // ausencia de comando do inicio e fim
                //ausencia de comando do faça do enquanto
                // ausencia de incio
                */
                if (token.Simbolo == Simbolo.Sfim)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"inicio\" esperado ou ausência de comando!", token.Linha), token.Linha);

                // quando o token for do tipo sem token, o arquivo já acabou e não encontrou um fim
                if (token.Simbolo == Simbolo.Ssemtoken)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"fim\" esperado ou ausência de comando!", token.Linha), token.Linha);

                // quando o token for Sfunçao ou Sprocediemnto é que a declaração de sub rotinas está ocorrendo dentro do bloco de comandos
                if (token.Simbolo == Simbolo.Sprocedimento || token.Simbolo == Simbolo.Sfuncao)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"funcao\" ou \"procedimento\" devem ser declarados na etapa de declaração de sub-rotinas!", token.Linha), token.Linha);
                
                // quando o token for Sabreparenteses, uso inadequado
                if (token.Simbolo == Simbolo.Sabreparenteses)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Termo de expressão inválido \"(\"!", token.Linha), token.Linha);

                // quando o token for Sfechaparentes, uso inadequado
                if (token.Simbolo == Simbolo.Sfechaparenteses)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Termo de expressão inválido \")\"!", token.Linha), token.Linha);

                // quando o token for Svar é que a declaração de variavel está ocorrendo dentro do bloco de comandos
                if (token.Simbolo == Simbolo.Svar)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"var\" deve ser declarado apenas na etapa de declaração de variáveis!", token.Linha), token.Linha);
                
                // declaração do Sprograma dentro de outro programa
                if (token.Simbolo == Simbolo.Sprograma)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"programa\" somente pode ser usado no começo da criação do progama como um todo!", token.Linha), token.Linha);
               
                // quando o token for Sinteiro ou Sbooleano só pode ser usado para a declaração de tipo
                if (token.Simbolo == Simbolo.Sinteiro || token.Simbolo == Simbolo.Sbooleano)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"inteiro\" ou \"booleano\" somente deve ser usado na declaração de tipo!", token.Linha), token.Linha);
                
                // para toda Satribuição deve preceder uma variável
                if (token.Simbolo == Simbolo.Satribuicao)
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Para toda \":=\" deve preceder uma variável!", token.Linha), token.Linha);

                //erro esperado inicio
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"inicio\" esperado!", token.Linha), token.Linha);
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica de um comando simples
        /// </summary>
        private void AnalisaComandoSimples()
        {

            if (retornoFuncaoFlag == false)
            {
                if (token.Simbolo == Simbolo.Sidentificador)
                {
                    AnalisaAtribChProcedimento();
                }
                else
                {
                    if (token.Simbolo == Simbolo.Sse)
                    {
                        AnalisaSe();
                    }
                    else
                    {
                        if (token.Simbolo == Simbolo.Senquanto)
                        {
                            AnalisaEnquanto();
                        }
                        else
                        {
                            if (token.Simbolo == Simbolo.Sleia)
                            {
                                AnalisaLeia();
                            }
                            else
                            {
                                if (token.Simbolo == Simbolo.Sescreva)
                                {
                                    AnalisaEscreva();
                                }
                                else
                                {
                                    AnalisaComandos();
                                }
                            }
                        }
                    }
                }
            }
            else //erro código inalcançável devido ter comandos após um retorno de função no escopo principal
                throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Código inacessível detectado!", token.Linha), token.Linha);

        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da atribuição ou chamada de procedimento
        /// </summary>
        private void AnalisaAtribChProcedimento()
        {
            //amazenar o token lido anteriormente, pois esse token pode ser um procedimento ou uma variavel
            Token tokenAnterior = token.Clonar();

            ObtemTokenLexico();
            if (token.Simbolo == Simbolo.Satribuicao)
            { // quando for uma atribuição, verificar se o elemento a ser atribuido é variável ou função.

                //verifica se não é variavel declarada em todo o escopo
                if (!tabelaSimbolos.PesquisarVariavel(tokenAnterior.Lexema, false))
                {
                    //verifica se o tokenAnterior é uma chamada de função existente no topo da pilha de funções
                    if (semantico.VerTopoPilhaFuncao().Lexema == tokenAnterior.Lexema)
                    {
                        //verifica se existe este nome de função ou é um procedimento
                        ItemSimbolo item = tabelaSimbolos.PesquisaItem(tokenAnterior.Lexema);

                        if (item.GetType() == typeof(FuncaoSimbolo))
                        { //se for função, realiza o controle de retorno de função

                            retornoFuncaoFlag = true; //é retorno de função, flag é setada
                        }
                        else
                            throw new ErroException(String.Format("Linha: {0}. Erro Semântico! A atribuição deve ocorrer uma variável ou função para retorno!", tokenAnterior.Linha), tokenAnterior.Linha);
                    }
                    else
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! A variável \"{1}\" não está declarada ou não faz parte deste escopo!", tokenAnterior.Linha, tokenAnterior.Lexema), tokenAnterior.Linha);                 
                }
                AnalisaAtribuicao(tokenAnterior);

            }
            else
                AnalisaChamadaProcedimento(tokenAnterior);
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do comando leia
        /// </summary>
        private void AnalisaLeia()
        {
            ObtemTokenLexico();
            if (token.Simbolo == Simbolo.Sabreparenteses)
            {
                ObtemTokenLexico();
                // (semantico) identificador precisa ser uma variável inteira
                if (token.Simbolo == Simbolo.Sidentificador)
                {
                    //se pesquisa_declvar_tabela(token.lexema)
                    //entao (pesquisa em toda a tabela)
                    if (!tabelaSimbolos.PesquisarVariavel(token.Lexema,false)) //verificia se não existe a variavel declarada em todo o escopo
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O nome \"{1}\" não está declarado ou não é uma variável!", token.Linha,token.Lexema), token.Linha);
                    
                    if(tabelaSimbolos.PesquisaSimbolo(token.Lexema) != Simbolo.Sinteiro)
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O comando leia somente aceita variável do tipo inteiro!", token.Linha), token.Linha);

                    geradorCodigo.GerarCodigo(Codigo.RD,new string[] { });
                    geradorCodigo.GerarCodigo(Codigo.STR, new string[] { ((VariavelSimbolo)tabelaSimbolos.PesquisaItem(token.Lexema)).Memoria.ToString() });
                    ObtemTokenLexico();
                    if (token.Simbolo == Simbolo.Sfechaparenteses)
                    {
                        ObtemTokenLexico();
                    }
                    else
                    {
                        //erro esperado fecha parenteses
                        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \")\" esperado!", token.Linha), token.Linha);
                    }
                    //(semantico) senao erro
                }
                else
                {
                    //erro esperado identificador
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da variável esperada!", token.Linha), token.Linha);
                }
            }
            else
            {
                //erro esperado abre parenteses
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"(\" esperado!", token.Linha), token.Linha);
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do comando escreva
        /// </summary>
        //private void AnalisaEscreva()
        //{
        //    ObtemTokenLexico();
        //    if (token.Simbolo == Simbolo.Sabreparenteses)
        //    {
        //        ObtemTokenLexico();
        //        if (token.Simbolo == Simbolo.Sidentificador)
        //        {
        //            //se pesquisa_declvarfunc_tabela(token.lexema)
        //            //entao
        //            ItemSimbolo item = tabelaSimbolos.PesquisaItem(token.Lexema);
        //            if (item.GetType() != typeof(VariavelSimbolo) && item.GetType() != typeof(FuncaoSimbolo)) //verificia se não existe a variavel ou funcao declarada em todo o escopo
        //                throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O nome \"{1}\" não está declarado ou não é uma variável nem função!", token.Linha, token.Lexema), token.Linha);

        //            if (tabelaSimbolos.PesquisaSimbolo(token.Lexema) != Simbolo.Sinteiro)
        //                throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O comando escreva somente aceita variável do tipo inteiro!", token.Linha), token.Linha);

        //            if (item.GetType() == typeof(VariavelSimbolo))
        //                geradorCodigo.GerarCodigo(Codigo.LDV, new string[] { ((VariavelSimbolo) tabelaSimbolos.PesquisaItem(token.Lexema)).Memoria.ToString() });
        //            else
        //                geradorCodigo.GerarCodigo(Codigo.CALL, new string[] { ((FuncaoSimbolo)tabelaSimbolos.PesquisaItem(token.Lexema)).Nivel.ToString() });

        //            geradorCodigo.GerarCodigo(Codigo.PRN, new string[] { });

        //            ObtemTokenLexico();
        //            if (token.Simbolo == Simbolo.Sfechaparenteses)
        //            {
        //                ObtemTokenLexico();
        //            }
        //            else
        //            {
        //                //erro esperado fecha parenteses
        //                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \")\" esperado!", token.Linha), token.Linha);
        //            }
        //            //(semantico) senao erro
        //        }
        //        else
        //        {
        //            //erro esperado identificador
        //            throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da variável esperada!", token.Linha), token.Linha);
        //        }
        //    }
        //    else
        //    {
        //        //erro esperado abre parenteses
        //        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"(\" esperado!", token.Linha), token.Linha);
        //    }
        //}

        private void AnalisaEscreva()
        {
            ObtemTokenLexico();
            if (token.Simbolo == Simbolo.Sabreparenteses)
            {
                ObtemTokenLexico();
                if (token.Simbolo == Simbolo.Sidentificador)
                {
                    //se pesquisa_declvarfunc_tabela(token.lexema)
                    //entao
                    if (!tabelaSimbolos.PesquisarVariavel(token.Lexema, false)) //verificia se não existe a variavel declarada em todo o escopo
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O nome \"{1}\" não está declarado ou não é uma variável!", token.Linha, token.Lexema), token.Linha);

                    if (tabelaSimbolos.PesquisaSimbolo(token.Lexema) != Simbolo.Sinteiro)
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O comando escreva somente aceita variável do tipo inteiro!", token.Linha), token.Linha);

                    geradorCodigo.GerarCodigo(Codigo.LDV, new string[] { ((VariavelSimbolo)tabelaSimbolos.PesquisaItem(token.Lexema)).Memoria.ToString() });
                    geradorCodigo.GerarCodigo(Codigo.PRN, new string[] { });

                    ObtemTokenLexico();
                    if (token.Simbolo == Simbolo.Sfechaparenteses)
                    {
                        ObtemTokenLexico();
                    }
                    else
                    {
                        //erro esperado fecha parenteses
                        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \")\" esperado!", token.Linha), token.Linha);
                    }
                    //(semantico) senao erro
                }
                else
                {
                    //erro esperado identificador
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da variável esperada!", token.Linha), token.Linha);
                }
            }
            else
            {
                //erro esperado abre parenteses
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"(\" esperado!", token.Linha), token.Linha);
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do comando enquanto
        /// </summary>
        private void AnalisaEnquanto()
        {
            //auxrot1 = rotulo
            //Gera(rotulo, NULL, '   ', '     ') {inicio while}
            //rotulo = rotulo+1
            int rotulo1 = geradorCodigo.Label;
            geradorCodigo.Label++; //incrementa o rotulo
            geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { rotulo1.ToString() });

            ObtemTokenLexico();
            AnalisaExpressao();
            semantico.AnalisaExpressaoSemanticamente(geradorCodigo,token,Simbolo.Sbooleano, "O comando ENQUANTO");

            if (token.Simbolo == Simbolo.Sfaca)
            {
                //auxrot2 = rotulo
                //Gera('   ', JMPF, rotulo, '     ') {salta se falso}
                //rotulo = rotulo+1
                int rotulo2 = geradorCodigo.Label;
                geradorCodigo.Label++; //incrementa o rotulo
                geradorCodigo.GerarCodigo(Codigo.JMPF, new string[] { rotulo2.ToString() });

                ObtemTokenLexico();
                AnalisaComandoSimples();
                retornoFuncaoFlag = false;

                //Gera('    ', JMP, auxrot1, '  ') {retorna inicio loop}
                //Gera(auxrot2, NULL, '     ', '    ') {fim do while}
                geradorCodigo.GerarCodigo(Codigo.JMP, new string[] { rotulo1.ToString() });
                geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { rotulo2.ToString() });
            }
            else
            {
                //erro esperado faca
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"faca\" esperado!", token.Linha), token.Linha);
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do comando se
        /// </summary>
        private void AnalisaSe()
        {
            bool retornoFuncaoEmSe;
            int rotulo1 = geradorCodigo.Label, rotulo2 = geradorCodigo.Label;
            
            ObtemTokenLexico();
            AnalisaExpressao();
            semantico.AnalisaExpressaoSemanticamente(geradorCodigo,token,Simbolo.Sbooleano, "O comando SE");
            
            if (token.Simbolo == Simbolo.Sentao)
            {
                geradorCodigo.Label++; //incrementa o rotulo
                geradorCodigo.GerarCodigo(Codigo.JMPF, new string[] { rotulo1.ToString() });

                ObtemTokenLexico();
                AnalisaComandoSimples();

                retornoFuncaoEmSe = retornoFuncaoFlag; // armazena o status da existencia de retorno

                if (token.Simbolo == Simbolo.Ssenao)
                {
                    rotulo2 = geradorCodigo.Label; //obtem um novo rotulo para o senao
                    geradorCodigo.Label++; //incrementa o rotulo
                    geradorCodigo.GerarCodigo(Codigo.JMP, new string[] { rotulo2.ToString() });
                    geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { rotulo1.ToString() });

                    retornoFuncaoFlag = false;

                    ObtemTokenLexico();
                    AnalisaComandoSimples();

                    retornoFuncaoFlag = retornoFuncaoEmSe & retornoFuncaoFlag;
                }
                else
                {
                    retornoFuncaoFlag = false;
                }

                geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { rotulo2.ToString() });
            }
            else
            {
                //erro esperado entao
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \"entao\" esperado!", token.Linha), token.Linha);
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da declaração de funções e procedimentos
        /// </summary>
        private void AnalisaSubRotinas()
        {
            bool flagSubRotinas = false;
            int labelCorrente = 0;

            if (token.Simbolo == Simbolo.Sprocedimento || token.Simbolo == Simbolo.Sfuncao)
            { // gerador de codigo, gera o JMP
                //auxrot = rotulo
                //Gera('    ', JMP, rotulo, '   ') {Salta sub-rotinas}
                //rotulo = rotulo+1
                //flag = 1
                flagSubRotinas = true;

                labelCorrente = geradorCodigo.Label;
                geradorCodigo.Label++;

                geradorCodigo.GerarCodigo(Codigo.JMP, new string[] { labelCorrente.ToString() });
            }
            while (token.Simbolo == Simbolo.Sprocedimento || token.Simbolo == Simbolo.Sfuncao)
            {
                if (token.Simbolo == Simbolo.Sprocedimento)
                {
                    AnalisaDeclaracaoProcedimento();
                }
                else
                {
                    AnalisaDeclaracaoFuncao();
                }
                if (token.Simbolo == Simbolo.Spontovirgula)
                {
                    ObtemTokenLexico();
                }
                else
                {
                    //erro esperado ponto e virgula
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" esperado!", token.Linha), token.Linha);
                }
            }
            //if flag == 1
            //Gera(auxrot, NULL, '  ', '    ') {inicio do principal}
            if (flagSubRotinas)
            {// gerador de codigo, gera o label 
                geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { labelCorrente.ToString() });
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da declaração de procedimentos
        /// </summary>
        private void AnalisaDeclaracaoProcedimento()
        {
            int nivelCorrente = geradorCodigo.Label;
            geradorCodigo.Label++;

            ObtemTokenLexico();
            
            //nivel = "L" (marca ou novo galho)
            if (token.Simbolo == Simbolo.Sidentificador)
            {
                //pesquisa_declproc_tabela(token.lexema)
                //se nao encontrou
                //entao inicio
                //      insere_tabela(token.lexema,"procedimento",nivel,rotulo) {guarda na tabSimb}
                //      Gera(rotulo, NULL, '    ', '    ') {CALL ira buscar este rotulo na tabSimb}
                //      rotulo = rotulo+1

                if (tabelaSimbolos.PesquisaItem(token.Lexema) != null)
                    throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O nome \"{1}\" já está sendo utilizado neste escopo!", token.Linha,token.Lexema), token.Linha);

                tabelaSimbolos.InserirTabela(new ProcedimentoSimbolo{Lexema = token.Lexema, Nivel = nivelCorrente});

                semantico.PilhaRetornoFuncao.Push(token.Clonar());

                geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { nivelCorrente.ToString()});

                ObtemTokenLexico();
                if (token.Simbolo == Simbolo.Spontovirgula)
                {
                    AnalisaBloco();
                    semantico.PilhaRetornoFuncao.Pop();

                    //gerador de codigo, gera o comando Dalloc e o return
                    int qtdVariaveis = geradorCodigo.PilhaQtdVariaveisAlocadas.Pop();
                    if (qtdVariaveis != 0)
                    { //gera um dalloc
                        geradorCodigo.BaseVariavel-= qtdVariaveis; // decrementa a base
                        geradorCodigo.GerarCodigo(Codigo.DALLOC, new string[] { geradorCodigo.BaseVariavel.ToString(), qtdVariaveis.ToString() });
                    }
                    geradorCodigo.GerarCodigo(Codigo.RETURN, new string[] { });

                }
                else
                {
                    //erro esperado ponto virgula
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" esperado!", token.Linha), token.Linha);
                }
                //(semantico)      fim
                //senao erro
            }
            else
            {
                //erro esperado identificador
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome do procedimento esperado!", token.Linha), token.Linha);
            }
            //(tabela de simbolo)DESEMPILHA OU VOLTA NIVEL
            tabelaSimbolos.DesimpilharNivel(nivelCorrente);
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da declaração de função
        /// </summary>
        private void AnalisaDeclaracaoFuncao()
        {
            int nivelCorrente = geradorCodigo.Label;
            geradorCodigo.Label++;

            ObtemTokenLexico();
            //nivel = "L" {marca ou novo galho}
            if (token.Simbolo == Simbolo.Sidentificador)
            {
                //pesquisa_declfunc_tabela(token.lexema)
                //se nao encontrou
                //entao inicio
                //      insere_tabela(token.lexema,"",nivel,rotulo)
                if (tabelaSimbolos.PesquisaItem(token.Lexema) != null)
                    throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O nome \"{1}\" já está sendo utilizado neste escopo!", token.Linha, token.Lexema), token.Linha);

                tabelaSimbolos.InserirTabela(new FuncaoSimbolo {Lexema = token.Lexema, Nivel = nivelCorrente });
                semantico.PilhaRetornoFuncao.Push(token.Clonar());

                geradorCodigo.GerarCodigo(Codigo.NULL, new string[] { nivelCorrente.ToString() });

                ObtemTokenLexico();
                if (token.Simbolo == Simbolo.Sdoispontos)
                {
                    ObtemTokenLexico();
                    if (token.Simbolo == Simbolo.Sinteiro || token.Simbolo == Simbolo.Sbooleano)
                    {
                        //if (token.Simbolo == Simbolo.Sinteiro)
                        //entao TABSIMB[pc].tipo = "funcao inteiro"
                        //senao TABSIMB[pc].tipo = "funcao boolean"
                        tabelaSimbolos.ColocarTipo(token.Simbolo);
                        
                        ObtemTokenLexico();
                        if (token.Simbolo == Simbolo.Spontovirgula)
                        {
                            AnalisaBloco();
                            if(retornoFuncaoFlag == false)
                            {
                                //erro função sem retorno
                                //acertar mensagem!!!!!!!!
                                throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Esperado retorno da função", token.Linha), token.Linha);
                            }
                            retornoFuncaoFlag = false;
                            semantico.PilhaRetornoFuncao.Pop();

                            //remove as variaveis da pilha de codigo
                            geradorCodigo.BaseVariavel -= geradorCodigo.PilhaQtdVariaveisAlocadas.Pop(); // decrementa a base
                        }
                        else
                        {
                            //erro esperado ponto virgula
                            throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \";\" esperado!", token.Linha), token.Linha);
                        }
                    }
                    else
                    {
                        //erro esperado inteiro ou booleano
                        throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Tipo \"inteiro\" ou \"booleano\" esperado!", token.Linha), token.Linha);
                    }
                }
                else
                {
                    //erro esperado dois pontos
                    throw new ErroException(String.Format("Linha: {0}. Erro Sintático! \":\" esperado!", token.Linha), token.Linha);
                }
                // (semantico)     fim
                //senao erro
            }
            else
            {
                //erro esperado identificador
                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Nome da função esperado!", token.Linha), token.Linha);
            }
            //(tabela de simbolo) DESEMPILHA OU VOLTA NIVEL
            tabelaSimbolos.DesimpilharNivel(nivelCorrente);
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da expressão
        /// </summary>
        private void AnalisaExpressao()
        {
            AnalisaExpressaoSimples();
            if (token.Simbolo == Simbolo.Smaior || token.Simbolo == Simbolo.Smaiorig || token.Simbolo == Simbolo.Sig ||
                token.Simbolo == Simbolo.Smenor || token.Simbolo == Simbolo.Smenorig || token.Simbolo == Simbolo.Sdif)
            {
                semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo

                ObtemTokenLexico();
                AnalisaExpressaoSimples();
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica da expressão simples
        /// </summary>
        private void AnalisaExpressaoSimples()
        {
            if (token.Simbolo == Simbolo.Smais || token.Simbolo == Simbolo.Smenos)
            {
                if (token.Simbolo == Simbolo.Smais)
                    token.Simbolo = Simbolo.Smaisunario;
                else
                    token.Simbolo = Simbolo.Smenosunario;

                semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo

                ObtemTokenLexico();
            }
            AnalisaTermo();
            while (token.Simbolo == Simbolo.Smais || token.Simbolo == Simbolo.Smenos || token.Simbolo == Simbolo.Sou)
            {
                semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo

                ObtemTokenLexico();
                AnalisaTermo();
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do termo
        /// </summary>
        private void AnalisaTermo()
        {
            AnalisaFator();
            while (token.Simbolo == Simbolo.Smult || token.Simbolo == Simbolo.Sdiv || token.Simbolo == Simbolo.Se)
            {
                semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo

                ObtemTokenLexico();
                AnalisaFator();
            }
        }

        /// <summary>
        /// método responsável por analisar a garantia da gramática e semantica do fator
        /// </summary>
        private void AnalisaFator()
        {
            if (token.Simbolo == Simbolo.Sidentificador) //variavel ou funcao
            {
                //Se pesquisa_tabela(token.lexema,nivel,ind)
                //entao se (TabSimb[ind].tipo = "funcao inteiro") ou
                //         (TabSimb[ind].tipo = "funcao booleano")
                /*          entao*/
                //          senao Lexico(token)
                //      senao ERRO
                ItemSimbolo item = tabelaSimbolos.PesquisaItem(token.Lexema);

                if (item != null)
                {
                    if (item.GetType() == typeof(FuncaoSimbolo))
                    {
                        semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                        AnalisaChamadaFuncao();
                    }
                    else
                    {
                        if (item.GetType() == typeof(VariavelSimbolo))
                        {
                            semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                            ObtemTokenLexico();
                        }     
                        else
                            throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Nome \"{1}\" não é uma variável ou função!", token.Linha, token.Lexema), token.Linha);
                    }       
                        
                }
                else
                    throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Nome \"{1}\" não está declarado no escopo!", token.Linha,token.Lexema), token.Linha);
   
            }
            else
            {
                if (token.Simbolo == Simbolo.Snumero) //numero
                {
                    semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                    ObtemTokenLexico();
                }
                else
                {
                    if (token.Simbolo == Simbolo.Snao) //nao
                    {
                        semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                        ObtemTokenLexico();
                        AnalisaFator();
                    }
                    else
                    {
                        if (token.Simbolo == Simbolo.Sabreparenteses) //expressao entre parenteses
                        {
                            semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                            ObtemTokenLexico();
                            AnalisaExpressao(); //AnalisaExpressao(token);
                            if (token.Simbolo == Simbolo.Sfechaparenteses)
                            {
                                semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                                ObtemTokenLexico();
                            }
                            else
                            {
                                //erro esperado fecha parenteses
                                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! Expressão Incorreta ou \")\" esperado!", token.Linha), token.Linha);
                            }
                        }
                        else
                        {
                            if (token.Simbolo == Simbolo.Sverdadeiro || token.Simbolo == Simbolo.Sfalso)
                            {
                                semantico.InsereTokenPosFixa(token); //semantico e geracao de codigo
                                ObtemTokenLexico();
                            }
                            else
                            {
                                //erro esperado um fator
                                throw new ErroException(String.Format("Linha: {0}. Erro Sintático! fator esperado!", token.Linha), token.Linha);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// método responsável por garantiar a gramática e semantica da chamada de procedimento
        /// </summary>
        /// <param name="tokenProcedimento"></param>
        private void AnalisaChamadaProcedimento(Token tokenProcedimento)
        {   //verifica se não existe um procedimento declarado em todo o escopo
            ItemSimbolo procedimento = tabelaSimbolos.PesquisaItem(tokenProcedimento.Lexema);

            if ( procedimento == null)  
                throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Este procedimento \"{1}\" não está declarado!", tokenProcedimento.Linha, tokenProcedimento.Lexema), tokenProcedimento.Linha);

            if (procedimento.GetType() != typeof(ProcedimentoSimbolo))
                throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Uso da variável \"{1}\" de forma indevida!", tokenProcedimento.Linha, tokenProcedimento.Lexema), tokenProcedimento.Linha);

            // gerar codigo da chamada de procedimento
            geradorCodigo.GerarCodigo(Codigo.CALL, new string[] { ((ProcedimentoSimbolo)procedimento).Nivel.ToString() });

        }

        /// <summary>
        /// método responsável por garantiar a gramática e semantica da atribuição
        /// </summary>
        /// <param name="tokenAnterior"></param>
        private void AnalisaAtribuicao(Token tokenAnterior)
        {

            ObtemTokenLexico();
            AnalisaExpressao();
            semantico.AnalisaExpressaoSemanticamente(geradorCodigo,token,tabelaSimbolos.PesquisaSimbolo(tokenAnterior.Lexema), "A atribuição");

            if (retornoFuncaoFlag) // gera o códgo do returnf caso seja o retorno de função
            {
                //gerador de codigo, gera o comando returnf
                int qtdVariaveis = geradorCodigo.PilhaQtdVariaveisAlocadas.Pop();
                //insere na pilha, pois pode ter outros retornos 
                geradorCodigo.PilhaQtdVariaveisAlocadas.Push(qtdVariaveis);

                int baseFuncao = geradorCodigo.BaseVariavel - qtdVariaveis;
                geradorCodigo.GerarCodigo(Codigo.RETURNF, new string[] { baseFuncao.ToString(), qtdVariaveis.ToString() });
            }
            else // senao gera um str
                geradorCodigo.GerarCodigo(Codigo.STR, new string[] {((VariavelSimbolo)tabelaSimbolos.PesquisaItem(tokenAnterior.Lexema)).Memoria.ToString() });
        }

        /// <summary>
        /// método responsável por garantiar a gramática e semantica da chamada de função
        /// </summary>
        private void AnalisaChamadaFuncao()
        {
            // a geração de código ocorre na geração da pos fixa, pois ocorre somente em atribuições
            //geradorCodigo.GerarCodigo(Codigo.CALL, new string[] { ((FuncaoSimbolo)tabelaSimbolos.PesquisaItem(token.Lexema)).Nivel.ToString() });

            ObtemTokenLexico();
        }

    }
}
