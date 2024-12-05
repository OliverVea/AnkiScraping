using System.Text;

namespace AnkiScraping.Anki;

internal class AnkiCardCollection : IAnkiCardCollectionWithoutFields, IAnkiCardCollectionWithFields
{
    private const char HeaderStart = '#';
    private const char HeaderDelimiter = ',';
    private const char HeaderNameEnd = ':';
    
    private const char FieldContainerMark = '"';
    
    private readonly AnkiDeckFields _fields = new();
    private readonly AnkiDeckCards _cards = new();
    private readonly AnkiDeckTags _tags = new();

    // IAnkiDeckBase
    public IReadOnlyList<string> Tags => _tags.Tags;
    public int TagCount => _tags.Tags.Count;

    // IAnkiDeckWithFields
    public int FieldCount => _fields.Fields.Count;
    public AnkiCardTextFileSeparator DefaultSeparator { get; private set; } = AnkiCardTextFileSeparator.Comma;

    public IReadOnlyList<string> Fields => _fields.Fields;
    
    public IAnkiCardCollectionWithoutFields AddTags(params string[] tags)
    {
        _tags.AddTags(tags);
        return this;
    }

    public IAnkiCardCollectionWithoutFields WithTags(params string[] tags)
    {
        _tags.WithTags(tags);
        return this;
    }

    public IAnkiCardCollectionWithFields WithFields(params string[] fields)
    {
        _fields.WithFields(fields);
        return this;
    }


    public IAnkiCardCollectionWithFields WithDefaultSeparator(AnkiCardTextFileSeparator separator)
    {
        DefaultSeparator = separator;
        return this;
    }

    public IAnkiCardCollectionWithFields AddCard(params IReadOnlyCollection<string> fieldValues)
    {
        if (fieldValues.Count != FieldCount)
        {
            throw new ArgumentException("The number of field values must match the number of fields.", nameof(fieldValues));
        }
        
        _cards.AddCard(fieldValues);
        
        return this;
    }

    public IAnkiCardCollectionWithFields AddCards(params IReadOnlyCollection<IReadOnlyCollection<string>> cards)
    {
        foreach (var card in cards)
        {
            AddCard(card);
        }
        
        return this;
    }

    public string ExportToString(AnkiCardTextFileSeparator? separatorOverride = null)
    {
        var separator = separatorOverride ?? DefaultSeparator;
        
        var sb = new StringBuilder();
        
        AddHeader(sb, "separator", separator.Name);
        
        if (TagCount > 0)
        {
            AddHeader(sb, "tags", Tags);
        }
        
        AddFieldRow(sb, separator, Fields);
        
        foreach (var card in _cards.Cards)
        {
            AddFieldRow(sb, separator, card);
        }
        
        return sb.ToString();
    }

    public Task<ExportResult> ExportToCsvAsync(string filePath, AnkiCardTextFileSeparator? separatorOverride = null, CancellationToken ct = default)
    {
        return Task.Run(() => ExportToCsv(filePath, separatorOverride), ct);
    }

    private ExportResult ExportToCsv(string filePath, AnkiCardTextFileSeparator? separatorOverride = null)
    {
        var text = ExportToString(separatorOverride);
        
        File.WriteAllText(filePath, text);
        
        return new Success();
    }
    
    private void AddHeader(StringBuilder sb, string header, params IReadOnlyList<string> values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header, nameof(header));
        
        if (values.Count == 0)
        {
            throw new ArgumentException("The number of values must be greater than 0.", nameof(values));
        }
        
        sb.Append(HeaderStart);
        sb.Append(header);
        sb.Append(HeaderNameEnd);
        
        for (var i = 0; i < values.Count; i++)
        {
            if (i > 0) sb.Append(HeaderDelimiter);
            sb.Append(values[i]);
        }
        
        sb.AppendLine();
    }
    
    private void AddFieldRow(StringBuilder sb, AnkiCardTextFileSeparator separator, IReadOnlyList<string> fields)
    {
        for (var i = 0; i < fields.Count; i++)
        {
            if (i > 0) sb.Append(separator.Value);
            sb.Append(FieldContainerMark);
            foreach (var ch in fields[i])
            {
                if (ch == FieldContainerMark)
                {
                    sb.Append(FieldContainerMark);
                }
                
                sb.Append(ch);
            }
            sb.Append(FieldContainerMark);
        }
        
        sb.AppendLine();
    }

    Task<ExportResult> IAnkiCardCollectionWithFields.ExportToAnkiPackageAsync(string filePath, CancellationToken ct)
    {
        throw new NotImplementedException("Will be implemented in the future. Please use Export to csv for now.");
        // 1. Create SQLite database called 'collection.anki2'
        // 2. Create 'decks' table with 'id', 'name', 'description', 'author' columns
    }
}