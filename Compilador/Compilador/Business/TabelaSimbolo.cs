using Compilador.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Business
{
    public class TabelaSimbolo
    {
        private List<ItemSimbolo> pilhaItens;

        public TabelaSimbolo ()
        {
            pilhaItens = new List<ItemSimbolo>();
        }

        /// <summary>
        /// Método que realiza a inserção na tabela de simbolos
        /// </summary>
        /// <param name="novoItem"></param>
        public void InserirTabela (ItemSimbolo novoItem)
        {
            pilhaItens.Add(novoItem);
        }

        /// <summary>
        /// Método destinado a pesquisar um item Simbolo e retornar o item que encontrou
        /// </summary>
        /// <param name="lexema"></param>
        /// <returns></returns>
        public ItemSimbolo PesquisaItem(String lexema)
        {
            ItemSimbolo novoItem = null;
            foreach (ItemSimbolo item in pilhaItens.Reverse<ItemSimbolo>())
            {
                if (item.Lexema == lexema)
                    return novoItem = item;
            }

            return novoItem;
        }

        /// <summary>
        /// Método destinado a Pesquisar um simbolo para ser retornado
        /// </summary>
        /// <param name="lexema"></param>
        /// <returns></returns>
        public Simbolo PesquisaSimbolo(String lexema)
        {
            Simbolo simbolo = new Simbolo();

            foreach (ItemSimbolo item in pilhaItens.Reverse<ItemSimbolo>())
            {
                if (item.Lexema == lexema)
                {
                    if (item.GetType() == typeof(VariavelSimbolo))
                        return simbolo = ConverteParaSimbolo(((VariavelSimbolo)item).TipoVariavel);


                    if (item.GetType() == typeof(FuncaoSimbolo))
                        return simbolo = ConverteParaSimbolo(((FuncaoSimbolo)item).TipoFuncao);
                }
            }

            return simbolo;
        }

        /// <summary>
        /// metodo que pesquisa variável podendo ter 2 tipos de pesquisa:
        ///  pesquisarDuplicidade = true - pesquisa a duplicidade durante a delcaracao de variaveis
        /// pesquisarDuplicidade = false - pesquisa se a variavel foi declarada dentro e fora do escopo
        /// retorna true caso encontre a variavel == lexema
        /// </summary>
        /// <param name="lexema"></param>
        /// <param name="pesquisarDuplicidade"></param>
        /// <returns></returns>
        public bool PesquisarVariavel(String lexema, bool pesquisarDuplicidade)
        {
            bool foraEscopo = false;
            
            foreach (ItemSimbolo item in pilhaItens.Reverse<ItemSimbolo>())
            {
                if (foraEscopo) //a pesquisa ocorre verificando se o lexema é igual a nome de procedimento, função ou programa
                {
                    if (pesquisarDuplicidade)
                    {
                        if (item.GetType() != typeof(VariavelSimbolo))
                        {
                            if (item.Lexema == lexema)
                                return true;
                        }
                    }
                    else
                    { //a pesquisa ocorre verificando se o lexema é igual a uma variavel declarada em todo o escopo do sistema
                        if (item.GetType() == typeof(VariavelSimbolo))
                        {
                            if (item.Lexema == lexema)
                                return true;
                        }
                    }
                    
                }
                else
                {  // a pesquisa ocorre somente no escopo
                    if (item.GetType() == typeof(VariavelSimbolo))
                    {
                        if (item.Lexema == lexema)
                            return true;
                    }
                    else
                        foraEscopo = true;
                }
               
            }
            return false; // caso não tenha itens na pilha ou não encontrou duplicidade
        }

        /// <summary>
        /// Método a destinado a colocar o tipo em funções ou variáveis sem tipo ainda
        /// </summary>
        /// <param name="simbolo"></param>
        public void ColocarTipo(Simbolo simbolo)
        {   //verifica se o tipo é função, 
            //se for o metodo ColocaTipo foi chamado no analisa_declaração_função
            //senão o metodo ColocaTipo foi chamado no analisa_tipo

            Tipo novoTipo = ConverteParaTipo(simbolo);

            if (pilhaItens.Last().GetType() == typeof(FuncaoSimbolo))
            {
                ((FuncaoSimbolo)pilhaItens.Last()).TipoFuncao = novoTipo;
            }
            else
            {
                foreach (ItemSimbolo item in pilhaItens.Reverse<ItemSimbolo>())
                {
                    //verifica se o tipo do item é variável
                    if (item.GetType() == typeof(VariavelSimbolo))
                    {
                        //verifica se o tipo da variavel não foi preenchido
                        if (((VariavelSimbolo)item).TipoVariavel == Tipo.semTipo)
                        {
                            ((VariavelSimbolo)item).TipoVariavel = novoTipo;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            
        }

        /// <summary>
        /// Método a destinado a remover todo conteudo declarado dentro de um escopo
        /// </summary>
        /// <param name="nivel"></param>
        public void DesimpilharNivel(int nivel)
        {
            //percorre a tabela desimpilhando até achar o nivel informado
            foreach (ItemSimbolo item in pilhaItens.Reverse<ItemSimbolo>())
            {
                if (item.GetType() == typeof(ProcedimentoSimbolo))
                {
                    if (((ProcedimentoSimbolo)item).Nivel == nivel)
                        break;
                }

                if (item.GetType() == typeof(FuncaoSimbolo))
                {
                    if (((FuncaoSimbolo)item).Nivel == nivel)
                        break;
                }
                pilhaItens.Remove(item);
            }
        }

        /// <summary>
        /// método destinado a realizar a conversão de simbolo para tipo
        /// </summary>
        /// <param name="simbolo"></param>
        /// <returns></returns>
        public Tipo ConverteParaTipo (Simbolo simbolo)
        {
            Tipo tipo = new Tipo();

            if (simbolo == Simbolo.Sinteiro)
                tipo = Tipo.inteiro;

            if (simbolo == Simbolo.Sbooleano)
                tipo = Tipo.booleano;

            //se nao for inteiro ou booleano o tipo vai ser sem tipo

            return tipo;   
        }

        /// <summary>
        /// método destinado a realizar a conversão de tipo para Simbolo
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public Simbolo ConverteParaSimbolo(Tipo tipo)
        {
            Simbolo simbolo = new Simbolo();

            //se nao for inteiro ou booleano o simbolo vai ser sem token
            simbolo = Simbolo.Ssemtoken;

            if (tipo == Tipo.inteiro)
                simbolo = Simbolo.Sinteiro;

            if (tipo == Tipo.booleano)
                simbolo = Simbolo.Sbooleano;

            return simbolo;
        }
    }
}
