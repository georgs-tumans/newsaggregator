using System.ComponentModel.DataAnnotations;

namespace helmesproject.Models
{
    public class NewsItem
    {
        [Key]
        public int Id { get; set; }
        public string ItemGuid { get; set; }
        public string? Author { get; set; }
        public DateTime Date { get; set; }
        public string? ReadMoreUrl { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Url { get; set; }
        public int CategoryId { get; set; }
    }
}
