using System;
using System.Collections.Generic;
using System.Text;

namespace InterfaceDictionary
{
    public interface IDictionary
    {      
        Dictionary<string, int> TextReview(string path);
    }
}
