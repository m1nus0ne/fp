using MyStemWrapper;
using TagCloud.Excluders;

namespace TagCloud.TextHandlers;

public class FileTextHandler(Stream stream, IWordFilter filter) : ITextHandler
{
    public Result<Dictionary<string,int>> Handle()
    {
        var wordCounts = new Dictionary<string, int>();
        using var sr = new StreamReader(stream);
        
        while (!sr.EndOfStream)
        {
            var word = sr.ReadLine()?.ToLower();
            if (word == null)
                continue;
            wordCounts.TryAdd(word, 0);
            wordCounts[word]++;
        }

        wordCounts = filter.ExcludeWords(wordCounts);

        return wordCounts;
    }
}