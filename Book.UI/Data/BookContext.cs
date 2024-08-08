using Book.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Book.UI.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
        }

        public DbSet<BookDM> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookDM>().HasKey(b => b.Id);
            modelBuilder.Entity<BookDM>().ToTable("Books");
        }
    }

}
