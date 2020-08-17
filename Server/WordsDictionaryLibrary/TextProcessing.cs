using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InterfaceDictionary;

namespace TextHandling
{
    public class TextProcessing : IDictionary
    {
        
        private const int minLength = 3, maxLength = 15;
        private string[] ReadText(string path)
        {
            try
            {
                //Создадим объект класса для считывания файла
                using StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8, false);
                //Считаем файл и разобъем его на слова
                string line = sr.ReadToEnd().ToLower();
                var word = line.Split(' ', '.', ',', ':', ';', '-','(',')','\n','\r');               
                return word;
            }
            catch (Exception e)
            {
                // Выведем ошибку если произойдет.
                Console.WriteLine("Невозможно прочитать файл:");
                Console.WriteLine(e.Message);
                Environment.Exit(0);
                string[] exWord = new string[1];                
                return exWord;
            }

        }

        private Dictionary<string,int> GroupToDictionary(string[] words)
        {   
            //Сгруппируем в словарь слова и количество их повторений.
            IEnumerable<string> unicWords = from w in words
                                            where w.Length >= minLength & w.Length <= maxLength                                                                                                          
                                            select w;
            unicWords = unicWords.Distinct();
            Dictionary<string, int> unsortedWords = new Dictionary<string, int>();
            foreach (var w in unicWords)
            {
                if (words.Count(p => p == w) >=3)
                    unsortedWords.Add(w, words.Count(p => p == w));
            }
            return unsortedWords;
        }

        private Dictionary<string, int> SortDictionary(Dictionary<string, int> words)
        {
            //Отсортируем выборку по кол-ву повторений, а после по алфавиту.
            words = words.OrderByDescending(w => w.Value).ThenBy(w => w.Key).ToDictionary(w => w.Key, w => w.Value);

            return words;
        }
        
        public Dictionary<string, int> TextReview(string path)
        {
            Dictionary<string, int> sortWords = SortDictionary(GroupToDictionary(ReadText(path)));            
            return sortWords;
        }
    }
}
