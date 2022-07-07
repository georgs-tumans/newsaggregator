namespace helmesproject
{
    using System.Globalization;
    using System.Text.Json;

    public class AppBackgroundService : IHostedService
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly LoggerService _logger;

        public AppBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration, LoggerService logger)
        {
            _scopeFactory = scopeFactory;
            _config = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.Log("Service iteration start", Helpers.LogLevel.Debug);
                    await GetDataAsync();
                    _logger.Log("Service iteration end", Helpers.LogLevel.Debug);
                    await Task.Delay(TimeSpan.FromSeconds(Convert.ToDouble(_config.GetSection("RefreshInterval").Value)));
                }
            });

            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private async Task GetDataAsync()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };

                    List<NewsItem> resultList = new List<NewsItem>();
                    NewsResponse responseResult;

                    using (var httpClient = new HttpClient())
                    {
                        var url = _config.GetSection("ApiUrl").Value;
                        _logger.Log("Calling API", Helpers.LogLevel.Info, null, url);

                        using (var response = await httpClient.GetAsync(url))
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                _logger.Log("API call successful, reading response", Helpers.LogLevel.Debug);
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                responseResult = JsonSerializer.Deserialize<NewsResponse>(apiResponse, options);
                            }
                            else throw new Exception("API request failed!");
                            
                        }
                    }

                    if (responseResult is not null && responseResult.Data.Any())
                    {
                        _logger.Log($"API response successfully deserialized; {responseResult.Data.Count()} items received", Helpers.LogLevel.Info);
                        //Ja DB vēl nav piefiksēta atnākušo ziņu kategorija, tad sākumā ievietojam to, jo DB visiem ziņu ierakstiem jābūt atslēgai uz kategoriju
                        int categoryId;

                        if (dbContext.Categories.Where(x => x.Name.ToLower() == responseResult.Category.ToLower()).Any())
                        {
                            categoryId = dbContext.Categories.Where(x => x.Name.ToLower() == responseResult.Category.ToLower()).First().Id;
                            _logger.Log($"News category found; categoryID: {categoryId}", Helpers.LogLevel.Debug);
                        }
                        else
                        {
                            var newCategory = new Category { Name = responseResult.Category.Trim() };
                            dbContext.Categories.Add(newCategory);
                            await dbContext.SaveChangesAsync();
                            categoryId = newCategory.Id;
                            _logger.Log($"New category; categoryID: {categoryId}", Helpers.LogLevel.Debug);
                        }

                        //Sataisam sarakstu ar DB draudzīgiem objektiem un tos saglabājam
                        
                        foreach (var item in responseResult.Data)
                        {
                            //Lai nerakstītu dublikātus, bet laikam guidi tām pašām ziņām sakrīt
                            if (!dbContext.NewsItems.Where(x => x.ItemGuid == item.Id).Any())
                            {
                                resultList.Add(new NewsItem()
                                {
                                    ItemGuid = item.Id,
                                    Author = item.Author,
                                    ReadMoreUrl = item.ReadMoreUrl,
                                    Title = item.Title,
                                    Content = item.Content,
                                    Url = item.Url,
                                    Date = ConvertTime(item.Date, item.Time),//Jāsataisa normāls, saprotams datums
                                    CategoryId = categoryId
                                });
                            }
                        }

                        if (resultList is not null)
                        {
                            _logger.Log($"Adding {resultList.Count()} new items to database..", Helpers.LogLevel.Debug);
                            foreach (var ri in resultList)
                            {
                                dbContext.NewsItems.Add(ri);
                                await dbContext.SaveChangesAsync();
                            }
                            _logger.Log($"{resultList.Count()} items added to database", Helpers.LogLevel.Info);
                        }

                        else
                            _logger.Log("No items were added to database", Helpers.LogLevel.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString(), Helpers.LogLevel.Error, ex.StackTrace);
            }
           

        }

        private DateTime ConvertTime(string date, string time)
        {
            string pattern = "dd MMM yyyy hh:mm";
            string datetime = $"{date.Substring(0, date.IndexOf(","))} {time.Substring(0, time.IndexOf(" "))}";

            DateTime parsedDate;
            DateTime.TryParseExact(datetime, pattern, null, DateTimeStyles.None, out parsedDate);

            return parsedDate;
          
        }

    }
}
