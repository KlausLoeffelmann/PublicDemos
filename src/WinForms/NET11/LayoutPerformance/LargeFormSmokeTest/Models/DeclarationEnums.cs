namespace LargeFormSmokeTest.Models;

/// <summary>Marital status (Familienstand). Rendered as a radio-button group.</summary>
public enum MaritalStatus
{
    /// <summary>Single (ledig).</summary>
    Ledig,

    /// <summary>Married (verheiratet).</summary>
    Verheiratet,

    /// <summary>Divorced (geschieden).</summary>
    Geschieden,

    /// <summary>Widowed (verwitwet).</summary>
    Verwitwet
}

/// <summary>Religious affiliation (Religionszugehörigkeit). Rendered as a radio-button group.</summary>
public enum ReligionAffiliation
{
    /// <summary>None.</summary>
    Keine,

    /// <summary>Roman Catholic.</summary>
    RoemischKatholisch,

    /// <summary>Protestant.</summary>
    Evangelisch,

    /// <summary>Other.</summary>
    Sonstige
}

/// <summary>Education status of a child (Ausbildungsstatus). Rendered as a radio-button group.</summary>
public enum EducationStatus
{
    /// <summary>At school.</summary>
    Schule,

    /// <summary>At university.</summary>
    Studium,

    /// <summary>In vocational training.</summary>
    Ausbildung,

    /// <summary>Employed.</summary>
    Berufstaetig
}

/// <summary>German wage-tax class (Steuerklasse I–VI). Rendered as a radio-button group.</summary>
public enum TaxClass
{
    /// <summary>Class I (single).</summary>
    I,

    /// <summary>Class II (single parent).</summary>
    II,

    /// <summary>Class III (married, higher earner).</summary>
    III,

    /// <summary>Class IV (married, equal earners).</summary>
    IV,

    /// <summary>Class V (married, lower earner).</summary>
    V,

    /// <summary>Class VI (secondary employment).</summary>
    VI
}

/// <summary>Type of pension (Rentenart). Rendered as a radio-button group.</summary>
public enum PensionType
{
    /// <summary>Statutory pension.</summary>
    GesetzlicheRente,

    /// <summary>Occupational pension.</summary>
    BetrieblicheRente,

    /// <summary>Private pension.</summary>
    PrivateRente
}
