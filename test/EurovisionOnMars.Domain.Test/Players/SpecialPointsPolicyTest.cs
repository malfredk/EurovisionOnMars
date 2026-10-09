using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Test.TestData.Players;

namespace EurovisionOnMars.Domain.Test.Players;

public class SpecialPointsPolicyTest
{
    private readonly Points RegularPoints = new(1);
    private readonly Points SpecialPoints = new(10);

    [Fact]
    public void Validate_NoOtherRatingAndSpecialPoints_DoesNotThrow()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayer();
        var ratingToUpdate = player.PlayerRatings[0];

        // act
        var exception = Record.Exception(() =>
            SpecialPointsPolicy.Validate(
                ratingToUpdate,
                player.PlayerRatings,
                SpecialPoints,
                SpecialPoints,
                SpecialPoints)
            );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_InitialRatingAndSpecialPoints_DoesNotThrow()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];

        // act
        var exception = Record.Exception(() =>
            SpecialPointsPolicy.Validate(
                ratingToUpdate,
                player.PlayerRatings,
                SpecialPoints,
                SpecialPoints,
                SpecialPoints)
            );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_SameNonSpecialPoints_DoesNotThrow()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];
        otherRating.SetPoints(RegularPoints, RegularPoints, RegularPoints);

        // act
        var exception = Record.Exception(() =>
            SpecialPointsPolicy.Validate(
                ratingToUpdate,
                player.PlayerRatings,
                RegularPoints,
                RegularPoints,
                RegularPoints)
            );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_SameSpecialPointsInOtherCategory_DoesNotThrow()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];
        otherRating.SetPoints(SpecialPoints, SpecialPoints, RegularPoints);

        // act
        var exception = Record.Exception(() =>
            SpecialPointsPolicy.Validate(
                ratingToUpdate,
                player.PlayerRatings,
                RegularPoints,
                RegularPoints,
                SpecialPoints)
            );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_DifferentSpecialPointsInSameCategory_DoesNotThrow()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];
        otherRating.SetPoints(SpecialPoints, SpecialPoints, SpecialPoints);
        
        var otherSpecialPoints = new Points(12);

        // act
        var exception = Record.Exception(() =>
            SpecialPointsPolicy.Validate(
                ratingToUpdate,
                player.PlayerRatings,
                otherSpecialPoints,
                otherSpecialPoints,
                otherSpecialPoints)
            );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_SpecialPointsAlreadyUsedInCategory1_ThrowsArgumentException()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];

        otherRating.SetPoints(
            SpecialPoints,
            RegularPoints,
            RegularPoints);

        // act
        var action = () => SpecialPointsPolicy.Validate(
            ratingToUpdate,
            player.PlayerRatings,
            SpecialPoints,
            RegularPoints,
            RegularPoints);

        // assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Validate_SpecialPointsAlreadyUsedInCategory2_ThrowsArgumentException()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];

        otherRating.SetPoints(
            RegularPoints,
            SpecialPoints,
            RegularPoints);

        // act
        var action = () => SpecialPointsPolicy.Validate(
            ratingToUpdate,
            player.PlayerRatings,
            RegularPoints,
            SpecialPoints,
            RegularPoints);

        // assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Validate_SpecialPointsAlreadyUsedInCategory3_ThrowsArgumentException()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];

        otherRating.SetPoints(
            RegularPoints,
            RegularPoints,
            SpecialPoints);

        // act
        var action = () => SpecialPointsPolicy.Validate(
            ratingToUpdate,
            player.PlayerRatings,
            RegularPoints,
            RegularPoints,
            SpecialPoints);

        // assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Validate_SpecialPointsUsedByRatingBeingUpdated_DoesNotThrow()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();
        var ratingToUpdate = player.PlayerRatings[0];
        var otherRating = player.PlayerRatings[1];
        otherRating.SetPoints(RegularPoints, RegularPoints, RegularPoints);
        ratingToUpdate.SetPoints(
            SpecialPoints,
            SpecialPoints,
            SpecialPoints);

        // act
        var exception = Record.Exception(() =>
            SpecialPointsPolicy.Validate(
                ratingToUpdate,
                player.PlayerRatings,
                SpecialPoints,
                SpecialPoints,
                SpecialPoints));

        // assert
        Assert.Null(exception);
    }
}