namespace SharedApp.Dto
{
    /// <summary>
    /// Generic paged response DTO for returning paginated data along with metadata
    /// </summary>

   public record PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
}