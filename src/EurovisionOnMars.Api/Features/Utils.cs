namespace EurovisionOnMars.Api.Features.Common;

public class Utils
{
    public static List<B>? MapList<A, B>(List<A>? entityList, Func<A, B> toDto)
    {
        if (entityList == null)
        {
            return null; 
        }
        return entityList.Select(entity => toDto(entity)).ToList();
    }
}