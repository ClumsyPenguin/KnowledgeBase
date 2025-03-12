namespace AOP.Interception.Caching;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class CacheAttribute : Attribute
{
    public CacheAttribute(int itemLifeSpanInMinutes)
    {
        ItemLifeSpan = TimeSpan.FromMinutes(itemLifeSpanInMinutes);
    }

    public TimeSpan ItemLifeSpan { get; }
}