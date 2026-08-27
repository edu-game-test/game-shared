namespace GameShared.Models;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Level { get; set; }
    public long ExperiencePoints { get; set; }
    public PlayerCurrency Currency { get; set; } = new();
    public List<string> HeroIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
}

public class PlayerCurrency
{
    public int Gold { get; set; }
    public int Gems { get; set; }
    public int SummonShards { get; set; }
    public int EnergyPoints { get; set; }
}
