using Compilador.Business;
using Compilador.Controller;
using Compilador.Exceptions;
using Compilador.Model;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Compilador
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class CompiladorWindow : Window
    {
        private string caminhoArquivo;
        /// <summary>
        /// Método de inicialização da interface gráfica
        /// </summary>
        public CompiladorWindow()
        {
            InitializeComponent();
            ResetarComponentes();

        }

        /// <summary>
        /// método de fechar window quando é realizado um MouseLeftButtonDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FecharAppMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //fecha o app
            this.Close();
        }

        /// <summary>
        /// método de minimizar window quando é realizado um MouseLeftButtonDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizarAppMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //minimiza
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        ///  método de movimentar a window quando é realizado um WindowMouseDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
        /// <summary>
        /// Método usado para realizar a abertura do arquivo quando é realizado um MouseLeftButtonDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbrirArquivoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //cria a caixa de diálogo para abertura do arquivo fonte
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "*.lpd";
            dialog.Filter = "Arquivos LPD (*.lpd)|*.lpd| Todos os arquivos (*.*)|*.*";

            //exibe a caixa de dialogo
            DialogResult result = dialog.ShowDialog();

            if (result.ToString() == "OK")
            {
                ResetarComponentes();

                //obtem diretório do arquivo
                caminhoArquivo = dialog.FileName;

                //mostra na aba o arquivo selecionado
                ((TabItem)tabComandos.Items[0]).Header = System.IO.Path.GetFileName(dialog.FileName);

                //carrega todo o arquivo
                Arquivo arquivo = new Arquivo();
                arquivo.AbrirModoLeitura(caminhoArquivo);
                textBoxComandos.Text = arquivo.LerTodoConteudo();
                arquivo.FechaArquivo();

                //exibe componentes
                image_fundo.Visibility = Visibility.Collapsed;
                label1.Visibility = Visibility.Collapsed;
                label2.Visibility = Visibility.Collapsed;
                tabComandos.Visibility = Visibility.Visible;

                //habilita funcionalidade de compilar
                HabilitarCompilarArquivo();

            }

        }

        /// <summary>
        /// Método usado para realizar a compilação do programa quando é realizado um MouseLeftButtonDown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompilarArquivoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (caminhoArquivo == "")
            {
                System.Windows.MessageBox.Show("Selecione ou abra um arquivo antes de compilar!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // habilita componentes de compilacao 
                //expanderTokens.Visibility = Visibility.Visible;

                //desabilita botão de funcionalidade
                DesabilitarCompilarArquivo();

                //reseta componentes de compilação
                listBoxErros.Items.Clear();
                dataGridTokens.Items.Clear();

                //instancia a classe compilador
                Compilador.Controller.Compilador compiladorControler = new Compilador.Controller.Compilador(caminhoArquivo);
                try
                {
                    //inicia a compilação do código
                    compiladorControler.Compilar();

                    //após a compilação, obtem os tokens gerados para ser exibido
                    //foreach (Token item in compiladorControler.ObterTokens())
                      //  dataGridTokens.Items.Add(item);

                    //destrava o botão compilar
                    HabilitarCompilarArquivo();

                    //mensagem de compilação com exito
                    System.Windows.MessageBox.Show("Código compilado com sucesso! Arquivo disponível em: "+ Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\" + "Arquivos Compilados no CSD", "Aviso", MessageBoxButton.OK, MessageBoxImage.Asterisk);


                }
                catch (ErroException error)
                {
                    tabErros.Visibility = Visibility.Visible;

                    //caso ocorra um erro durante a compilação, é adicionado no componente de erro
                    listBoxErros.Items.Add(error.Message);
                    listBoxErros.SelectedIndex = 0;
                    listBoxErros.Focus();

                    int linhaCodigoSelecionada = int.Parse(error.Data["linha"].ToString());

                    if (linhaCodigoSelecionada == 0)
                        linhaCodigoSelecionada = 1;

                    // seleciona a linha com erro no componente de texto
                    textBoxComandos.ScrollToLine(linhaCodigoSelecionada);
                    DocumentLine line = textBoxComandos.Document.GetLineByNumber(linhaCodigoSelecionada);
                    textBoxComandos.Select(line.Offset, line.Length);

                    // obtem os tokens gerados até o erro para ser exibido
                    //foreach (Token item in compiladorControler.ObterTokens())
                    //    dataGridTokens.Items.Add(item);

                    //destrava o botão compilar
                    HabilitarCompilarArquivo();
                }
            }
        }
        /// <summary>
        /// Método utilizado para criação de um novo arquivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NovoArquivoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // restaura os componentes para criar um novo arquivo
            ResetarComponentes();

            //exibir componentes
            image_fundo.Visibility = Visibility.Collapsed;
            label1.Visibility = Visibility.Collapsed;
            label2.Visibility = Visibility.Collapsed;

            tabComandos.Visibility = Visibility.Visible;

            //exibe um arquivo temporário
            ((TabItem)tabComandos.Items[0]).Header = "NewFile.lpd";

            //habilita componentes
            HabilitarSalvarArquivo();

            //aponta focus do cursor para a tabComandos
            textBoxComandos.Focusable = true;
            Keyboard.Focus(textBoxComandos);

        }

        /// <summary>
        /// Método utilizado para resetar componentes
        /// </summary>
        private void ResetarComponentes()
        {   
            //oculta as imagens
            image_fundo.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Visible;
            label2.Visibility = Visibility.Visible;


            //oculta abas
            tabComandos.Visibility = Visibility.Hidden;
            tabErros.Visibility = Visibility.Hidden;
            expanderTokens.Visibility = Visibility.Hidden;

            textBoxComandos.Options.HighlightCurrentLine = true;
                
            // limpa campos
            textBoxComandos.Text = "";
            listBoxErros.Items.Clear();
            dataGridTokens.Items.Clear();

            // reseta caminho do arquivo
            caminhoArquivo = "";

            //desabilita botões de funcionalidades
            DesabilitarCompilarArquivo();
            DesabilitarSalvarArquivo();

        }

        /// <summary>
        /// método usado para desabilitar o botão compilar
        /// </summary>
        private void DesabilitarCompilarArquivo()
        {
            compilarArquivo.IsEnabled = false;
            compilarArquivo.Source = new BitmapImage(new Uri("pack://application:,,,/Images/startDisable.png"));
        }
        /// <summary>
        /// método usado para habilitar o botão compilar
        /// </summary>
        private void HabilitarCompilarArquivo()
        {
            compilarArquivo.IsEnabled = true;
            compilarArquivo.Source = new BitmapImage(new Uri("pack://application:,,,/Images/start.png"));
        }

        /// <summary>
        /// método usado para desabilitar o botão salvar arquivo
        /// </summary>
        private void DesabilitarSalvarArquivo()
        {
            salvarArquivo.IsEnabled = false;
            salvarArquivo.Source = new BitmapImage(new Uri("pack://application:,,,/Images/saveFileDisable.png"));
        }
        /// <summary>
        /// método usado para habilitar o salvar arquivo
        /// </summary>
        private void HabilitarSalvarArquivo()
        {
            salvarArquivo.IsEnabled = true;
            salvarArquivo.Source = new BitmapImage(new Uri("pack://application:,,,/Images/saveFile.png"));
        }

        
        private void FecharArquivoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResetarComponentes();
        }

        /// <summary>
        /// método utilizado para o armazenamento físico do arquivo para poder ser executado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SalvarArquivoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Arquivo arquivo = new Arquivo();

            // entra nessa condição quando ocorrer a edição de um novo arquivo
            if (caminhoArquivo == "")
            {
                //cria uma caixa de dialogo
                SaveFileDialog arquivoDialog = new SaveFileDialog();
                arquivoDialog.FileName = ((TabItem)tabComandos.Items[0]).Header.ToString(); //preenche com o nome do arquivo sugerido
                arquivoDialog.DefaultExt = "*.lpd";
                arquivoDialog.Filter = "Arquivos LPD (*.lpd)|*.lpd| Todos os arquivos (*.*)|*.*";

                //Exibe a caixa de dialogo e verifica se o processo de salvamento foi confirmado
                if (arquivoDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && arquivoDialog.FileName.Length > 0)
                {
                    //obtem o caminho do arquivo a ser salvo
                    caminhoArquivo = arquivoDialog.FileName;

                    // salva o arquivo no caminho escolhido pelo o usuário
                    arquivo.AbrirModoEscrita(arquivoDialog.FileName);
                    arquivo.EscreverTodoConteudo(textBoxComandos.Text);
                    arquivo.FechaArquivo();
                    
                    //mostra na aba o arquivo selecionado
                    ((TabItem)tabComandos.Items[0]).Header = System.IO.Path.GetFileName(caminhoArquivo);

                    //desabilita o salvar arquivo()
                    DesabilitarSalvarArquivo();

                    //habilita componentes para compilação
                    HabilitarCompilarArquivo();
                }
            }
            else
            { // entra neste caso, somente quando é a edição de um arquivo carregado

                // salva o arquivo no caminho escolhido pelo o usuário
                // salva o arquivo no caminho escolhido pelo o usuário
                arquivo.AbrirModoEscrita(caminhoArquivo);
                arquivo.EscreverTodoConteudo(textBoxComandos.Text);
                arquivo.FechaArquivo();

                //desabilita o salvar arquivo()
                DesabilitarSalvarArquivo();

                //habilita componentes para compilação
                HabilitarCompilarArquivo();
            }
        }
        /// <summary>
        /// método que é executado quando o evento KeyDown é acionado,
        /// este método desabilita a compilação e obrigando a salvar o arquivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxComandosKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            DesabilitarCompilarArquivo();
            HabilitarSalvarArquivo();
        }

        private void textBoxComandosKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                DesabilitarCompilarArquivo();
                HabilitarSalvarArquivo();
            }
        }

        private void BotaoSobre_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string msg = "Compilador CSD criado para a linguagem LPD\n" +
                         "\nIntegrantes: \n" +
                         "- Hugo Marques Casarini - RA:12014742 \n" +
                         "- Thamer El Ayssami         - RA:12133302 \n";
            System.Windows.MessageBox.Show(msg, "Sobre", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
