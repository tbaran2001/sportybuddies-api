namespace API.Common.Models;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public Guid Id { get; protected set; }

    public bool Equals(Entity other)
    {
        return Equals((object)other);
    }

    public override bool Equals(object obj)
    {
        return obj is Entity entity && Id.Equals(entity.Id);
    }

    public static bool operator ==(Entity left, Entity right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}