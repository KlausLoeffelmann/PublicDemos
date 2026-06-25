namespace LargeFormSmokeTest.Data;

using System.Text.Json;
using System.Text.Json.Serialization;
using LargeFormSmokeTest.Models;

/// <summary>
///  Singleton, read-once repository holding the whole demo dataset in memory. All forms
///  read their data from <see cref="Instance"/>; this guarantees a single source of truth.
/// </summary>
public sealed class TaxRepository
{
    private static readonly Lazy<TaxRepository> s_instance = new(static () => new TaxRepository());

    private readonly TaxDemoDataset _dataset;

    private TaxRepository()
    {
        _dataset = LoadDataset();
    }

    /// <summary>Gets the process-wide singleton instance.</summary>
    public static TaxRepository Instance
        => s_instance.Value;

    /// <summary>Gets all tax payers in load order.</summary>
    public IReadOnlyList<Person> Persons
        => _dataset.Persons;

    /// <summary>Gets the schema version of the loaded dataset.</summary>
    public string SchemaVersion
        => _dataset.SchemaVersion;

    /// <summary>
    ///  Locates the dataset file next to the executable and deserializes it. The JSON uses
    ///  string enum values and ISO dates, hence the custom converter options.
    /// </summary>
    private static TaxDemoDataset LoadDataset()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "copilot", "tax-demo-data.json");

        if (!File.Exists(path))
        {
            // Falling back to an empty dataset keeps the warm-up form alive even when the
            // data file is missing, which is preferable to crashing the perf harness.
            return new TaxDemoDataset();
        }

        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        using FileStream stream = File.OpenRead(path);
        TaxDemoDataset? dataset = JsonSerializer.Deserialize<TaxDemoDataset>(stream, options);

        return dataset ?? new TaxDemoDataset();
    }
}
