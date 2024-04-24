namespace Shopping.Cart;

public interface Carts
{
    Task<CartContents> Get(CartId cartId);
    Task<CartContents> Upsert(CartContents newContents, Func<CartId, CartContents, CartContents> updateFactory);
}


public record CartItem(ProductRef ProductRef, uint Quantity);

public class Result
    {
        public static Result Success() => new Result();
        public static Result Fail(params Exception[] exceptions) => new Result(exceptions);
        private List<Exception> errors = new List<Exception>();
        private Result()
        { }

        private Result(Exception[] exceptions)
        {
            errors.AddRange(exceptions);
        }

        public bool IsSuccess { get { return !errors.Any(); } }
        public IEnumerable<Exception> Errors => errors;
    }

    public class Result<T>
    {
        public static Result<T> Success(T value) => new Result<T>(value);
        public static Result<T> Fail(params Exception[] exceptions) => new Result<T>(exceptions);
        private T value;
        private List<Exception> errors = new List<Exception>();
        private Result(T value)
        {
            this.value = value;
        }

        private Result(Exception[] exceptions)
        {
            errors.AddRange(exceptions);
        }
        private bool iHaveBeenChecked = false;
        public T Value
        {
            get
            {
                if (iHaveBeenChecked == false)
                {
                    if (errors.Any())
                    {
                        throw new InvalidOperationException(
                            $"You cannot access {nameof(Value)} without checking {nameof(IsSuccess)}.",
                            new AggregateException($"You cannot access {nameof(Value)} Since you have multiple errors.", errors)
                        );
                    }                        
                    throw new InvalidOperationException($"You cannot access {nameof(Value)} without checking {nameof(IsSuccess)}.");
                }
                return value;
            }
        }
        public bool IsSuccess
        {
            get
            {
                iHaveBeenChecked = true;
                return !errors.Any();
            }
        }
        
        public bool IsFailure => !IsSuccess;

        public IEnumerable<Exception> Errors => errors;
        
        public void RaiseOnFailure()
        {
            if (IsFailure)
            {
                throw new AggregateException("Operation failed", errors);
            }
        }
    }

public record CartId(Guid Value, ulong Version);

public record Sku(string Value);
public record ProductRef(Sku Sku, string Title, uint AvailableStock)
{
    public static ProductRef From(string sku, string title, uint availableStock)
    {
        return new ProductRef(new Sku(sku), title, availableStock);
    }
}

public interface ProductLookup
{
    ProductRef? LookupProduct(Sku sku);
}
