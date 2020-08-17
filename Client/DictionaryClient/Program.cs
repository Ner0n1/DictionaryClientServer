using System.Configuration;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DictionaryClient
{
    class Program
    {      
        static void Main(string[] args)
        {
            Console.Write("Введите свое имя:");
            string userName = Console.ReadLine();
            TcpClient client = null;
            //IPHostEntry host = Dns.GetHostEntry(args[0]);
            int port = int.Parse(args[1]);

            try
            {
                client = new TcpClient(args[0], port);
                NetworkStream stream = client.GetStream();
               
                while (true)
                {
                    Console.Write(userName + ": ");
                    // ввод сообщения                   
                    string message = Console.ReadLine();                    
                    if (message == "")
                        Environment.Exit(0);
                    message.ToLower();                                   

                    string[] splitMessage = message.Split();

                    if(splitMessage[0]== ConfigurationManager.AppSettings["Disconnect"])
                        Environment.Exit(0);

                    if (splitMessage[0] == ConfigurationManager.AppSettings["GetWords"])
                    {
                        message = String.Format("{0}: {1}", userName, message);
                        // преобразуем сообщение в массив байтов
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        // отправка сообщения
                        stream.Write(data, 0, data.Length);

                        // получаем ответ
                        data = new byte[64]; // буфер для получаемых данных
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        message = builder.ToString();
                        Console.WriteLine("Сервер: {0}", message);
                    }
                    else
                    {
                        Console.WriteLine("Неизвестная команда");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
    
