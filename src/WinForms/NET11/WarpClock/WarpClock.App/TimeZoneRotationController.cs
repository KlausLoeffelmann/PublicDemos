namespace WarpClock.App;

/// <summary>
///  Alternates the displayed timezone through the configured default zone.
/// </summary>
internal sealed class TimeZoneRotationController
{
    private TimeZoneOptions _options = new();
    private int _alternateIndex = -1;
    private double _elapsedSeconds;
    private bool _showingDefault = true;

    public ConfiguredTimeZone Current { get; private set; } = new()
    {
        TimeZoneId = TimeZoneInfo.Local.Id,
        DisplayName = "Local",
        IsDefault = true,
    };

    public void Reset(TimeZoneOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Clone();
        _options.Normalize();
        _alternateIndex = -1;
        _elapsedSeconds = 0d;
        _showingDefault = true;
        Current = GetDefault();
    }

    public bool Advance(TimeSpan elapsed)
    {
        if (!_options.Enabled || _options.Entries.Count <= 1)
        {
            if (!Current.IsDefault)
            {
                Current = GetDefault();
                _showingDefault = true;
                _elapsedSeconds = 0d;
                return true;
            }

            return false;
        }

        _elapsedSeconds += Math.Max(0d, elapsed.TotalSeconds);
        bool changed = false;

        while (true)
        {
            double interval = _showingDefault
                ? _options.ChangeToNextSeconds
                : _options.ReturnToDefaultSeconds;
            if (_elapsedSeconds < interval)
            {
                return changed;
            }

            _elapsedSeconds -= interval;
            if (_showingDefault)
            {
                Current = GetNextAlternate();
                _showingDefault = false;
            }
            else
            {
                Current = GetDefault();
                _showingDefault = true;
            }

            changed = true;
        }
    }

    private ConfiguredTimeZone GetDefault()
        => _options.Entries.FirstOrDefault(entry => entry.IsDefault)
            ?? _options.Entries.FirstOrDefault()
            ?? new ConfiguredTimeZone
            {
                TimeZoneId = TimeZoneInfo.Local.Id,
                DisplayName = "Local",
                IsDefault = true,
            };

    private ConfiguredTimeZone GetNextAlternate()
    {
        for (int attempts = 0; attempts < _options.Entries.Count; attempts++)
        {
            _alternateIndex = (_alternateIndex + 1) % _options.Entries.Count;
            ConfiguredTimeZone candidate = _options.Entries[_alternateIndex];
            if (!candidate.IsDefault)
            {
                return candidate;
            }
        }

        return GetDefault();
    }
}
