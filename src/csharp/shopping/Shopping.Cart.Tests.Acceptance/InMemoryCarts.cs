namespace Shopping.Cart.Tests.Tooling;

public class InMemoryCarts : Carts
{
    private Dictionary<Guid, CartContents> _state;
    
    public InMemoryCarts(params CartContents[] actorContents)
    {
        _state = actorContents.ToDictionary(contents => contents.Id);
    }
    
    public Task<CartContents> Get(CartId cartId)
    {
        _state.TryGetValue(cartId.Value, out var cartContents);
        return Task.FromResult(cartContents);
    }
    
    public Task<CartContents> Upsert(CartContents cartContents, Func<CartId, CartContents, CartContents> updateFactory)
    {
        if (!_state.ContainsKey(cartContents.CartId.Value))
        {
            _state[cartContents.Id] = cartContents;
            return Task.FromResult(cartContents);
        }
        else
        {
            var existingContents = _state[cartContents.CartId.Value];
            var newContents = updateFactory(cartContents.CartId, existingContents);
            _state[cartContents.Id] = newContents;
            return Task.FromResult(newContents);
        }
    }
}
