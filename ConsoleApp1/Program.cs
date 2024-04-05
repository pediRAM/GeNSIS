namespace ConsoleApp1
{
    using LiteDB;

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new LiteDatabase(@"MeineDatenbank.db"))
            {
                var collection = db.GetCollection<Product>("products");
                var product = new Product { Name = "eTrash2000", Price = 999.99m };
                var xxx = collection.Insert(product);

                // Die ID des eingefügten Produkts abrufen
                var id = product.Id; // Angenommen, Id ist vom Typ int oder ein anderer Typ

                var alles = collection.FindAll();
                
            }

        }
    }
}
