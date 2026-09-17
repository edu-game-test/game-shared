using MetaFramework.Contract;
using Xunit;

namespace GameShared.Tests.Contract
{
    public class LevelLaunchContextTests
    {
        [Fact]
        public void Defaults_AreEmptyCollections_NotNull()
        {
            var ctx = new LevelLaunchContext { LevelNumber = 1 };
            Assert.NotNull(ctx.ServerParams);
            Assert.NotNull(ctx.ActiveBoosterIds);
            Assert.Empty(ctx.ActiveBoosterIds);
            Assert.Equal(string.Empty, ctx.LevelConfigKey);
            Assert.False(ctx.IsRetry);
        }

        [Fact]
        public void AiHooks_DefaultToNeutral()
        {
            var ctx = new LevelLaunchContext { LevelNumber = 1 };
            Assert.Equal(0f, ctx.DifficultyBias);
            Assert.Null(ctx.BoosterSuggestion);
        }
    }
}
