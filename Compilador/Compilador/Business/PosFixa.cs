using Compilador.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Business
{
    public class PosFixa
    {
        private List<Token> listaPosFixa;
        private List<Token> pilhaPrioridade;
        private IDictionary<Simbolo,int> tabelaPrioridade;

        public PosFixa ()
        {
            listaPosFixa = new List<Token>();

            pilhaPrioridade = new List<Token>();

            tabelaPrioridade = new Dictionary<Simbolo, int>();
            GerarPrioridade();
        }

        
        /// <summary>
        /// método destinado a retornar a lista pos-fixa gerada
        /// </summary>
        /// <returns></returns>
        public List<Token> ObterListaPosfixa ()
        {
            return listaPosFixa;
        }

        /// <summary>
        /// Método destinado a esvaziar a pilha de Prioridade e inserir na lista, caso sobre
        /// algum item ainda.
        /// </summary>
        public void DesimpilharPilhaPrioridade()
        {
            if (pilhaPrioridade.Count != 0)
            {
                foreach (Token item in pilhaPrioridade.Reverse<Token>())
                {
                    InsereLista(item);
                    pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);
                }
            } 
        }

        /// <summary>
        /// método responsável por limpar as listas utilizadas na posfixa
        /// </summary>
        public void ResetarPosFixa()
        {
            listaPosFixa.Clear();
            pilhaPrioridade.Clear();
        }

        /// <summary>
        /// Método destinado a inserir um item na pilha de prioridade realizando a lógica da 
        /// pós fixa
        /// </summary>
        /// <param name="item"></param>
        private void InserePilha(Token item)
        {
            if (pilhaPrioridade.Count == 0 || (item.Simbolo == Simbolo.Sabreparenteses))
                pilhaPrioridade.Add(item);
            else
            {
                if(item.Simbolo == Simbolo.Sfechaparenteses)
                { //caso seja um fecha parenteses realiza o desempilhamento até encontrar um abre
                    while (pilhaPrioridade.Last().Simbolo != Simbolo.Sabreparenteses)
                    {
                        //remove da pilha de prioridade e insere na lista PosFixa
                        InsereLista(pilhaPrioridade.Last()); //
                        pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);
                   
                    } 
                    
                    //remove da pilha de prioridade o abre parenteses
                    pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);

                }
                else
                {

                    // verifica se a prioridade do item atual é MENOR que o ultimo item, entao insere na pilha.
                    if (tabelaPrioridade[item.Simbolo] < tabelaPrioridade[pilhaPrioridade.Last().Simbolo])
                        pilhaPrioridade.Add(item);
                    else
                    {
                        if (pilhaPrioridade.Last().Simbolo == Simbolo.Snao && item.Simbolo == Simbolo.Snao)
                            pilhaPrioridade.Add(item);
                        else
                        {
                            do
                            {
                                //remove da pilha de prioridade e insere na lista PosFixa
                                InsereLista(pilhaPrioridade.Last()); //
                                pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);

                                if (pilhaPrioridade.Count == 0)
                                    break;

                                //verifica se a prioridade do item atual é MAIOR ou IGUAL que o ultimo item, entao desimpilha
                            } while (tabelaPrioridade[item.Simbolo] >= tabelaPrioridade[pilhaPrioridade.Last().Simbolo]);

                            pilhaPrioridade.Add(item);
                        }
                        
                    }
                }
                
            }

        }

        //private void InserePilha(Token item)
        //{
        //    if (pilhaPrioridade.Count == 0 || (item.Simbolo == Simbolo.Sabreparenteses))
        //        pilhaPrioridade.Add(item);
        //    else
        //    {
        //        if (item.Simbolo == Simbolo.Sfechaparenteses)
        //        { //caso seja um fecha parenteses realiza o desempilhamento até encontrar um abre
        //            while (pilhaPrioridade.Last().Simbolo != Simbolo.Sabreparenteses)
        //            {
        //                //remove da pilha de prioridade e insere na lista PosFixa
        //                InsereLista(pilhaPrioridade.Last()); //
        //                pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);

        //            }

        //            //remove da pilha de prioridade o abre parenteses
        //            pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);

        //        }
        //        else
        //        {
        //            // verifica se a prioridade do item atual é MENOR que o ultimo item, entao insere na pilha.
        //            if (tabelaPrioridade[item.Simbolo] < tabelaPrioridade[pilhaPrioridade.Last().Simbolo])
        //                pilhaPrioridade.Add(item);
        //            else
        //            {
        //                do
        //                {
        //                    //remove da pilha de prioridade e insere na lista PosFixa
        //                    InsereLista(pilhaPrioridade.Last()); //
        //                    pilhaPrioridade.RemoveAt(pilhaPrioridade.Count - 1);

        //                    if (pilhaPrioridade.Count == 0)
        //                        break;

        //                    //verifica se a prioridade do item atual é MAIOR ou IGUAL que o ultimo item, entao desimpilha
        //                } while (tabelaPrioridade[item.Simbolo] >= tabelaPrioridade[pilhaPrioridade.Last().Simbolo]);

        //                pilhaPrioridade.Add(item);
        //            }
        //        }

        //    }

        //}

        /// <summary>
        /// método responsável por inserir na lista da formação da PosFixa
        /// </summary>
        /// <param name="item"></param>
        private void InsereLista(Token item)
        {
            listaPosFixa.Add(item);
        }

        /// <summary>
        /// metodo responsável por inserir um token ou na pilha de prioridade ou na lista da pós-fixa
        /// conforme a logica de geração da pós-fixa
        /// </summary>
        /// <param name="item"></param>
        public void InserePosFixa(Token item)
        {
            Token novoItem = item.Clonar();
            // se for um identificador insere direto na lista
            if (novoItem.Simbolo == Simbolo.Sidentificador || 
                novoItem.Simbolo == Simbolo.Snumero || 
                novoItem.Simbolo == Simbolo.Sverdadeiro ||
                novoItem.Simbolo == Simbolo.Sfalso)
                InsereLista(novoItem);
            else
                InserePilha(novoItem);
        }

        /// <summary>
        /// Método destinado a gerar a tabela de prioridade para a lógica
        /// de geração da pós-fixa
        /// </summary>
        private void GerarPrioridade()
        {
            //insere prioridade 1
            tabelaPrioridade.Add(Simbolo.Smaisunario, 1);
            tabelaPrioridade.Add(Simbolo.Smenosunario, 1);
            tabelaPrioridade.Add(Simbolo.Snao, 1);

            //insere prioridade 2
            tabelaPrioridade.Add(Simbolo.Smult, 2);
            tabelaPrioridade.Add(Simbolo.Sdiv, 2);

            //insere prioridade 3
            tabelaPrioridade.Add(Simbolo.Smais, 3);
            tabelaPrioridade.Add(Simbolo.Smenos, 3);

            //insere prioridade 4
            tabelaPrioridade.Add(Simbolo.Sdif, 4);
            tabelaPrioridade.Add(Simbolo.Sig, 4);
            tabelaPrioridade.Add(Simbolo.Smenor, 4);
            tabelaPrioridade.Add(Simbolo.Smenorig, 4);
            tabelaPrioridade.Add(Simbolo.Smaior, 4);
            tabelaPrioridade.Add(Simbolo.Smaiorig, 4);

            //insere prioridade 5
            tabelaPrioridade.Add(Simbolo.Se, 5);

            //insere prioridade 6
            tabelaPrioridade.Add(Simbolo.Sou, 6);

            //insere prioridade 7
            tabelaPrioridade.Add(Simbolo.Sabreparenteses, 7);

        }
    }
}
