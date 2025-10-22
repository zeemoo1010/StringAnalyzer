using Microsoft.AspNetCore.Mvc;
using StringAnalyzer.Entity;
using StringAnalyzer.Repositories;
using StringAnalyzer.Service;

namespace StringAnalyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StringsController : ControllerBase
    {
        private readonly StringAnalysisService _analysisService;
        private readonly IStringRepository _repository;

        public StringsController(StringAnalysisService analysisService, IStringRepository repository)
        {
            _analysisService = analysisService;
            _repository = repository;
        }

        [HttpPost]
        public IActionResult AnalyzeString([FromBody] StringRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Value))
                return BadRequest(new { message = "Missing or invalid 'value' field" });

            if (_repository.Exists(request.Value))
                return Conflict(new { message = "String already exists in the system" });

            var analyzed = _analysisService.AnalyzeString(request.Value);
            _repository.Add(analyzed);

            return CreatedAtAction(nameof(GetString), new { string_value = request.Value }, analyzed);
        }

        [HttpGet("{string_value}")]
        public IActionResult GetString(string string_value)
        {
            var result = _repository.GetByValue(string_value);
            if (result == null)
                return NotFound(new { message = "String not found in the system" });

            return Ok(result);
        }

        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var data = _repository.GetAll();
            return Ok(new
            {
                data,
                count = data.Count()
            });
        }

        [HttpDelete("{string_value}")]
        public IActionResult Delete(string string_value)
        {
            if (!_repository.Exists(string_value))
                return NotFound(new { message = "String not found in the system" });

            _repository.Delete(string_value);
            return NoContent();
        }

        [HttpGet("filter-by-natural-language")]
        public IActionResult FilterByNaturalLanguage([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query cannot be empty" });

            var strings = _repository.GetAll();

            var parsedFilters = new Dictionary<string, object>();

            var normalized = query.Trim().ToLower();

            if (normalized.Contains("palindromic"))
            {
                strings = strings.Where(s => s.IsPalindrome);
                parsedFilters["is_palindrome"] = true;
            }

            if (normalized.Contains("single word"))
            {
                strings = strings.Where(s => s.WordCount == 1);
                parsedFilters["word_count"] = 1;
            }

            if (normalized.Contains("longer than"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(normalized, @"longer than (\d+)");
                if (match.Success)
                {
                    int minLen = int.Parse(match.Groups[1].Value);
                    strings = strings.Where(s => s.Length > minLen);
                    parsedFilters["min_length"] = minLen + 1;
                }
            }

            if (normalized.Contains("containing the letter"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(normalized, @"containing the letter ([a-z])");
                if (match.Success)
                {
                    var letter = match.Groups[1].Value;
                    strings = strings.Where(s => s.Value.Contains(letter, StringComparison.OrdinalIgnoreCase));
                    parsedFilters["contains_character"] = letter;
                }
            }

            if (normalized.Contains("containing the first vowel"))
            {
                strings = strings.Where(s => s.Value.Contains('a', StringComparison.OrdinalIgnoreCase));
                parsedFilters["contains_character"] = "a";
            }

            if (!parsedFilters.Any())
                return BadRequest(new { error = "Unable to parse natural language query" });

            var result = new
            {
                data = strings.Select(s => new
                {
                    id = s.Id,
                    value = s.Value,
                    properties = new
                    {
                        length = s.Properties.Length,
                        is_palindrome = s.Properties.IsPalindrome,
                        unique_characters = s.Properties.UniqueCharacters,
                        word_count = s.Properties.WordCount,
                        sha256_hash = s.Properties.Sha256Hash,
                        character_frequency_map = s.Properties.CharacterFrequencyMap
                    },
                    created_at = s.CreatedAt
                }),
                count = strings.Count(),
                interpreted_query = new
                {
                    original = query,
                    parsed_filters = parsedFilters
                }
            };

            return Ok(result);
        }


    }
}
