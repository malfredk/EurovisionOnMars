using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public class Utils
{
    public static List<B>? MapList<A, B>(List<A>? entityList, Func<A, B> toDto)
    {
        if (entityList == null)
        {
            return null; 
        }
        return entityList.Select(entity =>  toDto(entity)).ToList();
    }

    public List<A>? UpdateList<A, B>(List<A>? entityList, List<B>? dtoList, Func<A, B, A> updateEntity)
        where A : IdBase
        where B : IdBaseDto
    {
        if (entityList == null || dtoList == null)
        {
            return entityList;
        }

        return entityList
            .Select(entity => updateEntity
            (
                entity,
                dtoList.FirstOrDefault(dto => dto.Id == entity.Id)
            )
            ).ToList();
    }
}
