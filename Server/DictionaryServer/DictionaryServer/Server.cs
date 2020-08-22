using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ConnectionProcessing;
using DictionaryLibrary;
using TextHandling;
using TSPUsage;
using System.Configuration;
using System.IO;

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
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Отсутствует путь к файлу. Невозможно обновить базу данных.");
                }
                catch (IOException e)
                {
                    Console.WriteLine("Невозможно прочитать файл:");
                    Console.WriteLine(e.Message);
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
                TSPServer serv = new TSPServer();
                listener=serv.StartListener(args[1]);               
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {                    
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client, DBIO);
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
