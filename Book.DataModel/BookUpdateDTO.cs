using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataModel
{
    public class BookUpdateDTO
    {
        public required BookDM Book { get; set; }
        public required string Password { get; set; }
    }
}
