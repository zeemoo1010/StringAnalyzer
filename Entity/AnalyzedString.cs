namespace StringAnalyzer.Entity
{
    public class AnalyzedString
    {
        public string? Id { get; set; }
        public string? Value { get; set; }
        public StringProperties? Properties { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPalindrome => Properties?.IsPalindrome ?? false;

        public int WordCount => Properties?.WordCount ?? 0;

        public int Length => Properties?.Length ?? 0


        public int UniqueCharacters => Properties?.UniqueCharacters ?? 0;

        public string Sha256Hash => Properties?.Sha256Hash ?? string.Empty;

        public Dictionary<char, int> CharacterFrequencyMap =>
            Properties?.CharacterFrequencyMap ?? new Dictionary<char, int>();
    }
}
