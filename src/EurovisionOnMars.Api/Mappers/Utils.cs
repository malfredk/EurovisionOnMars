using EurovisionOnMars.Dto;
using System.Collections.Generic;

namespace EurovisionOnMars.Api.Mappers;

public static class Utils
{
    public static List<B>? MapList<A, B>(List<A>? list, Func<A, B> mapper)
    {
        if (list == null)
        {
            return null; 
        }
        return list.Select(item =>  mapper(item)).ToList();
    }
}
