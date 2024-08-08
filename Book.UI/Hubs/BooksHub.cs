using Book.DataModel;
using Microsoft.AspNetCore.SignalR;

namespace Book.UI.Hubs
{
    public class BooksHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task BookAdded(BookDM book)
        {
            await Clients.All.SendAsync("BookAdded", book);
        }

        public async Task BookUpdated(BookDM book)
        {
            await Clients.All.SendAsync("BookUpdated", book);
        }

        public async Task BookDeleted(int bookId)
        {
            await Clients.All.SendAsync("BookDeleted", bookId);
        }

    }
}
