namespace Web.MVC.Api.Dto;

public class Basket
{
    public int Id { get; set; }
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    public string BuyerId { get; set; }

    public decimal Total()
    {
        return Math.Round(Items.Sum(x => x.UnitPrice * x.Quantity), 2);
    }
}
