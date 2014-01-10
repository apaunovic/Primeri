using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using primer1.Model;

namespace primer1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GasClient GasServerClient { get; set; }
        private Task activeTask;
        CancellationTokenSource t;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            GasServerClient = new GasClient("192.168.5.55", 43976);
            await GasServerClient.ConnectAsync();
            statusBox.Text = "Connected";
        }

        private async void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            t = new CancellationTokenSource();
            try
            {
                readBox.Text = await TaskCancellation.WithCancellation(GasServerClient.GetResponseAsync(writeBox.Text + "\n\n"), t.Token);
            }
            catch (OperationCanceledException ex)
            {
                readBox.Text = "Action cancelled";
            }     
            //readBox.Text = await TaskCancellation.WithCancellation(GasServerClient.GetResponseAsync(writeBox.Text + "\n\n"), t.Token);
            //readBox.Text = await GasServerClient.GetResponseAsync(writeBox.Text + "\n\n");
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
                t.Cancel();             
         }
    }
}
