using StringAnalyzer.Entity;
using System.Security.Cryptography;
using System.Text;

namespace StringAnalyzer.Service
{
    public class StringAnalysisService
    {
        public AnalyzedString AnalyzeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Value cannot be empty or whitespace.");

            var lower = input.ToLower();
            var properties = new StringProperties
            {
                Length = input.Length,
                IsPalindrome = lower.SequenceEqual(lower.Reverse()),
                UniqueCharacters = lower.Distinct().Count(),
                WordCount = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                Sha256Hash = ComputeSha256Hash(input),
                CharacterFrequencyMap = GetCharacterFrequency(input)
            };

            return new AnalyzedString
            {
                Id = properties.Sha256Hash,
                Value = input,
                Properties = properties,
                CreatedAt = DateTime.UtcNow
            };
        }

        private string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private Dictionary<char, int> GetCharacterFrequency(string input)
        {
            var frequency = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (frequency.ContainsKey(c))
                    frequency[c]++;
                else
                    frequency[c] = 1;
            }
            return frequency;
        }
    }
}
