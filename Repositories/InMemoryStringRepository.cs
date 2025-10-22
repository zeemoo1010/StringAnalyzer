using StringAnalyzer.Entity;
using System.Collections.Concurrent;

namespace StringAnalyzer.Repositories
{
    public class InMemoryStringRepository : IStringRepository
    {
        private readonly ConcurrentDictionary<string, AnalyzedString> _store = new();

        public void Add(AnalyzedString analyzedString)
        {
            _store.TryAdd(analyzedString.Value.ToLower(), analyzedString);
        }

        public AnalyzedString? GetByValue(string value)
        {
            _store.TryGetValue(value.ToLower(), out var result);
            return result;
        }

        public IEnumerable<AnalyzedString> GetAll()
        {
            return _store.Values;
        }

        public void Delete(string value)
        {
            _store.TryRemove(value.ToLower(), out _);
        }

        public bool Exists(string value)
        {
            return _store.ContainsKey(value.ToLower());
        }
    }
}
