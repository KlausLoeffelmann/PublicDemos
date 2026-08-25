using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Logging;
using WarpClock.Abstractions;

namespace WarpClock.App;

public sealed class ThemeCustomPropertyStore
{
    private readonly Dictionary<string, Dictionary<string, string>> _valuesByTheme =
        new(StringComparer.OrdinalIgnoreCase);

    public void Load(PersistedThemeState? persistedState)
    {
        _valuesByTheme.Clear();

        if (persistedState?.CustomPropertyValues is not { Count: > 0 } values)
        {
            return;
        }

        foreach (PersistedThemeCustomPropertyValue value in values)
        {
            SetValue(value.ThemeKey, value.PropertyName, value.Value);
        }
    }

    public IReadOnlyList<PersistedThemeCustomPropertyValue> ExportValues()
        => _valuesByTheme
            .OrderBy(theme => theme.Key, StringComparer.OrdinalIgnoreCase)
            .SelectMany(
                theme => theme.Value
                    .OrderBy(property => property.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(property => new PersistedThemeCustomPropertyValue
                    {
                        ThemeKey = theme.Key,
                        PropertyName = property.Key,
                        Value = property.Value,
                    }))
            .ToArray();

    public void Apply(ThemeCustomPropertySession session, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(logger);

        if (!_valuesByTheme.TryGetValue(session.ThemeKey, out Dictionary<string, string>? persistedValues)
            || persistedValues.Count == 0)
        {
            return;
        }

        foreach ((string propertyName, string persistedValue) in persistedValues.ToArray())
        {
            if (!session.TryGetProperty(propertyName, out ThemeCustomPropertyDefinition definition))
            {
                logger.LogWarning(
                    "Omitting persisted custom theme property {ThemeKey}.{PropertyName} because the active theme no longer exposes it.",
                    session.ThemeKey,
                    propertyName);
                RemoveValue(session.ThemeKey, propertyName);
                continue;
            }

            try
            {
                object? value = definition.ConvertFromInvariantString(persistedValue);
                definition.SetValue(session.Theme, value);
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException or InvalidCastException or NotSupportedException or TargetInvocationException)
            {
                logger.LogWarning(
                    ex,
                    "Omitting persisted custom theme property {ThemeKey}.{PropertyName} because '{PersistedValue}' is invalid.",
                    session.ThemeKey,
                    propertyName,
                    persistedValue);
                RemoveValue(session.ThemeKey, propertyName);
            }
        }
    }

    public void CaptureValue(ThemeCustomPropertySession session, string propertyName, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(logger);

        if (!session.TryGetProperty(propertyName, out ThemeCustomPropertyDefinition definition))
        {
            return;
        }

        try
        {
            string persistedValue = definition.ConvertToInvariantString(session.Theme);
            SetValue(session.ThemeKey, propertyName, persistedValue);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidCastException or NotSupportedException or TargetInvocationException)
        {
            logger.LogWarning(
                ex,
                "Omitting custom theme property {ThemeKey}.{PropertyName} because it could not be persisted as an invariant string.",
                session.ThemeKey,
                propertyName);
            RemoveValue(session.ThemeKey, propertyName);
        }
    }

    private void SetValue(string themeKey, string propertyName, string value)
    {
        string normalizedThemeKey = ThemeCatalogInfo.NormalizeThemeKey(themeKey);
        string normalizedPropertyName = propertyName.Trim();
        if (string.IsNullOrWhiteSpace(normalizedThemeKey) || string.IsNullOrWhiteSpace(normalizedPropertyName))
        {
            return;
        }

        if (!_valuesByTheme.TryGetValue(normalizedThemeKey, out Dictionary<string, string>? properties))
        {
            properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _valuesByTheme[normalizedThemeKey] = properties;
        }

        properties[normalizedPropertyName] = value ?? string.Empty;
    }

    private void RemoveValue(string themeKey, string propertyName)
    {
        string normalizedThemeKey = ThemeCatalogInfo.NormalizeThemeKey(themeKey);
        if (!_valuesByTheme.TryGetValue(normalizedThemeKey, out Dictionary<string, string>? properties))
        {
            return;
        }

        properties.Remove(propertyName);
        if (properties.Count == 0)
        {
            _valuesByTheme.Remove(normalizedThemeKey);
        }
    }
}

public sealed class ThemeCustomPropertySession
{
    private readonly Dictionary<string, ThemeCustomPropertyDefinition> _propertiesByName;

