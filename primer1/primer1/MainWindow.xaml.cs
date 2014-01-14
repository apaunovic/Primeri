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
//using System.Threading.Tasks;
using primer1.Model;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace primer1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GasClient GasServerClient { get; set; }
        //private Task activeTask;
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

        private void buttonMessage_Click(object sender, RoutedEventArgs e)
        {
            var errors = XElement.Load("C:/Users/Grid/ew.xml");

            var eCount = errors.Elements().Where(el => "error".Equals(el.Attribute("type").Value)).Count();
            var wCount = errors.Elements().Where(el => "warning".Equals(el.Attribute("type").Value)).Count();

            readBox.Text=errors.ToString();

            eBox.Text = " E:" + eCount + " -- W:" + wCount;

           
            
        }

        private void buttonErrors_Click(object sender, RoutedEventArgs e)
        {
            var errors = XElement.Load("C:/Users/Grid/Primeri/ew.xml");

            /*var items = new List<string>();

            foreach (XElement x in errors.Elements().Where(el => "error".Equals(el.Attribute("type").Value)))
                {
                    items.Add("Na liniji " + x.Attribute("line").Value.ToString() + " u fajlu " + x.Attribute("file").Value.ToString() + " postoji " + x.Attribute("text").Value.ToString());           
                }
    
            lista.DataContext = items;*/

            lista.DataContext = errors.Elements().Where(el => "error".Equals(el.Attribute("type").Value));
        }

        private void buttonWarnings_Click(object sender, RoutedEventArgs e)
        {
            var errors = XElement.Load("C:/Users/Grid/ew.xml");
            
            /*var items = new List<string>();

            foreach (XElement x in errors.Elements().Where(el => "warning".Equals(el.Attribute("type").Value)))
            {
                items.Add("Na liniji " + x.Attribute("line").Value.ToString() + " u fajlu " + x.Attribute("file").Value.ToString() + " postoji " + x.Attribute("text").Value.ToString());
            }

            lista.DataContext = items;*/

            lista.DataContext = errors.Elements().Where(el => "warning".Equals(el.Attribute("type").Value));
        }
    }
}
