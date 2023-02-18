namespace advs_backend.JSON
{
    public class AdvJSON
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;

        public string? Location { get; set; }

        public string? Discription { get; set; }

        public decimal? Price { get; set; }
    }

    public class NewAdvJSON
    {
        public string Name { get; set; } = null!;

        public string? Location { get; set; }

        public string? Discription { get; set; }

        public decimal? Price { get; set; }

        public int UserId { get; set; }
    }

    public class UserJSON
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class Result
    {
        public string Message { get; set; }
    }
}
