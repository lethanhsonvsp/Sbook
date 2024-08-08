using Book.DataModel;
using Book.UI.Data;

namespace Book.UI.Service
{
    public class BooksService : IBooksService
    {
        private readonly HttpClient _httpClient;
        private readonly BookContext _context;

        public BooksService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5265/");
        }

        public async Task<IEnumerable<BookDM>> GetBooks()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BookDM>>("api/Books") ?? Array.Empty<BookDM>();
        }

        public async Task<BookDM> GetBook(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookDM>($"api/Books/{id}");
        }

        public async Task<BookDM> AddBook(BookDM book)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Books", book);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BookDM>();
        }

        public async Task<BookDM> UpdateBook(BookDM book)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Books/{book.Id}", book);
            response.EnsureSuccessStatusCode();

            // Thêm kiểm tra nội dung
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("API returned empty response");
            }

            return await response.Content.ReadFromJsonAsync<BookDM>();
        }

        public async Task DeleteBook(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Books/{id}");
            response.EnsureSuccessStatusCode();
        }

        private async Task SaveBooksToDatabase(IEnumerable<BookDM> books)
        {
            foreach (var book in books)
            {
                var existingBook = await _context.Books.FindAsync(book.Id);
                if (existingBook == null)
                {
                    _context.Books.Add(book);
                }
                else
                {
                    _context.Entry(existingBook).CurrentValues.SetValues(book);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}