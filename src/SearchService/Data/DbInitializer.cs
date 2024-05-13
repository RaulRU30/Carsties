using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));

        await DB.Index<Item>()
            .Key(a => a.Make, KeyType.Text)
            .Key(a => a.Model, KeyType.Text)
            .Key(a => a.Year, KeyType.Text)
            .Key(a => a.Color, KeyType.Text)
            .Key(a => a.Mileage, KeyType.Text)
            .Key(a => a.CreatedAt, KeyType.Text)
            .Key(a => a.UpdatedAt, KeyType.Text)
            .Key(a => a.AuctionEnd, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();

        Console.WriteLine($"Items count from AuctionService: {items.Count}");

        if (items.Count > 0) await DB.SaveAsync(items);


    }
}