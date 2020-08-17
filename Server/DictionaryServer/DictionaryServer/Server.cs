using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ConnectionProcessing;
using DictionaryLibrary;
using TextHandling;
using EFUserContext;
using System.Configuration;

namespace DictionaryServer
{
    class Server
    {
        static TcpListener listener;
        
        static void Main(string[] args)
        {            
            DBDictionary<TextProcessing> DBIO = new DBDictionary<TextProcessing>(args[0]);
            while (true)
            {
                Console.WriteLine("Введите управляющую комаду:");
                string info = Console.ReadLine();
                string[] command = info.Split(' ');
                var a = ConfigurationManager.AppSettings["CreateDictionary"];

                try
                {
                    if (command[0] == ConfigurationManager.AppSettings["CreateDictionary"])
                    {
                        TextProcessing w = new TextProcessing();
                        DBIO.Create();
                        DBIO.Update(w, command[1]);
                        Console.WriteLine("Словарь создан.");
                    }

                    if (command[0] == ConfigurationManager.AppSettings["UpdateDictionary"])
                    {
                        TextProcessing w = new TextProcessing();
                        DBIO.Update(w, command[1]);
                        Console.WriteLine("Обновление завершено.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (command[0] == ConfigurationManager.AppSettings["DeleteDictionary"])
                {
                    DBIO.Delete();
                    Console.WriteLine("Удаление завершено.");
                }

                if (command[0] == ConfigurationManager.AppSettings["StartServer"])
                    break;
                if (command[0] == ConfigurationManager.AppSettings["Exit"])
                    Environment.Exit(0);
            }

            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), int.Parse(args[1]));
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject<DBDictionary<TextProcessing>> clientObject = new ClientObject<DBDictionary<TextProcessing>>(client, DBIO);
                    //Создание потока для нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Autocomplete));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                listener.Stop();
            }
                
            
        }
    }
}
