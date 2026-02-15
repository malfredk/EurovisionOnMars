using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings.Domain;

public class SpecialPointsValidatorTest
{
    private readonly Mock<ILogger<SpecialPointsValidator>> _loggerMock;
    private readonly SpecialPointsValidator _validator;

    public SpecialPointsValidatorTest()
    {
        _loggerMock = new Mock<ILogger<SpecialPointsValidator>>();
        _validator = new SpecialPointsValidator(_loggerMock.Object);
    }

    [Fact]
    public void ValidateSpecialCategoryPoints_NoSpecialPoints_NoException()
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(8, 6, 1);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(12, 12, 12),
            Utils.CreatePlayerRating(10, 10, 10), 
            Utils.CreatePlayerRating(12, 10, 12),
            Utils.CreatePlayerRating(8, 6, 1), 
            editedRating,
        };

        // act
        var exception = Record.Exception(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));

        // assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(10, 12)]
    [InlineData(12, 10)]
    public void ValidateSpecialCategoryPoints_SpecialCategory1Points_NoException(
        int specialCategory1Points,
        int otherSpecialCategory1Points
        )
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(specialCategory1Points, 6, 1);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(1, 12, 12),
            Utils.CreatePlayerRating(1, 10, 10),
            Utils.CreatePlayerRating(otherSpecialCategory1Points, 6, 1),
            editedRating,
        };

        // act
        var exception = Record.Exception(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));

        // assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(10, 12)]
    [InlineData(12, 10)]
    public void ValidateSpecialCategoryPoints_SpecialCategory2Points_NoException(
        int specialCategory2Points,
        int otherSpecialCategory2Points
        )
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(1, specialCategory2Points, 1);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(12, 1, 12),
            Utils.CreatePlayerRating(10, 1, 10),
            Utils.CreatePlayerRating(1, otherSpecialCategory2Points, 1),
            editedRating,
        };

        // act
        var exception = Record.Exception(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));

        // assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(10, 12)]
    [InlineData(12, 10)]
    public void ValidateSpecialCategoryPoints_SpecialCategory3Points_NoException(
        int specialCategory3Points,
        int otherSpecialCategory3Points
        )
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(1, 6, specialCategory3Points);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(12, 12, 1),
            Utils.CreatePlayerRating(10, 10, 1),
            Utils.CreatePlayerRating(1, 6, otherSpecialCategory3Points),
            editedRating,
        };

        // act
        var exception = Record.Exception(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));

        // assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void ValidateSpecialCategoryPoints_SpecialCategory1Points_ThrowException(
        int specialCategory1Points
        )
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(specialCategory1Points, 2, 3);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(specialCategory1Points, 6, 1),
            editedRating,
        };

        // act and assert
        Assert.Throws<ArgumentException>(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void ValidateSpecialCategoryPoints_SpecialCategory2Points_ThrowException(
        int specialCategory2Points
        )
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(6, specialCategory2Points, 1);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(2, specialCategory2Points, 3),
            editedRating,
        };

        // act and assert
        Assert.Throws<ArgumentException>(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(12)]
    public void ValidateSpecialCategoryPoints_SpecialCategory3Points_ThrowException(
        int specialCategory3Points
        )
    {
        // arrange
        var editedRating = Utils.CreatePlayerRating(2, 3, specialCategory3Points);
        var ratings = new List<PlayerRating>
        {
            Utils.CreatePlayerRating(2, 3, specialCategory3Points),
            editedRating,
        };

        // act and assert
        Assert.Throws<ArgumentException>(() =>
            _validator.ValidateSpecialCategoryPoints(editedRating, ratings));
    }
}
