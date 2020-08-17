using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using DictionaryLibrary;
using EntityWords;
using InterfaceStorage;
using TextHandling;

namespace ConnectionProcessing
{
    public class ClientObject<T> where T:IStorage
    {
        public TcpClient client;

        public T DBIO;
        public ClientObject(TcpClient tcpClient, T db)
        {
            client = tcpClient;
            DBIO = db;
        }

       
        public void Autocomplete()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    Console.WriteLine(message);
                    message = message.ToLower();
                    string[] splitMessage = message.Split(' ');
                    var result = DBIO.Search(splitMessage[2]);
                    string serverAnswer = string.Empty;
                    foreach (Words w in result)
                    {
                        serverAnswer += w.Word + "\n";
                    }
                    if (serverAnswer == string.Empty)
                        serverAnswer = "Совпадений не найдено";
                    data = Encoding.UTF8.GetBytes(serverAnswer);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
