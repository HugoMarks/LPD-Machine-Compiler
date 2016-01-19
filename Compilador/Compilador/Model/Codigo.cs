using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.Model
{
    public class Codigo
    {
        public const String LDC = "LDC"; //(Carregar constante)

        public const String LDV = "LDV"; //(Carregar valor)

        public const String ADD = "ADD"; //(Somar)

        public const String SUB = "SUB"; //(Subtrair)

        public const String MULT = "MULT"; //(Multiplicar)

        public const String DIVI = "DIVI"; //(Dividir)

        public const String INV = "INV"; //(Inverter sinal)

        public const String AND = "AND"; //(Conjunção)
        public const String OR = "OR"; // (Disjunção)
        public const String NEG = "NEG"; // (Negação)
        public const String CME = "CME"; // (Comparar menor)
        public const String CMA = "CMA"; // (Comparar maior)
        public const String CEQ = "CEQ"; // (Comparar igual)
        public const String CDIF = "CDIF"; // (Comparar desigual)
        public const String CMEQ = "CMEQ"; // (Comparar menor ou igual)
        public const String CMAQ = "CMAQ"; // (Comparar maior ou igual)
        public const String START = "START"; // (Iniciar programa principal)
        public const String HLT = "HLT"; // (Parar) 

        //Atribuição
        public const String STR = "STR"; //(Armazenar valor)
        public const String JMP = "JMP"; // t (Desviar sempre)
        public const String JMPF = "JMPF";  // t (Desviar se falso)

        //Operação Nula
        public const String NULL = "NULL";  // (Nada)
        public const String RD = "RD"; // (Leitura)

        //Saída
        public const String PRN = "PRN";  // (Impressão)

        //Alocação e Desalocação de Variáveis
        public const String ALLOC = "ALLOC"; //  m,n (Alocar memória)
        public const String DALLOC = "DALLOC"; //  m,n (Desalocar memória)

        //Chamada de Rotina
        public const String CALL = "CALL"; // (Chamar procedimento ou função)
        public const String RETURN = "RETURN";  //(Retornar de procedimento)
        public const String RETURNF = "RETURNF";  //(Retornar de função)
    }
}
