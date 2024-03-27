using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using Moq;

namespace EurovisionOnMars.Api.Test.Mappers;

public class RatingMapperTest
{
    private readonly Mock<ICountryMapper> _countryMapperMock;
    private readonly Mock<IRatingResultMapper> _ratingResultMapperMock;

    private readonly RatingMapper _mapper;

    public RatingMapperTest()
    {
        _countryMapperMock = new Mock<ICountryMapper>();
        _ratingResultMapperMock = new Mock<IRatingResultMapper>();

        _mapper = new RatingMapper(_countryMapperMock.Object, _ratingResultMapperMock.Object);
    }
    
    [Fact]
    public void ToDto()
    {
        // arrange
        var countryEntity = CreateCountryEntity();
        var countryDto = CreateCountryDto();

        var resultEntity = new RatingResult { Id = 567892 };
        var resultDto = new RatingResultDto { Id = 29 };

        var entity = CreateRatingEntity();
        entity.Country = countryEntity;
        entity.RatingResult = resultEntity;

        _countryMapperMock.Setup(c => c.ToDto(countryEntity))
            .Returns(countryDto);
        _ratingResultMapperMock.Setup(m => m.ToDto(resultEntity))
            .Returns(resultDto);

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Category1Points, dto.Category1Points);
        Assert.Equal(entity.Category2Points, dto.Category2Points);
        Assert.Equal(entity.Category3Points, dto.Category3Points);
        Assert.Equal(entity.PlayerId, dto.PlayerId);
        Assert.Equal(entity.PointsSum, dto.PointsSum);
        Assert.Equal(entity.Ranking, dto.Ranking);
        Assert.Equal(countryDto, dto.Country);
        Assert.Equal(resultDto, dto.RatingResult);

        _countryMapperMock.Verify(c => c.ToDto(countryEntity), Times.Once());
        _ratingResultMapperMock.Verify(m => m.ToDto(resultEntity), Times.Once());
    }

    private Rating CreateRatingEntity()
    {
        var playerEntity = new Player { Username = "jadda" };
        return new Rating
        {
            Id = 34,
            Category1Points = 1,
            Category2Points = null,
            Category3Points = 3,
            PlayerId = 788888,
            Player = playerEntity,
            PointsSum = 9000,
            Ranking = 26,
            CountryId = 5678
        };
    }

    private Country CreateCountryEntity()
    {
        return new Country
        { 
            Id = 233,
            Number = 1,
            Name = "mdewl"
        };
    }

    private CountryDto CreateCountryDto()
    {
        return new CountryDto
        {
            Id = 43,
            Number = 3,
            Name = "md79232ewl"
        };
    }
}
