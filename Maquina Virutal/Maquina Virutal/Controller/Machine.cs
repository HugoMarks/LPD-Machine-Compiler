using Maquina_Virutal.exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Maquina_Virutal
{
    class Machine
    {
        private string conteudoArquivo;
        private string[] linhas;

        private InstrucoesPrograma programa;
        private Type tipoInstrucao;

        public List<LinhaComando> LinhasComando { get; set; }
       
        public bool ModoDebug { get; set; }
        public bool FimExecucao { get; set; }

        public Machine(string arquivo)
        {
            this.conteudoArquivo = arquivo;

        }
        public void inicializar()
        {

            //divide o conteudo de um arquivo em linhas 
            separarLinhas();

            // separa cada cada linha por contexto 
            LinhasComando = new List<LinhaComando>();

            for (int i = 0; i < linhas.Length; i++)
            {
                if (separarContexto(linhas[i], i) != 0)
                {
                    //verifica se obteve um comando vazio e remove
                    if (LinhasComando[i].Comando == String.Empty)
                        LinhasComando.RemoveAt(i);
                }        
            }

            // inicializa instruções de comando e posição do indice da instrução
            programa = new InstrucoesPrograma();
            programa.IndiceProxInstrucao = 0;
            //Obtém o objeto Type da classe System.Type. 
            tipoInstrucao = typeof(InstrucoesPrograma);

        }

        private int separarContexto(string linha, int numLinha)
        {
            List<string> palavras = new List<string>();

            //obtem as palavras de cada linha
            string[] palavrasAuxiliar = linha.Split(new Char[] { ' ', '\t', ',' });

            foreach(string palavra in palavrasAuxiliar)
            {
                if (palavra != String.Empty)
                    palavras.Add(palavra);
            }


            switch (palavras.Count)
            {
                case 0:
                    break;
                case 1: // comandos sem parametro
                    LinhasComando.Add(new LinhaComando(numLinha, palavras[0], ""));
                    break;
                case 2: // comandos com 1 parametro
                    if (palavras[0] == "CALL" || palavras[0] == "JMP" || palavras[0] == "JMPF")
                    {
                        LinhasComando.Add(new LinhaComando(numLinha, palavras[0], palavras[1], ""));
                        break;
                    }
                    if (palavras[1] == "NULL")
                    {
                        LinhasComando.Add(new LinhaComando(numLinha, palavras[1], palavras[0], ""));
                        break;
                    }
                    LinhasComando.Add(new LinhaComando(numLinha, palavras[0], converterParaInteiro(palavras[1]), ""));
                    break;
                case 3: //comandos com 2 parametros 
                    LinhasComando.Add(new LinhaComando(numLinha, palavras[0], converterParaInteiro(palavras[1]), converterParaInteiro(palavras[2]), ""));
                    break;
                default: // gerar uma exception
                    throw new ErroExecucaoException("Arquivo com comandos inválidos!");
            }

            return palavras.Count;
        }

        private void separarLinhas()
        {
            if (conteudoArquivo != null)
            {
                linhas = Regex.Split(conteudoArquivo, "\r\n");
            }

        }

        protected int converterParaInteiro(string numero)
        {
            int valor;
            int.TryParse(numero, out valor);
            return valor;
        }

        private int ConverteLabelEndereco (string label)
        {
            foreach (LinhaComando linha in LinhasComando)
            {
                if (label == linha.Label && linha.Comando == "NULL")
                {
                    return linha.Linha;
                }
            }

            return 0;
        }

        public async Task Executar(VirtualMachine controladoraGrafica)
        {
            bool desvioAtivo = false;
            
            if (LinhasComando.Count > 0)
            {
                
                while (LinhasComando[programa.IndiceProxInstrucao].Comando != "HLT")
                {
                    try
                    {
                        if (this.ModoDebug)
                            controladoraGrafica.SelecionarLinhaExecucao(LinhasComando[programa.IndiceProxInstrucao].Linha);

                        switch (LinhasComando[programa.IndiceProxInstrucao].Tipo)
                        {
                            case 1: // comandos 2 parametros na instrução
                                if (LinhasComando[programa.IndiceProxInstrucao].Comando == "RETURNF")
                                    desvioAtivo = true;

                                tipoInstrucao.InvokeMember(LinhasComando[programa.IndiceProxInstrucao].Comando,
                                                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                                                            null, programa,
                                                            new object[] { LinhasComando[programa.IndiceProxInstrucao].Atributo1, LinhasComando[programa.IndiceProxInstrucao].Atributo2 });
                                break;
                            case 2: // comandos com 1 parametro na instrução
                                tipoInstrucao.InvokeMember(LinhasComando[programa.IndiceProxInstrucao].Comando,
                                                           BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                                                           null, programa,
                                                           new object[] { LinhasComando[programa.IndiceProxInstrucao].Atributo1 });

                                break;
                            case 3: //comandos JMP JMPF CALL NULL
                                    // comandos com 1 parametro na instrução
                                if (LinhasComando[programa.IndiceProxInstrucao].Comando != "NULL")
                                {
                                    tipoInstrucao.InvokeMember(LinhasComando[programa.IndiceProxInstrucao].Comando,
                                                           BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                                                           null, programa,
                                                           new object[] { ConverteLabelEndereco(LinhasComando[programa.IndiceProxInstrucao].Label) });
                                    desvioAtivo = true;
                                }
                                
                                break;
                            case 4: // comandos sem parametro
                                if (LinhasComando[programa.IndiceProxInstrucao].Comando == "RETURN")
                                    desvioAtivo = true;

                                if (LinhasComando[programa.IndiceProxInstrucao].Comando == "RD")
                                {
                                    int valor = await controladoraGrafica.LerEntradaDados();
                                    programa.RD(valor);
                                } 
                                else
                                {
                                    object objeto = tipoInstrucao.InvokeMember(LinhasComando[programa.IndiceProxInstrucao].Comando,
                                                            BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                                                            null, programa,
                                                            null);

                                    if (objeto != null)
                                    {// quando for diferente de null o comando executado foi um PRN
                                        controladoraGrafica.ImprimirDadoSaida((int)objeto);
                                    }

                                }
                                break;
                        }
                    }
                    catch (MissingMethodException)
                    {
                        throw new ErroExecucaoException("Erro na linha: " + LinhasComando[programa.IndiceProxInstrucao].Linha + ". Corrija para continuar!");
                    }
                    catch (Exception)
                    {
                        throw new ErroExecucaoException("Referência de memória não encontrado. Erro na linha: " + LinhasComando[programa.IndiceProxInstrucao].Linha + ".");
                    }

                    controladoraGrafica.CarregarGridPilha(programa.PilhaDados);

                    if (!desvioAtivo)
                    {
                        // ir para a proxima instrução
                        programa.IndiceProxInstrucao += 1;
                    }
                    desvioAtivo = false;
                   
                    if (LinhasComando.Count == programa.IndiceProxInstrucao)
                    {
                        // gera um erro com falta do HTL
                        throw new ErroExecucaoException("Programa sem o terminador de fim de programa!");
                    }

                    //debug: executa só uma instrução
                    if (this.ModoDebug)
                    {
                        break;
                    }

                }
                controladoraGrafica.SelecionarLinhaExecucao(LinhasComando[programa.IndiceProxInstrucao].Linha);
                if (LinhasComando[programa.IndiceProxInstrucao].Comando == "HLT")
                {
                    FimExecucao = true;
                    inicializar();
                }

            }


        }

    }


}
