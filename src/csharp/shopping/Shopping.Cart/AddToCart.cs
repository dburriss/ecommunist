namespace Shopping.Cart;

public class AddToCart
{
    private readonly Carts _carts;
    private readonly ProductLookup _productLookup;
    
    public AddToCart(ProductLookup productLookup, Carts carts)
    {
        _productLookup = productLookup;
        _carts = carts;
    }
    
    public async Task<Result<CartContents>> Execute(
        CartId cartId,
        Sku sku,
        uint quantity,
        ulong currentCartVersion = 0)
    {
        // Check availability in product catalog cache
        var productRef = _productLookup.LookupProduct(sku);
        if (productRef == null) return Result<CartContents>.Fail(new NullReferenceException("Product not found"));
        // Check if enough stock 
        if (productRef.AvailableStock < quantity)
            return Result<CartContents>.Fail(new InvalidOperationException("Not enough stock"));
        // Get cart contents
        try
        {
            var cart = await _carts.Upsert(
                CartContents.AddItem(CartContents.New, currentCartVersion, productRef, quantity),
                (_, existingContents) =>
                {
                    return CartContents.AddItem(existingContents, currentCartVersion, productRef, quantity);
                }
            );
            return Result<CartContents>.Success(cart);
        }
        catch (Exception e)
        {
            return Result<CartContents>.Fail(e);
        }
    }
}
