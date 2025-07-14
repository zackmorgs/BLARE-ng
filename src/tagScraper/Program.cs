using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Data;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using PuppeteerSharp;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static async global::System.Threading.Tasks.Task Main(string[] args)
    {
        // Set up configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        IConfiguration configuration = builder.Build();

        string tagScraperUrl = "https://en.wikipedia.org/wiki/List_of_music_genres_and_styles";

        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        using (
            var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = false,
                    DefaultViewport = new ViewPortOptions { Width = 1280, Height = 800 },
                }
            )
        )
        {
            var page = await browser.NewPageAsync();
            await page.GoToAsync(tagScraperUrl);
            await page.WaitForSelectorAsync("#mw-content-text li .mw-redirect");

            // Extract tags from the page
            var tagsObjects = await page.QuerySelectorAllAsync("#mw-content-text li .mw-redirect");

            var dataContext = new DataContext(configuration);

            var tags = new List<string>();

            foreach (var tagElement in tagsObjects)
            {
                string tagText = await tagElement.EvaluateFunctionAsync<string>("el => el.textContent");
                string processedTag = tagText.Trim()
                    .ToLower()
                    .Replace(" ", "-");
                
                Console.WriteLine(processedTag);
                tags.Add(processedTag);
            }

            foreach (var tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    var musicTag = new MusicTag { Name = tag };
                    await dataContext.MusicTags.InsertOneAsync(musicTag);
                }
            }

            Console.WriteLine("Tags scraped and saved successfully.");
        }
    }
}
