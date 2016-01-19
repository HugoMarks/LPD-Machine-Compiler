using Compilador.Exceptions;
using Compilador.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Business
{
    public class Semantico
    {
        private PosFixa posFixa;

        private TabelaSimbolo tabelaSimbolos;

        public Stack<Token> PilhaRetornoFuncao { get; set; }

        public Semantico (TabelaSimbolo tabela)
        {
            posFixa = new PosFixa();
            PilhaRetornoFuncao = new Stack<Token>();

            tabelaSimbolos = tabela;
        }

        #region semantico pos fixa
        /// <summary>
        /// método destinado a fazer a inserção na lista da pós fixa
        /// </summary>
        /// <param name="token"></param>
        public void InsereTokenPosFixa (Token token)
        {
            posFixa.InserePosFixa(token);
        }

        /// <summary>
        /// Método responsável por Analisar o tipo de cada membro da expressao e validar o tipo final da expressâo
        /// </summary>
        /// <param name="tabelaSimbolos"></param>
        /// <param name="tokenAtual"></param>
        /// <param name="simboloValido"></param>
        /// <param name="comando"></param>
        public void AnalisaExpressaoSemanticamente(GeradorCodigo gerador,Token tokenAtual,Simbolo simboloValido, String comando)
        {
            posFixa.DesimpilharPilhaPrioridade(); // remove os membros restantes da pilha, caso venha ter.

            // verifica se o tipo da expressao retornada é de fato igual ao tipo exigido
            if (AnalisaExpressaoPosFixa() != simboloValido)
            {
                if (simboloValido == Simbolo.Sbooleano)
                    throw new ErroException(String.Format("Linha: {0}. Erro Semântico! {1} deve conter uma expressão booleana!", tokenAtual.Linha, comando), tokenAtual.Linha);
                else
                    throw new ErroException(String.Format("Linha: {0}. Erro Semântico! {1} deve conter uma expressão inteira!", tokenAtual.Linha, comando), tokenAtual.Linha);
            }

            //gera código da expressao pos fixa
            GeraCodigoExpressao(gerador);

            // limpar lista da pos fixa
            posFixa.ResetarPosFixa();
        }

        /// <summary>
        /// Método responsável por obter a lista da expressão no formato pos fixo e avaliar cada tipo dos membros
        /// </summary>
        /// <param name=></param>
        /// <returns></returns>
        private Simbolo AnalisaExpressaoPosFixa()
        {
            List<Tipo> listaTipos = new List<Tipo>();

            foreach (Token item in posFixa.ObterListaPosfixa())
            {
                if (item.Simbolo == Simbolo.Sidentificador)
                {
                    listaTipos.Add(tabelaSimbolos.ConverteParaTipo(tabelaSimbolos.PesquisaSimbolo(item.Lexema)));
                    continue;
                }

                if (item.Simbolo == Simbolo.Snumero)
                {
                    listaTipos.Add(Tipo.inteiro);
                    continue;
                }

                if (item.Simbolo == Simbolo.Sverdadeiro || item.Simbolo == Simbolo.Sfalso)
                {
                    listaTipos.Add(Tipo.booleano);
                    continue;
                }

                if (item.Simbolo == Simbolo.Smaisunario || item.Simbolo == Simbolo.Smenosunario)
                { //logica unaria: + - I => I 
                    if (listaTipos[listaTipos.Count - 1] != Tipo.inteiro)
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Operador unário somente pode ser usado com variável ou função do tipo inteiro!", item.Linha), item.Linha);
                    continue;
                }

                if (item.Simbolo == Simbolo.Smult || item.Simbolo == Simbolo.Sdiv || item.Simbolo == Simbolo.Smais || item.Simbolo == Simbolo.Smenos)
                { //logica aritmética: * div + - I I => I 
                    if (listaTipos[listaTipos.Count - 1] == Tipo.inteiro && listaTipos[listaTipos.Count - 2] == Tipo.inteiro)
                    {
                        listaTipos.RemoveAt(listaTipos.Count - 1);
                        continue;
                    }
                    else
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O operador \"{1}\" somente pode ser usado com dois inteiros!", item.Linha, item.Lexema), item.Linha);
                }

                if (item.Simbolo == Simbolo.Smaior || item.Simbolo == Simbolo.Smaiorig ||
                    item.Simbolo == Simbolo.Smenor || item.Simbolo == Simbolo.Smenorig )
                { //logica racional: < <= > >= I I => B 
                    if (listaTipos[listaTipos.Count - 1] == Tipo.inteiro && listaTipos[listaTipos.Count - 2] == Tipo.inteiro)
                    {
                        listaTipos.RemoveAt(listaTipos.Count - 1);
                        listaTipos[listaTipos.Count - 1] = Tipo.booleano;
                        continue;
                    }
                    else
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O operador \"{1}\" somente pode ser usado com dois inteiros!", item.Linha, item.Lexema), item.Linha);
                }

                if (item.Simbolo == Simbolo.Snao)
                {// logica: nao  B => B
                    if (listaTipos[listaTipos.Count - 1] != Tipo.booleano)
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! Operador de negação somente pode ser usado com booleano!", item.Linha), item.Linha);
                    continue;
                }

                if (item.Simbolo == Simbolo.Sig || item.Simbolo == Simbolo.Sdif)
                {// logica: != = B B => B ou I I =>B
                    if (listaTipos[listaTipos.Count - 1] == listaTipos[listaTipos.Count - 2])
                    {
                        listaTipos.RemoveAt(listaTipos.Count - 1);
                        listaTipos[listaTipos.Count - 1] = Tipo.booleano;
                        continue;
                    }
                    else
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O operador \"{1}\" somente pode ser usado com dois booleano ou dois inteiros!", item.Linha, item.Lexema), item.Linha);
                }

                if (item.Simbolo == Simbolo.Se || item.Simbolo == Simbolo.Sou)
                {// logica: E OU B B => B
                    if (listaTipos[listaTipos.Count - 1] == Tipo.booleano && listaTipos[listaTipos.Count - 2] == Tipo.booleano)
                    {
                        listaTipos.RemoveAt(listaTipos.Count - 1);
                        continue;
                    }
                    else
                        throw new ErroException(String.Format("Linha: {0}. Erro Semântico! O operador lógico \"{1}\" somente pode ser usado com dois booleano!", item.Linha, item.Lexema), item.Linha);
                }
            }
            if (listaTipos[listaTipos.Count - 1] == Tipo.inteiro)
                return Simbolo.Sinteiro;

            if (listaTipos[listaTipos.Count - 1] == Tipo.booleano)
                return Simbolo.Sbooleano;

            return Simbolo.Ssemtoken;
        }

        /// <summary>
        /// método responsável por gerar o código da expressão pos-fixa gerada.
        /// </summary>
        /// <param name="geradorCodigo"></param>
        private void GeraCodigoExpressao(GeradorCodigo geradorCodigo)
        {
            foreach (Token item in posFixa.ObterListaPosfixa())
            {
                if (item.Simbolo == Simbolo.Sidentificador)
                { //pesquisa o item na tabela de simbolos para saber se é variável ou função
                    ItemSimbolo variavelFuncao = tabelaSimbolos.PesquisaItem(item.Lexema);

                    if (variavelFuncao.GetType() == typeof(VariavelSimbolo))
                    {
                        geradorCodigo.GerarCodigo(Codigo.LDV, new string[] { ((VariavelSimbolo)variavelFuncao).Memoria.ToString() });
                    }
                    else if (variavelFuncao.GetType() == typeof(FuncaoSimbolo))
                    {
                        geradorCodigo.GerarCodigo(Codigo.CALL, new string[] { ((FuncaoSimbolo)variavelFuncao).Nivel.ToString() });
                    }
                }
                else if (item.Simbolo == Simbolo.Snumero)
                {
                    geradorCodigo.GerarCodigo(Codigo.LDC, new string[] { item.Lexema });
                }
                else if (item.Simbolo == Simbolo.Snao)
                {
                    geradorCodigo.GerarCodigo(Codigo.NEG, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Sverdadeiro || item.Simbolo == Simbolo.Sfalso)
                {
                    geradorCodigo.GerarCodigo(Codigo.LDC, new string[] { ((int)item.Simbolo).ToString() });
                }
                else if (item.Simbolo == Simbolo.Smaisunario) //checar
                {
                   // geradorCodigo.GerarCodigo(Codigo.INV, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smenosunario)
                {
                    geradorCodigo.GerarCodigo(Codigo.INV, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smult)
                {
                    geradorCodigo.GerarCodigo(Codigo.MULT, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Sdiv)
                {
                    geradorCodigo.GerarCodigo(Codigo.DIVI, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smais)
                {
                    geradorCodigo.GerarCodigo(Codigo.ADD, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smenos)
                {
                    geradorCodigo.GerarCodigo(Codigo.SUB, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Sig)
                {
                    geradorCodigo.GerarCodigo(Codigo.CEQ, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Sdif)
                {
                    geradorCodigo.GerarCodigo(Codigo.CDIF, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smenor)
                {
                    geradorCodigo.GerarCodigo(Codigo.CME, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smenorig)
                {
                    geradorCodigo.GerarCodigo(Codigo.CMEQ, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smaior)
                {
                    geradorCodigo.GerarCodigo(Codigo.CMA, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Smaiorig)
                {
                    geradorCodigo.GerarCodigo(Codigo.CMAQ, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Se)
                {
                    geradorCodigo.GerarCodigo(Codigo.AND, new string[] { });
                }
                else if (item.Simbolo == Simbolo.Sou)
                {
                    geradorCodigo.GerarCodigo(Codigo.OR, new string[] { });
                }
            }
        }

        #endregion

        #region semantico controle da pilha de retorno de função

        /// <summary>
        /// retorna o topo da pilha sem desimpilhar
        /// </summary>
        /// <returns></returns>
        public Token VerTopoPilhaFuncao()
        {
            Token token = (PilhaRetornoFuncao.Pop()).Clonar();

            PilhaRetornoFuncao.Push(token);

            return token;
        }

        #endregion
    }
}
