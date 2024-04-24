using System.Collections.Immutable;

namespace Shopping.Cart;

public record CartContents(Guid Id, uint Version, ImmutableDictionary<string, Tuple<string, uint>> Items)
{
    public static CartContents New => new CartContents(Guid.NewGuid(), 0, ImmutableDictionary<string, Tuple<string, uint>>.Empty);
    public static CartContents NewWith(CartId cartId) => new(cartId.Value, 0, ImmutableDictionary<string, Tuple<string, uint>>.Empty);
    public static CartContents AddItem(CartContents contents, ulong previousVersion, ProductRef productRef, uint quantity)
    {
        // version check
        if (contents.Version != previousVersion)
        {
            throw new InvalidOperationException("Version mismatch");
        }
        
        // check if exists
        var sku = productRef.Sku.Value;
        if (contents.Items.ContainsKey(sku))
        {
            // increment quantity
            var item = contents.Items[sku];
            return new CartContents(contents.Id, contents.Version + 1, contents.Items.SetItem(sku, new Tuple<string, uint>(productRef.Title, item.Item2 + quantity)));
        }
        else
        {
            // add new item
            return new CartContents(contents.Id, contents.Version + 1, contents.Items.Add(sku, new Tuple<string, uint>(productRef.Title, quantity)));
        }
    }
    
    // Cart implementation
    
    public CartId CartId => new CartId(this.Id, this.Version);
    
}
