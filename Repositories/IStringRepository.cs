using StringAnalyzer.Entity;

namespace StringAnalyzer.Repositories
{
    public interface IStringRepository
    {
        void Add(AnalyzedString analyzedString);
        AnalyzedString? GetByValue(string value);
        IEnumerable<AnalyzedString> GetAll();
        void Delete(string value);
        bool Exists(string value);
    }
}
