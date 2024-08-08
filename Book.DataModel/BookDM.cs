using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataModel
{
    public class BookDM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; // Gán giá trị mặc định
        public string Author { get; set; } = string.Empty; // Gán giá trị mặc định
        public int Year { get; set; }

        public BookDM()
        {
        }

        public BookDM(int id, string title, string author, int year)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
        }
    }

}
