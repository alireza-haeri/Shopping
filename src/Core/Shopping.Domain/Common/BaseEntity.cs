namespace Shopping.Domain.Common;

public interface IEntity
{
    DateTime CreatedDate { get; set; }
    DateTime ModifyDate { get; set; }
}
public abstract class BaseEntity<TKey> : IEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime ModifyDate { get; set; }

    public TKey Id { get; protected set; }
    
    public override bool Equals(object? entity)
    {
        if (entity is null)
            return false;
        
        if(entity is not BaseEntity<TKey> baseEntity)
            return false;
        
        if(ReferenceEquals(this,entity))
            return true;

        return baseEntity.Id!.Equals(Id);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        return !(left == right);
    }
}