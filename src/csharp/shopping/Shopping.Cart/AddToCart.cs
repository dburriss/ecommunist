namespace Shopping.Cart;

public class AddToCart
{
    private readonly ProductLookup _productLookup;
    private readonly Carts _carts;
    
    public AddToCart(ProductLookup productLookup, Carts carts)
    {
        _productLookup = productLookup;
        _carts = carts;
    }
    
    public AddToCartResult Execute(CartId cartId, ProductId productId, int quantity)
    {
        // Check if known cart, else create new cart
        var cart = _carts.Get(cartId); 
        // Check availability in product catalog cache
        var productRef = _productLookup.LookupProduct(productId);
        // If available, add to cart
        // If not available, return error
        return new AddToCartResult(cartId, productRef!, quantity,true,string.Empty);
    }
}

public interface Carts
{
    Cart Get(CartId cartId);
}

public interface Cart
{
    CartId Id { get; }
    IEnumerable<CartItem> Items { get; }
    int ItemCount { get; }
    int TotalCostCents { get; }
    void Add(ProductRef productRef, int quantity);
    void Remove(ProductRef productRef, int quantity);
    void Clear();
}

public record CartItem(ProductRef ProductRef, int Quantity);

public record AddToCartResult(
    CartId CartId,
    ProductRef ProductRef,
    int Quantity,
    bool Success,
    string ErrorMessage);

public record CartId(Guid Value, uint Version);

public record ProductId(Guid Value);
public record ProductRef(ProductId ProductId, string Title, uint AvailableStock);

public interface ProductLookup
{
    ProductRef? LookupProduct(ProductId productId);
}
