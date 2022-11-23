namespace PTLab2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Mail { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthday { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
    }
    public class Basket
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public float ProductPrice { get; set; }
        public int ProductCount { get; set; }
        public float Amount { get; set; }
    }
}
