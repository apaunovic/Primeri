using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace primer1.Model
{
    public class GasClient
    {
        public string Server { get; set; }
        public int Port { get; set; }
        private TcpClient Client { get; set; }
        private NetworkStream ClientStream { get; set; }
        private StreamReader ClientStreamReader { get; set; }

        public GasClient(string server , int port)
        {
            Server = server;
            Port = port;
           
        }

        public async Task ConnectAsync()
        {
            Client = new TcpClient();
            await Client.ConnectAsync(Server, Port);
            ClientStream = Client.GetStream();
            ClientStreamReader = new StreamReader(ClientStream, Encoding.ASCII);

            var line = await RecvMsgAsync(); //check for error
        }

        public virtual void Dispose()
        {
            if (ClientStream != null) ClientStream.Close();
            if (Client != null) Client.Close();
        }

        public async Task SendMsgAsync(string msg)
        {
            var writeBuffer = Encoding.ASCII.GetBytes(msg);
            await ClientStream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
        }

        public async Task<string> RecvMsgAsync()
        {
            var sb = new StringBuilder();
            string line;
            do
            {
                line = await ClientStreamReader.ReadLineAsync();
                sb.AppendLine(line);
            } while (!string.IsNullOrEmpty(line));
            return sb.ToString();
        }

        public async Task<string> GetResponseAsync(string msg)
        {
            await SendMsgAsync(msg);
            return await RecvMsgAsync();
        }

        public async Task<bool> Ping(int timeoutMilliseconds = 1000)
        {
            var reply = await (new Ping()).SendPingAsync(Server, timeoutMilliseconds);

            return reply != null && reply.Status == IPStatus.Success;
        }

        public async Task<bool> CheckConnected()
        {
            if (Client == null || !Client.Connected)
                return false;

            if (Client.Client.Poll(1, SelectMode.SelectRead) && Client.Client.Available == 0)
                return false;

            return await Ping();
        }
    }
}
