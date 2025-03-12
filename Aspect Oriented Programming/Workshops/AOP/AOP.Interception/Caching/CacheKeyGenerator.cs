using System.Reflection;
using System.Text;

namespace AOP.Interception.Caching;


public class CacheKeyGenerator
{
    public static string Generate(MethodInfo method, object?[] arguments)
    {
        var sb = new StringBuilder();
        sb.Append(method.DeclaringType?.FullName)
            .Append('.')
            .Append(method.Name);

        foreach (var arg in arguments)
        {
            sb.Append('_');
            sb.Append(arg is null ? "null" : arg.ToString());
        }

        return sb.ToString();
    }
}