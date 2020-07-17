
namespace SampleASPNETCoreApplication.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public override string ToString()
        {
            return "Product Found: " + " Name: " + Name + " Price: " + Price;
        }
    }
}
