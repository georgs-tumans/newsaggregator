namespace helmesproject.Models
{
    public class NewsResponse
    {
        public string Category { get; set; }
        public List<NewsResponseItem> Data { get; set; }
        public bool Success;
    }
}
