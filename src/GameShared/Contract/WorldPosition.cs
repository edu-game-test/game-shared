namespace MetaFramework.Contract
{
    /// <summary>Unity-free stand-in for Vector2 (world position of a collected currency, used for fly-to-HUD animations).</summary>
    public readonly struct WorldPosition
    {
        public WorldPosition(float x, float y) { X = x; Y = y; }
        public float X { get; }
        public float Y { get; }
    }
}
