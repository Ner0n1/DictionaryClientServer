using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceStorage

{
    public interface IStorage
    {
        IQueryable Search(string searchWord);
    }
}
