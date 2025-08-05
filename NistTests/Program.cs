namespace NistTests
{
    internal class Program
    {
        private static readonly HashSet<string> _knownVectors = new()
    {
        "ShortMsg",
        "LongMsg",
    };

        private static readonly HashSet<string> _variants = new()
    {
        "SHA256",
        "SHA384",
        "SHA512",
    };

        static void Main(string[] args)
        {
            foreach (string variant in _variants)
            {
                foreach (string vectorSet in _knownVectors)
                {
                    NistKatPlayer.RunSha3Kat(Path.Combine("Vectors", $"{variant}{vectorSet}.rsp"));
                }
            }
        }
    }
}
