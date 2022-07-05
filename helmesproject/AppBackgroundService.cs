namespace helmesproject
{
    using System.Globalization;
    using System.Text.Json;

    public class AppBackgroundService : IHostedService
    {

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;

        public AppBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _config = configuration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("testing service");
                    GetDataAsync();
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
                    using (var response = await httpClient.GetAsync(_config.GetSection("ApiUrl").Value))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        responseResult = JsonSerializer.Deserialize<NewsResponse>(apiResponse, options);
                    }
                }

                if (responseResult is not null && responseResult.Data.Any())
                {
                    //Ja DB vēl nav piefiksēta atnākušo ziņu kategorija, tad sākumā ievietojam to, jo DB visiem ziņu ierakstiem jābūt atslēgai uz kategoriju
                    int categoryId;

                    if (dbContext.Categories.Where(x => x.Name.ToLower() == responseResult.Category.ToLower()).Any()) {
                        categoryId = dbContext.Categories.Where(x => x.Name.ToLower() == responseResult.Category.ToLower()).First().Id;
                    }
                    else
                    {
                        var newCategory = new Category { Name = responseResult.Category.Trim() };
                        dbContext.Categories.Add(newCategory);
                        await dbContext.SaveChangesAsync();
                        categoryId = newCategory.Id;
                    }
                    
                    //Sataisam sarakstu ar DB draudzīgiem objektiem un tos saglabājam
                    foreach (var item in responseResult.Data)
                    {
                        //Lai nerakstītu dublikātus
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
                                Date = ConvertTime(item.Date, item.Time),//DateTime.Now,
                                CategoryId = categoryId
                            });
                        }    
                    }


                    if (resultList is not null)
                    {
                        foreach (var ri in resultList)
                        {
                            dbContext.NewsItems.Add(ri);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
                
                
            }

        }

        private DateTime ConvertTime(string date, string time)
        {
            try
            {
                string pattern = "dd MMM yyyy hh:mm";
                string datetime = $"{date.Substring(0, date.IndexOf(","))} {time.Substring(0, time.IndexOf(" "))}";

                DateTime parsedDate;
                DateTime.TryParseExact(datetime, pattern, null, DateTimeStyles.None, out parsedDate);

                return parsedDate;
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

    }
}
