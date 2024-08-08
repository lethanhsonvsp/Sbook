using Book.DataModel;
using Book.UI.Service;
using Book.UI.Components;
using Microsoft.EntityFrameworkCore;
using Book.UI.Data;
using Book.UI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpClient<IBooksService, BooksService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5101/");
});

builder.Services.AddSingleton<IBooksService, BooksService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapHub<BooksHub>("/bookshub");

app.Run();
