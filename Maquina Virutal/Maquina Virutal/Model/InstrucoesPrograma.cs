using Maquina_Virutal.exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maquina_Virutal
{
    class InstrucoesPrograma
    {
        /*
        Para todas os metedos ter os seguintes parametros:
        1º Lista da pilha de dados passada por referencia utilizando a tag REF;
        2º Indice do topo da pilha por refencia também.
        */

        /* Sumario das intruções a serem implementadas 
        LDC k (Carregar constante):  OK
        LDV n (Carregar valor): OK
         ADD (Somar): OK
        SUB (Subtrair): OK
        MULT (Multiplicar): OK
        DIVI (Dividir): OK (só inteiro)
        INV (Inverter sinal): OK
        AND (Conjunção): OK
        OR (Disjunção): OK
        NEG (Negação): NOT TESTED
        CME (Comparar menor): NOT TESTED
        CMA (Comparar maior): NOT TESTED
        CEQ (Comparar igual): NOT TESTED
        CDIF (Comparar desigual): NOT TESTED
        CMEQ (Comparar menor ou igual): NOT TESTED
        CMAQ (Comparar maior ou igual): NOT TESTED
        START (Iniciar programa principal): OK
        HLT (Parar): 

        --Atribuição
        STR n (Armazenar valor):
        JMP t (Desviar sempre):
        JMPF t (Desviar se falso):

        --Operação Nula
        NULL (Nada)
        RD (Leitura):

        --Saída
        PRN (Impressão):

        --Alocação e Desalocação de Variáveis
        ALLOC m,n (Alocar memória):
        DALLOC m,n (Desalocar memória):

        --Chamada de Rotina
        CALL t (Chamar procedimento ou função):
        RETURN (Retornar de procedimento):
        RETURNF

        */

        public int IndiceTopoPilhaDados { get; set; }
        public List<int> PilhaDados { get; set; }

        public int IndiceProxInstrucao { get; set; }

        public InstrucoesPrograma()
        {
            this.PilhaDados = new List<int>();
        }
        //(Iniciar programa principal)
        public void START()
        {
            decrementaTopo();
        }

        //(Carregar constante)
        public void LDC(int constante)
        {
            incrementaTopo();
            this.PilhaDados[IndiceTopoPilhaDados] = constante;
        }

        //(Carregar valor)
        public void LDV(int endereco)
        {
            incrementaTopo();
            this.PilhaDados[IndiceTopoPilhaDados] = PilhaDados[endereco];
        }

        //(Somar)
        public void ADD()
        {
            this.PilhaDados[IndiceTopoPilhaDados-1] = PilhaDados[IndiceTopoPilhaDados - 1] + PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
        }
        //Subtrair
        public void SUB()
        {
            this.PilhaDados[IndiceTopoPilhaDados - 1] = PilhaDados[IndiceTopoPilhaDados - 1] - PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
        }
        //Multiplicar
        public void MULT()
        {
            this.PilhaDados[IndiceTopoPilhaDados - 1] = PilhaDados[IndiceTopoPilhaDados - 1] * PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
        }
        //Dividir
        public void DIVI()
        {
            this.PilhaDados[IndiceTopoPilhaDados - 1] = PilhaDados[IndiceTopoPilhaDados - 1] / PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
        }
        //Inverter
        public void INV()
        {
            this.PilhaDados[IndiceTopoPilhaDados] = this.PilhaDados[IndiceTopoPilhaDados] * (-1);
        }
        //AND
        public void AND()
        {
            if(this.PilhaDados[IndiceTopoPilhaDados - 1] == 1 && this.PilhaDados[IndiceTopoPilhaDados] == 1)
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //OR
        public void OR()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] == 1 || this.PilhaDados[IndiceTopoPilhaDados] == 1)
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //Negação
        public void NEG()
        {
            this.PilhaDados[IndiceTopoPilhaDados] = 1 - this.PilhaDados[IndiceTopoPilhaDados];
        }
        //Comparar menor
        public void CME()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] < this.PilhaDados[IndiceTopoPilhaDados])
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //Comparar maior
        public void CMA()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] > this.PilhaDados[IndiceTopoPilhaDados])
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //Comparar igual
        public void CEQ()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] == this.PilhaDados[IndiceTopoPilhaDados])
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //Comparar desigual
        public void CDIF()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] != this.PilhaDados[IndiceTopoPilhaDados])
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //Comparar menor ou igual
        public void CMEQ()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] <= this.PilhaDados[IndiceTopoPilhaDados])
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }
        //Comparar maior ou igual
        public void CMAQ()
        {
            if (this.PilhaDados[IndiceTopoPilhaDados - 1] >= this.PilhaDados[IndiceTopoPilhaDados])
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 1;
            }
            else
            {
                this.PilhaDados[IndiceTopoPilhaDados - 1] = 0;
            }
            decrementaTopo();
        }

     
        //Armazenar valor
        public void STR(int posicao)
        {
            this.PilhaDados[posicao] = PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
        }

        //Desviar sempre
        public void JMP(int endereco)
        {
            IndiceProxInstrucao = endereco;
        }

        //Desviar se falso
        public void JMPF(int endereco)
        {
            if (PilhaDados[IndiceTopoPilhaDados] == 0)
                IndiceProxInstrucao = endereco;
            else
                IndiceProxInstrucao++;
            decrementaTopo();
        }

        //leitura
        public void RD(int valor)
        {
            incrementaTopo();
            PilhaDados[IndiceTopoPilhaDados] = valor; 
        }

        //saída
        public int PRN()
        {
            int valor = PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();

            return valor;
        }

        //Alocação de Variáveis
        public void ALLOC(int inicio, int quantidade)
        {

            for(int i=0; i< quantidade;i++)
            {
                incrementaTopo();
                PilhaDados[IndiceTopoPilhaDados] = PilhaDados[inicio + i];
            }
        }

        //Desalocação de Variáveis
        public void DALLOC(int inicio, int quantidade)
        {
            quantidade--;
            for (int i = quantidade; i >= 0; i--)
            {
                PilhaDados[inicio + i] = PilhaDados[IndiceTopoPilhaDados];
                decrementaTopo();
            }
        }

        //Chamada de Rotina
        public void CALL(int endereco)
        {
            incrementaTopo();
            PilhaDados[IndiceTopoPilhaDados] = IndiceProxInstrucao + 1;
            IndiceProxInstrucao = endereco;
        }

        //Retornar de procedimento
        public void RETURN()
        {
            IndiceProxInstrucao = PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
        }

        //Retornar de função
        public void RETURNF(int inicio, int quantidade)
        {
            //armazena o valor de retorno da função
            int resgistrador = PilhaDados[IndiceTopoPilhaDados];
            decrementaTopo();
            //desaloca as variáveis
            DALLOC(inicio, quantidade);
            //altera o endereço da próxima instrução
            RETURN();
            //devolve ao topo o valor de retorno da função
            incrementaTopo();
            PilhaDados[IndiceTopoPilhaDados] = resgistrador;
        }

        private void incrementaTopo()
        {
            IndiceTopoPilhaDados += 1;
            this.PilhaDados.Add(0);
        }

        private void decrementaTopo()
        {
            if (PilhaDados.Count != 0)
                this.PilhaDados.RemoveAt(IndiceTopoPilhaDados);

            IndiceTopoPilhaDados -= 1;

        }

    }

}
