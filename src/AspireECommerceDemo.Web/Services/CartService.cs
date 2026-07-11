namespace AspireECommerceDemo.Web.Services;

public class CartService
{
    private readonly List<CartItem> _items = new();

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(i => i.Quantity * i.UnitPrice);
    public int Count => _items.Sum(i => i.Quantity);
    public event Action? OnChange;

    public void AddItem(Guid productId, string name, decimal price, string imageUrl)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
            existing.Quantity++;
        else
            _items.Add(new CartItem { ProductId = productId, ProductName = name, UnitPrice = price, ImageUrl = imageUrl, Quantity = 1 });
        OnChange?.Invoke();
    }

    public void RemoveItem(Guid productId)
    {
        _items.RemoveAll(i => i.ProductId == productId);
        OnChange?.Invoke();
    }

    public void UpdateQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0) _items.Remove(item);
            else item.Quantity = quantity;
        }
        OnChange?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        OnChange?.Invoke();
    }
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
