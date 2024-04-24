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
        try
        {
            var productRef = _productLookup.LookupProduct(sku);
            if (IsProductNotFoundInLookup(productRef)) 
                return ProductNotFoundResult();
            
            if (IsNotEnoughStock(quantity, productRef))
                return NotEnoughStockResult();
            
            var cart = await AddItemToCart(cartId, productRef!, quantity, currentCartVersion);
            return Result<CartContents>.Success(cart);
        }
        catch (Exception e)
        {
            return Result<CartContents>.Fail(e);
        }
    }
    
    private async Task<CartContents> AddItemToCart(
        CartId cartId,
        ProductRef productRef,
        uint quantity,
        ulong currentCartVersion)
    {
        return await _carts.Upsert(
            CartContents.AddItem(CartContents.NewWith(cartId), currentCartVersion, productRef, quantity),
            (_, existingContents) =>
            {
                return CartContents.AddItem(existingContents, currentCartVersion, productRef, quantity);
            }
        );
    }
    
    private static Result<CartContents> NotEnoughStockResult()
    {
        return Result<CartContents>.Fail(new InvalidOperationException("Not enough stock"));
    }
    
    private static bool IsNotEnoughStock(uint quantity, ProductRef? productRef)
    {
        return productRef.AvailableStock < quantity;
    }
    
    private static Result<CartContents> ProductNotFoundResult()
    {
        return Result<CartContents>.Fail(new NullReferenceException("Product not found"));
    }
    
    private static bool IsProductNotFoundInLookup(ProductRef? productRef)
    {
        return productRef == null;
    }
}
