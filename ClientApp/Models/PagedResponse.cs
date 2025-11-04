namespace ClientApp.Models
{
    /// <summary>
    /// A generic class to represent paginated responses.
    /// Used to wrap data along with pagination metadata.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}