using Maquina_Virutal.exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Maquina_Virutal
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class VirtualMachine : Window
    {

        public string DragWindowWithOneTab { get; private set; }
        private Machine maquina;
        private TaskCompletionSource<bool> dadoFoiInformado;

        

        public VirtualMachine()
        {
            InitializeComponent();
            //inicializar maquina virtual

            OcultarComponentes();
        }

        private void imageFechar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //fecha o app
            this.Close();
        }

        private void imageMinimizar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //minimiza
            this.WindowState = WindowState.Minimized;
        }

        private void imageLoadFile_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OcultarComponentes();
            ZeraComponanentes();


            //botão para carregar o arquivo e exibir na tela
            Folder diretorio = new Folder();
       
            if (diretorio.ConteudoLido)
            {
                MostrarComponanentes();
                try
                {
                    //inicializa a maquina virtual
                    maquina = new Machine(diretorio.conteudo);
                    maquina.inicializar();

                    // apos inicializar carrega o grid view
                    LinhaComandoView linhaTabela;
                    foreach (LinhaComando linha in maquina.LinhasComando)
                    {
                        linhaTabela = new LinhaComandoView(linha);
                        DataGridInstrucoes.Items.Add(linhaTabela);
                    }
                }
                catch (Exception error)
                {
                    System.Windows.MessageBox.Show(error.Message, "Erro Com o Arquivo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private async void Executar_Click(object sender, MouseButtonEventArgs e)
        {
            // roda a execução dos comandos
            try
            {
                if (maquina.FimExecucao)
                {
                    listViewSaida.Items.Clear();
                    dataGridPilha.Items.Clear();
                    maquina.FimExecucao = false;
                }
                maquina.ModoDebug = false;
                await maquina.Executar(this as VirtualMachine);
            }
            catch (ErroExecucaoException error)
            {
                System.Windows.MessageBox.Show(error.Message,"Erro durante a Execução",MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        public void SelecionarLinhaExecucao(int linha)
        {

            Keyboard.Focus(DataGridInstrucoes);
            DataGridRow row = (DataGridRow)DataGridInstrucoes.ItemContainerGenerator.ContainerFromIndex(linha);
            object item = DataGridInstrucoes.Items[linha];
            DataGridInstrucoes.SelectedItem = item;
            DataGridInstrucoes.ScrollIntoView(item);
           // DataGridInstrucoes.SelectedIndex = linha;
            // row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //row.Focusable = true;
        }

        public void ImprimirDadoSaida(int dado)
        {
            listViewSaida.Items.Add(dado);
            //listViewSaida.ScrollIntoView(dado);
            listViewSaida.SelectedIndex = listViewSaida.Items.Count - 1;        

        }

        public async Task<int> LerEntradaDados()
        {

            dadoFoiInformado = new TaskCompletionSource<bool>();

            TextBoxEntradaDados.Text = null;
            DialogData.Visibility = System.Windows.Visibility.Visible;
            TextBoxEntradaDados.Focusable = true;
            Keyboard.Focus(TextBoxEntradaDados);

            await dadoFoiInformado.Task;

            return Convert.ToInt32(TextBoxEntradaDados.Text);
        }
        private void mbox_ok(object sender, RoutedEventArgs e)
        {
            if (TextBoxEntradaDados.Text != String.Empty)
            {
                DialogData.Visibility = Visibility.Collapsed;
                dadoFoiInformado.SetResult(true); //libera o método que chamou
            }
            else
                Keyboard.Focus(TextBoxEntradaDados);

        }

        private void ValidaTextBoxEntrada(object sender, TextCompositionEventArgs e)
        {
           Regex regex = new Regex(@"^(\-?[0-9]*)$|^([0-9]*)$");

            string texto;
            if (TextBoxEntradaDados.Text.Length == 0)
                texto = e.Text; //obtem apenas o caracter digitado
            else
                texto = TextBoxEntradaDados.Text + e.Text; //obtem o texto + o caracter digitado

            bool result = regex.IsMatch(texto);
            e.Handled = !result;
        }

        public void CarregarGridPilha(List<int> pilha)
        {
            dataGridPilha.Items.Clear();
            int endereco = 0;

            foreach (int item in pilha)
            {
                dataGridPilha.Items.Add(new DadosView(endereco,item));
                endereco++;
            }
                
        }
        private void OcultarComponentes()
        {
            image_fundo.Visibility = Visibility.Visible;
            labelMain.Visibility = Visibility.Visible;

            //BotaoExecutar.Visibility = Visibility.Hidden;
            //BotaoDebug.Visibility = Visibility.Hidden;
            expander_comando.IsExpanded = false;
            expander_dados.IsExpanded = false;
            expander_saida.IsExpanded = false;

            expander_comando.IsEnabled = false;
            expander_dados.IsEnabled = false;
            expander_saida.IsEnabled = false;

            DesabilitarExecutar();
            DesabilitarFecharArquivo();
        }

        private void MostrarComponanentes()
        {
            image_fundo.Visibility = Visibility.Hidden;
            labelMain.Visibility = Visibility.Hidden;

            //BotaoExecutar.Visibility = Visibility.Visible;
            //BotaoDebug.Visibility = Visibility.Visible;
            expander_comando.IsExpanded = true;
            expander_dados.IsExpanded = true;
            expander_saida.IsExpanded = true;

            expander_comando.IsEnabled = true;
            expander_dados.IsEnabled = true;
            expander_saida.IsEnabled = true;

            HabilitarExecutar();
            HabilitarFecharArquivo();
        }
        private void DesabilitarExecutar()
        {
            BotaoExecutar.IsEnabled = false;
            BotaoExecutar.Source = new BitmapImage(new Uri("pack://application:,,,/Images/startDisable.png"));

            BotaoDebug.IsEnabled = false;
            BotaoDebug.Source = new BitmapImage(new Uri("pack://application:,,,/Images/startDisable.png"));

        }

        private void HabilitarExecutar()
        {
            BotaoExecutar.IsEnabled = true;
            BotaoExecutar.Source = new BitmapImage(new Uri("pack://application:,,,/Images/start.png"));

            BotaoDebug.IsEnabled = true;
            BotaoDebug.Source = new BitmapImage(new Uri("pack://application:,,,/Images/startDebug.png"));
        }

        private void DesabilitarFecharArquivo()
        {
            fecharArquivo.IsEnabled = false;
            fecharArquivo.Source = new BitmapImage(new Uri("pack://application:,,,/Images/closeFileDisable.png"));
        }

        private void HabilitarFecharArquivo()
        {
            fecharArquivo.IsEnabled = true;
            fecharArquivo.Source = new BitmapImage(new Uri("pack://application:,,,/Images/closeFile.png"));
        }
        private void ZeraComponanentes()
        {
            DataGridInstrucoes.Items.Clear();
            dataGridPilha.Items.Clear();
            listViewSaida.Items.Clear();

        }


        private async void Executar_Debug_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (maquina.FimExecucao)
                {
                    listViewSaida.Items.Clear();
                    dataGridPilha.Items.Clear();
                    maquina.FimExecucao = false;
                }
                maquina.ModoDebug = true;
                await maquina.Executar(this as VirtualMachine);
            }
            catch (ErroExecucaoException error)
            {
                System.Windows.MessageBox.Show(error.Message, "Erro durante a Execução", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void fecharArquivo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OcultarComponentes();
        }

        private void BotaoSobre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string msg = "Compilador CSD criado para a linguagem LPD\n"+
                         "\nIntegrantes: \n"+
                         "- Hugo Marques Casarini - RA:12014742 \n"+
                         "- Thamer El Ayssami         - RA:12133302 \n";
            System.Windows.MessageBox.Show(msg, "Sobre", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
