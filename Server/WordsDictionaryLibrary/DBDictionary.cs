using System;
using System.Collections.Generic;
using System.Linq;
using TextHandling;
using InterfaceDictionary;
using EFUserContext;
using EntityWords;
using InterfaceStorage;
using System.IO;

namespace DictionaryLibrary
{
    public class DBDictionary<T> where T : IDictionary
    {
        private Dictionary<string, int> _updateDictionary;

        private UserContext db;

        public DBDictionary(string path)
            {
            db = new UserContext(path);
            }

        public void Create() 
        {
            db.Database.EnsureCreated();
            //Создание файла бд.
        }

        public void Delete()
            //Очищение файла бд.
        {
            db.Words.RemoveRange(db.Words);
            db.SaveChanges();

        }
        public void Update(T review, string path)
        {
            _updateDictionary = review.TextReview(path);
            //Обновление словаря с учетом возможных повторений.                              
            foreach (var d in _updateDictionary)
            {
                if (db.Words.Any(p => p.Word==d.Key))
                {
                    Words w = db.Words.FirstOrDefault(a => a.Word == d.Key);
                    w.Frequency += d.Value;
                    db.Words.Update(w);
                    db.SaveChanges();
                }
                else
                {
                    Words w = new Words { Word = d.Key, Frequency = d.Value };
                    db.Words.Add(w);
                    db.SaveChanges();
                }                
            }
        }

        public IQueryable Search(string searchWord)
        {            
            var words = db.Words.Where(p => p.Word.StartsWith(searchWord));
            words = words.OrderByDescending(f => f.Frequency).ThenBy(f => f.Word).Take(5);
            return words;                     
            
        }
       
    }
}
