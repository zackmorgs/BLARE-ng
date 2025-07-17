using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Configuration;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using PuppeteerSharp;
using Slugify;

internal class Program
{
    private static async global::System.Threading.Tasks.Task Main(string[] args)
    {
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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Adjust if needed
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var dataContext = new DataContext(configuration);

            var tags = new List<string>();
            SlugHelper helper = new SlugHelper();

            foreach (var tagDom in tagsObjects)
            {
                string tag = tagDom
                    .EvaluateFunctionAsync<string>("el => el.textContent")
                    .Result.Trim()
                    .ToLower();

                // slugify the tag;
                tag = helper.GenerateSlug(tag);
                // tags.Add(tag.innerHTML.Trim());
                // Console.WriteLine(tag);
                tags.Add(tag);
            }

            foreach (var tagString in tags)
            {
                // Check if the tag already exists in the database
                var tagInDb = await dataContext
                    .MusicTags.Find(t => t.Name == tagString)
                    .FirstOrDefaultAsync();

                // Check if the tag exists in the Slugs collection
                var slugInDb = await dataContext
                    .Slugs.Find(t => t.SlugValue == tagString)
                    .FirstOrDefaultAsync();

                if (tagInDb != null || slugInDb != null)
                {
                    Console.WriteLine($"Tag '{tagString}' already exists in the database.");
                    continue; // Skip inserting if the tag already exists
                }
                else
                {
                    // isnt in either db.
                    await dataContext.MusicTags.InsertOneAsync(
                        new MusicTag
                        {
                            Id = ObjectId.GenerateNewId(),
                            Name = tagString,
                            Slug = tagString,
                        }
                    );

                    // create unique id
                    await dataContext.Slugs.InsertOneAsync(
                        new Slug { Id = ObjectId.GenerateNewId(), SlugValue = tagString }
                    );

                }
            }

            Console.WriteLine("Tags scraped and saved successfully.");
        }
    }
}
