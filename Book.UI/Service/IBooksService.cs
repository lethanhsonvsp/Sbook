// IBooksService.cs
using Book.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Book.UI.Service
{
    public interface IBooksService
    {
        Task<IEnumerable<BookDM>> GetBooks();
        Task<BookDM> GetBook(int id);
        Task<BookDM> AddBook(BookDM book);
        Task<BookDM> UpdateBook(BookDM book);
        Task DeleteBook(int id);
    }
}
