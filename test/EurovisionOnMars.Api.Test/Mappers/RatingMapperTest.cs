using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using Moq;

namespace EurovisionOnMars.Api.Test.Mappers;

public class RatingMapperTest
{
    private readonly Mock<ICountryMapper> _countryMapperMock;
    private readonly Mock<IRatingGameResultMapper> _ratingResultMapperMock;
    private readonly Mock<IPredictionMapper> _predictionMapperMock;

    private readonly PlayerRatingMapper _mapper;

    public RatingMapperTest()
    {
        _countryMapperMock = new Mock<ICountryMapper>();
        _ratingResultMapperMock = new Mock<IRatingGameResultMapper>();
        _predictionMapperMock = new Mock<IPredictionMapper>();

        _mapper = new PlayerRatingMapper(_countryMapperMock.Object, _ratingResultMapperMock.Object, _predictionMapperMock.Object);
    }
    
    [Fact]
    public void ToDto()
    {
        // arrange
        var countryEntity = CreateCountryEntity();
        var countryDto = CreateCountryDto();

        var resultEntity = new RatingGameResult { Id = 567892 };
        var resultDto = new RatingGameResultDto { Id = 29 };

        var predictionEntity = new Prediction { Id = 100689 };
        var predictionDto = new PredictionDto { Id = 689 };

        var entity = CreateRatingEntity();
        entity.Country = countryEntity;
        entity.RatingGameResult = resultEntity;
        entity.Prediction = predictionEntity;

        _countryMapperMock.Setup(c => c.ToDto(countryEntity))
            .Returns(countryDto);
        _ratingResultMapperMock.Setup(m => m.ToDto(resultEntity))
            .Returns(resultDto);
        _predictionMapperMock.Setup(m => m.ToDto(predictionEntity))
            .Returns(predictionDto);

        // act
        var dto = _mapper.ToDto(entity);

        // assert
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Category1Points, dto.Category1Points);
        Assert.Equal(entity.Category2Points, dto.Category2Points);
        Assert.Equal(entity.Category3Points, dto.Category3Points);
        Assert.Equal(predictionDto, dto.Prediction);
        Assert.Equal(countryDto, dto.Country);
        Assert.Equal(resultDto, dto.RatingGameResult);

        _countryMapperMock.Verify(c => c.ToDto(countryEntity), Times.Once());
        _ratingResultMapperMock.Verify(m => m.ToDto(resultEntity), Times.Once());
    }

    private PlayerRating CreateRatingEntity()
    {
        var playerEntity = new Player { Username = "jadda" };
        return new PlayerRating
        {
            Id = 34,
            Category1Points = 1,
            Category2Points = null,
            Category3Points = 3,
            PlayerId = 788888,
            Player = playerEntity,
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