    private ThemeCustomPropertySession(string themeKey, IClockTheme theme, IReadOnlyList<ThemeCustomPropertyDefinition> properties)
    {
        ThemeKey = ThemeCatalogInfo.NormalizeThemeKey(themeKey);
        Theme = theme;
        Properties = properties;
        _propertiesByName = properties.ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);
    }

    public string ThemeKey { get; }

    public IClockTheme Theme { get; }

    public IReadOnlyList<ThemeCustomPropertyDefinition> Properties { get; }

    public static ThemeCustomPropertySession Create(
        string themeKey,
        IClockTheme theme,
        ThemeCustomPropertyStore store,
        ILogger logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(themeKey);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(logger);

        ThemeCustomPropertySession session = new(
            themeKey,
            theme,
            ThemeCustomPropertyDefinition.Discover(theme, logger));

        store.Apply(session, logger);
        return session;
    }

    public bool ContainsProperty(string? propertyName)
        => !string.IsNullOrWhiteSpace(propertyName) && _propertiesByName.ContainsKey(propertyName);

    public bool TryGetProperty(string propertyName, out ThemeCustomPropertyDefinition definition)
        => _propertiesByName.TryGetValue(propertyName, out definition!);
}

public sealed class ThemeCustomPropertyDefinition
{
    private const string CustomPropertyCategory = "Custom Properties";

    public ThemeCustomPropertyDefinition(
        PropertyInfo property,
        TypeConverter converter,
        string description,
        string displayName)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(converter);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Property = property;
        Converter = converter;
        Description = description;
        DisplayName = displayName;
    }

    public string Name => Property.Name;

    public string DisplayName { get; }

    public string Description { get; }

    public Type PropertyType => Property.PropertyType;

    public PropertyInfo Property { get; }

    public TypeConverter Converter { get; }

    public object? GetValue(IClockTheme theme) => Property.GetValue(theme);

    public void SetValue(IClockTheme theme, object? value) => Property.SetValue(theme, value);

    public object? ConvertFromInvariantString(string value) => Converter.ConvertFromInvariantString(value);

    public string ConvertToInvariantString(IClockTheme theme)
        => Converter.ConvertToInvariantString(GetValue(theme)) ?? string.Empty;

    public static IReadOnlyList<ThemeCustomPropertyDefinition> Discover(IClockTheme theme, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(logger);

        List<ThemeCustomPropertyDefinition> properties = [];
        foreach (PropertyInfo property in theme.GetType()
                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase))
        {
            bool browsable = property.GetCustomAttribute<BrowsableAttribute>(inherit: true)?.Browsable == true;
            string? description = property.GetCustomAttribute<DescriptionAttribute>(inherit: true)?.Description;
            string? category = property.GetCustomAttribute<CategoryAttribute>(inherit: true)?.Category;
            bool inCustomCategory = string.Equals(category, CustomPropertyCategory, StringComparison.Ordinal);
            bool looksCustom = browsable || !string.IsNullOrWhiteSpace(description) || inCustomCategory;

            if (!looksCustom)
            {
                continue;
            }

            if (!TryCreate(theme, property, browsable, description, inCustomCategory, out ThemeCustomPropertyDefinition? definition, out string? reason))
            {
                logger.LogWarning(
                    "Omitting malformed custom theme property {ThemeType}.{PropertyName}: {Reason}",
                    theme.GetType().FullName ?? theme.GetType().Name,
                    property.Name,
                    reason);
                continue;
            }

            properties.Add(definition!);
        }

        return properties;
    }

    private static bool TryCreate(
        IClockTheme theme,
        PropertyInfo property,
        bool browsable,
        string? description,
        bool inCustomCategory,
        out ThemeCustomPropertyDefinition? definition,
        out string? reason)
    {
        definition = null;
        reason = null;

        if (!browsable)
        {
            reason = "Browsable(true) is required.";
            return false;
        }

        if (!inCustomCategory)
        {
            reason = $"Category(\"{CustomPropertyCategory}\") is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            reason = "DescriptionAttribute is required.";
            return false;
        }

        if (property.GetIndexParameters().Length > 0)
        {
            reason = "Indexed properties are not supported.";
            return false;
        }

        if (property.GetMethod?.IsPublic != true || property.SetMethod?.IsPublic != true)
        {
            reason = "A public getter and setter are required.";
            return false;
        }

        TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
        if (!converter.CanConvertTo(typeof(string)) || !converter.CanConvertFrom(typeof(string)))
        {
            reason = $"Type '{property.PropertyType.FullName}' does not support invariant string conversion.";
            return false;
        }

        try
        {
            converter.ConvertToInvariantString(property.GetValue(theme));
        }
        catch (Exception ex) when (ex is InvalidCastException or NotSupportedException or TargetInvocationException)
        {
            reason = $"Type '{property.PropertyType.FullName}' failed invariant serialization: {ex.Message}";
            return false;
        }

        string displayName = property.GetCustomAttribute<DisplayNameAttribute>(inherit: true)?.DisplayName
            ?? property.Name;

        definition = new ThemeCustomPropertyDefinition(property, converter, description, displayName);
        return true;
    }
}

