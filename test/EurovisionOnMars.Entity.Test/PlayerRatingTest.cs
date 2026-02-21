using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test;

public class PlayerRatingTest
{
    [Theory]
    [InlineData(12, 20)]
    [InlineData(10, 18)]
    [InlineData(8, 16)]
    [InlineData(6, 14)]
    [InlineData(1, 9)]
    public void SetPoints_Valid(int category1Points, int totalPoints) {
        // arrange
        var rating = GetPlayerRating();
        var category2Points = 5;
        var category3Points = 3;

        // act
        rating.SetPoints(category1Points, category2Points, category3Points);    

        // assert
        Assert.Equal(category1Points, rating.Category1Points);
        Assert.Equal(category2Points, rating.Category2Points);
        Assert.Equal(category3Points, rating.Category3Points);
        Assert.Equal(totalPoints, rating.Prediction.TotalGivenPoints);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryPointsData))]
    public void SetPoints_InvalidCategory1Points(int? category1Points)
    {
        // arrange
        var rating = GetPlayerRating();
        var category2Points = 5;
        var category3Points = 3;

        // act & assert
        Assert.Throws<ArgumentException>(() => rating.SetPoints(category1Points, category2Points, category3Points));
        Assert.Null(rating.Category1Points);
        Assert.Null(rating.Category2Points);
        Assert.Null(rating.Category3Points);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryPointsData))]
    public void SetPoints_InvalidCategory2Points(int? category2Points)
    {
        // arrange
        var rating = GetPlayerRating();
        var category1Points = 5;
        var category3Points = 3;

        // act & assert
        Assert.Throws<ArgumentException>(() => rating.SetPoints(category1Points, category2Points, category3Points));
        Assert.Null(rating.Category1Points);
        Assert.Null(rating.Category2Points);
        Assert.Null(rating.Category3Points);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryPointsData))]
    public void SetPoints_InvalidCategory3Points(int? category3Points)
    {
        // arrange
        var rating = GetPlayerRating();
        var category2Points = 5;
        var category1Points = 3;

        // act & assert
        Assert.Throws<ArgumentException>(() => rating.SetPoints(category1Points, category2Points, category3Points));
        Assert.Null(rating.Category1Points);
        Assert.Null(rating.Category2Points);
        Assert.Null(rating.Category3Points);
    }

    public static IEnumerable<object[]> InvalidCategoryPointsData =>
        new List<object[]>
        {
            new object[] { 13 },
            new object[] { -3 },
            new object[] { 0 },
            new object[] { 11 },
            new object[] { 9 },
            new object[] { null }
        };

    private PlayerRating GetPlayerRating()
    {
        var countries = new List<Country>{ new Country(1, "norge") }.ToImmutableList();
        var player = new Player("testuser", countries);

        return player.PlayerRatings.First();
    }
}
