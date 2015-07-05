namespace TextAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    
    [Serializable]
    public class WordCount
    {
        public string Word { get; set; }
        public int Count { get; set; }
    }

    public class Analyzer
    {
        public static string[] Words(string text)
        {
            var lowerCase = text.ToLowerInvariant();
            var matchWords = new Regex(@"\w+");
            var words =
                matchWords
                    .Matches(lowerCase)
                    .Cast<Match>()
                    .Select(w => w.Value)
                    .Distinct();
            return words.ToArray();
        }

        public static WordCount[] WordsCount(string text)
        {
            var lowerCase = text.ToLowerInvariant();
            var matchWords = new Regex(@"\w+");
            var words =
                matchWords
                    .Matches(lowerCase)
                    .Cast<Match>()
                    .Select(w => w.Value)
                    .GroupBy(
                        word => word,
                        word => word,
                        (key, group) => new WordCount() { Word = key, Count = group.Count() });
            return words.ToArray();
        }
    }
}