public sealed class ThemePropertyGridAdapter(object baseSettings) : ICustomTypeDescriptor
{
    private readonly object _baseSettings = baseSettings ?? throw new ArgumentNullException(nameof(baseSettings));
    private ThemeCustomPropertySession? _themeSession;

    public void SetThemeSession(ThemeCustomPropertySession? themeSession) => _themeSession = themeSession;

    public bool IsThemeCustomProperty(string? propertyName)
        => _themeSession?.ContainsProperty(propertyName) == true;

    AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(_baseSettings, true);

    string? ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(_baseSettings, true);

    string? ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(_baseSettings, true);

    TypeConverter? ICustomTypeDescriptor.GetConverter() => TypeDescriptor.GetConverter(_baseSettings, true);

    EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(_baseSettings, true);

    PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(_baseSettings, true);

    object? ICustomTypeDescriptor.GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(_baseSettings, editorBaseType, true);

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(_baseSettings, true);

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes)
        => TypeDescriptor.GetEvents(_baseSettings, attributes, true);

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => GetProperties(attributes: null);

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes) => GetProperties(attributes);

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd)
        => pd?.ComponentType == typeof(ThemePropertyGridAdapter) ? this : _baseSettings;

    private PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
    {
        List<PropertyDescriptor> properties =
        [
            .. TypeDescriptor.GetProperties(_baseSettings, attributes, true).Cast<PropertyDescriptor>(),
        ];

        if (_themeSession is not null)
        {
            properties.AddRange(_themeSession.Properties.Select(property => new ThemeCustomPropertyDescriptor(_themeSession, property)));
        }

        return new PropertyDescriptorCollection([.. properties], readOnly: true);
    }

    private sealed class ThemeCustomPropertyDescriptor(
        ThemeCustomPropertySession session,
        ThemeCustomPropertyDefinition property)
        : PropertyDescriptor(property.Name, CreateAttributes(property))
    {
        public override Type ComponentType => typeof(ThemePropertyGridAdapter);

        public override bool IsReadOnly => false;

        public override Type PropertyType => property.PropertyType;

        public override bool CanResetValue(object component) => false;

        public override object? GetValue(object? component) => property.GetValue(session.Theme);

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object? component, object? value)
        {
            property.SetValue(session.Theme, value);
            OnValueChanged(component, EventArgs.Empty);
        }

        public override bool ShouldSerializeValue(object component) => false;

        private static Attribute[] CreateAttributes(ThemeCustomPropertyDefinition property)
        {
            List<Attribute> attributes =
            [
                BrowsableAttribute.Yes,
                new CategoryAttribute("Custom Properties"),
                new DescriptionAttribute(property.Description),
            ];

            if (!string.Equals(property.DisplayName, property.Name, StringComparison.Ordinal))
            {
                attributes.Add(new DisplayNameAttribute(property.DisplayName));
            }

            return [.. attributes];
        }
    }
}
