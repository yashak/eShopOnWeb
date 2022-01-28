namespace Web.MVC.BL.Entity;

public class Basket
{
    public int Id { get; set; }
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    public string BuyerId { get; set; }
}
