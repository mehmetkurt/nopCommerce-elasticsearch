namespace Nop.Plugin.SearchProvider.Elasticsearch.Settings;

/// <summary>
/// Enumeration for various search methodologies.
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Fuzzy search accommodates minor misspellings in queries.
    /// For instance, searching for 'appl' might still yield results for 'apple'.
    /// </summary>
    Fuzzy = 10,

    /// <summary>
    /// Wildcard search allows pattern-based queries.
    /// The * wildcard character is used to match any sequence of characters.
    /// For example, searching for 'doc*' might return 'document' and 'doctor'.
    /// </summary>
    Wildcard = 20,

    /// <summary>
    /// Exact search requires the query to precisely match the indexed terms.
    /// Only exact matches will be returned.
    /// </summary>
    Exact = 30
}
