namespace Naninovel.Csv;

/// <summary>
/// Serializes strings as text in comma-separated values (CSV) format.
/// </summary>
public class Writer (TextWriter writer, Writer.Options? options = null)
{
    /// <summary>
    /// Configures writing behaviour.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Whether to wrap all fields in quotes; disabled by default;
        /// </summary>
        public bool QuoteAll { get; set; }
        /// <summary>
        /// Whether to trim leading and trailing whitespace in fields; disabled by default;
        /// </summary>
        public bool Trim { get; set; }
    }

    private const char delimiter = ',';
    private const string quote = "\"";
    private const string doubleQuote = "\"\"";
    private static readonly char[] quoted = ['\r', '\n', delimiter];

    private readonly Options options = options ?? new();
    private int recordFieldCount;

    public void WriteField (string field)
    {
        var shouldQuote = options.QuoteAll;

        if (field.Length > 0 && options.Trim)
            field = field.Trim();

        if (!shouldQuote && field.Length > 0)
            if (field.Contains(quote) || field[0] == ' ' || field[^1] == ' ' || field.IndexOfAny(quoted) > -1)
                shouldQuote = true;
        if (shouldQuote && field.Length > 0)
            field = field.Replace(quote, doubleQuote);
        if (shouldQuote) field = quote + field + quote;

        if (recordFieldCount > 0) writer.Write(delimiter);
        if (field.Length > 0) writer.Write(field);

        recordFieldCount++;
    }

    public void NextRecord ()
    {
        writer.Write('\n');
        recordFieldCount = 0;
    }
}
