namespace MovieDatabaseAPI.Database
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Director { get; set; }
        public string? DistributedBy { get; set; }
        public uint Budget { get; set; }
        public string? CoverUrl { get; set; }
    }
}
