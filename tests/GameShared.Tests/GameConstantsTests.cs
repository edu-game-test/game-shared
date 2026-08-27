using GameShared.Constants;
using Xunit;

namespace GameShared.Tests;

public class GameConstantsTests
{
    [Fact]
    public void MaxPartySize_ShouldBeFive() =>
        Assert.Equal(5, GameConstants.MaxPartySize);

    [Fact]
    public void BaseCriticalDamage_ShouldBeLessThanMax() =>
        Assert.True(GameConstants.BaseCriticalDamage < GameConstants.MaxCriticalDamage);
}
