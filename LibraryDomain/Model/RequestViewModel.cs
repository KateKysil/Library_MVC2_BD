using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Model
{
    public class RequestViewModel
    {
        public string RequestTitle { get; set; }
        public List<string> Headers { get; set; } = new();
        public List<List<string>> Results { get; set; } = new();
    }

}
