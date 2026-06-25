namespace LargeFormSmokeTest.Data;

using LargeFormSmokeTest.Models;

/// <summary>
///  Produces a deterministic <see cref="DeclarationDetail"/> for a given person and declaration.
///  The detailed Anlage figures are not part of the source JSON, so they are synthesized here from
///  a seed derived from the person id and tax year. Because the seed is stable, the very same
///  numbers are produced every time a person/year is opened, and the summary fields are taken
///  straight from the declaration so the detail stays consistent with the overview grids.
/// </summary>
public static class DeclarationDetailFactory
{
    private static readonly string[] s_offices =
        ["Finanzamt Berlin-Mitte", "Finanzamt München", "Finanzamt Köln-Süd", "Finanzamt Hamburg", "Finanzamt Frankfurt"];

    private static readonly string[] s_employers =
        ["Globex GmbH", "Initech AG", "Umbrella KG", "Soylent SE", "Hooli GmbH", "Wernham Hogg AG"];

    private static readonly string[] s_banks =
        ["Sparkasse", "Volksbank", "Deutsche Bank", "ING", "DKB", "Comdirect"];

    private static readonly string[] s_trades =
        ["Einzelhandel", "Handwerk", "IT-Beratung", "Gastronomie", "Onlinehandel"];

    private static readonly string[] s_activities =
        ["Freie Autorin", "Grafikdesign", "Unternehmensberatung", "Fotografie", "Übersetzungen"];

    private static readonly string[] s_measures =
        ["Wärmedämmung", "Heizungstausch", "Fensteraustausch", "Photovoltaik", "Lüftungsanlage"];

    /// <summary>
    ///  Determines, deterministically, which return obligation applies to a declaration. Roughly a
    ///  third of declarations are the combined wage+income case (employed plus a side hustle).
    /// </summary>
    public static TaxObligation ObligationFor(Person person, Declaration declaration)
        => (person.Id + declaration.Year) % 3 == 0
            ? TaxObligation.LohnsteuerUndEinkommensteuer
            : TaxObligation.Einkommensteuer;

    /// <summary>Builds the deterministic detail for the given person and declaration.</summary>
    public static DeclarationDetail Create(Person person, Declaration declaration)
    {
        // Seed strictly from stable identity + year so results never change between runs.
        Random rng = new(person.Id * 100_000 + declaration.Year);

        decimal Money(double min, double max)
            => Math.Round((decimal)(min + rng.NextDouble() * (max - min)), 2);

        decimal Part(decimal value, double min, double max)
            => Math.Round(value * (decimal)(min + rng.NextDouble() * (max - min)), 2);

        int IntRange(int min, int max)
            => rng.Next(min, max + 1);

        bool Chance(double p)
            => rng.NextDouble() < p;

        TEnum PickEnum<TEnum>() where TEnum : struct, Enum
        {
            TEnum[] values = Enum.GetValues<TEnum>();

            return values[rng.Next(values.Length)];
        }

        string Pick(string[] values)
            => values[rng.Next(values.Length)];

        decimal basis = declaration.AssessmentBasis;
        bool combined = declaration.Obligation is TaxObligation.LohnsteuerUndEinkommensteuer;
        bool churchMember = Chance(0.55);

        decimal grossWage = Part(basis, 0.45, 0.70);
        decimal wageTax = Part(declaration.AssessedTax, 0.55, 0.85);

        DeclarationDetail detail = new()
        {
            // Mantelbogen / Stammdaten
            TaxNumber = person.TaxNumber,
            TaxOffice = Pick(s_offices),
            Year = declaration.Year,
            TaxId = $"{rng.Next(10, 99)} {rng.Next(100, 999)} {rng.Next(100, 999)} {rng.Next(100, 999)}",
            MaritalStatus = PickEnum<MaritalStatus>(),
            Religion = churchMember ? PickEnum<ReligionAffiliation>() : ReligionAffiliation.Keine,
            Iban = $"DE{rng.Next(10, 99)} {rng.Next(1000, 9999)} {rng.Next(1000, 9999)} {rng.Next(1000, 9999)} {rng.Next(1000, 9999)} {rng.Next(10, 99)}",
            SubmittedElectronically = Chance(0.8),
            Obligation = declaration.Obligation,

            // Summary mirrors the declaration exactly.
            AssessmentBasis = declaration.AssessmentBasis,
            AssessedTax = declaration.AssessedTax,
            OutstandingAmount = declaration.OutstandingAmount,
            Status = declaration.Status,

            // Anlage N
            Employer = Pick(s_employers),
            GrossWage = grossWage,
            WageTaxWithheld = wageTax,
            SolidaritySurcharge = Math.Round(wageTax * 0.055m, 2),
            ChurchTax = churchMember ? Math.Round(wageTax * 0.09m, 2) : 0m,
            CommutingKm = IntRange(3, 65),
            WorkDays = IntRange(180, 220),
            WorkEquipment = Money(150, 2400),
            ProfessionalAssociationFees = Money(0, 600),
            FurtherEducation = Money(0, 3500),
            DoubleHousehold = Chance(0.2),
            HomeOffice = Chance(0.6),

            // Anlage KAP
            CapitalIncome = Part(basis, 0.0, 0.12),
            CapitalGainsTax = Money(0, 4000),
            SolidaritySurchargeKap = Money(0, 220),
            SaverAllowance = 1000m,
            ForeignCapitalIncome = Money(0, 3000),
            WithholdingTaxCredit = Money(0, 800),
            Bank = Pick(s_banks),
            LossCarryforward = Money(0, 5000),
            FavourableCheck = Chance(0.3),
            ChurchTaxLiableKap = churchMember,

            // Anlage V
            PropertyAddress = $"{Pick(["Lindenstr.", "Bahnhofstr.", "Gartenweg", "Seestr."])} {IntRange(1, 120)}, {person.CurrentAddress.PostalCode} {person.CurrentAddress.City}",
            RentalIncome = Part(basis, 0.0, 0.20),
            ServiceCharges = Money(800, 4200),
            Depreciation = Money(1500, 9000),
            DebtInterest = Money(0, 7000),
            MaintenanceCost = Money(0, 6000),
            PropertyTax = Money(200, 1400),
            Insurance = Money(150, 900),
            AdminCost = Money(0, 600),
            VacancyMonths = IntRange(0, 4),
            ConstructionYear = IntRange(1955, 2015),
            FullyLet = Chance(0.75),

            // Anlage G
            CompanyName = combined ? $"{person.LastName} {Pick(["Trading", "Service", "Handel", "Werkstatt"])}" : string.Empty,
            TradeType = Pick(s_trades),
            BusinessProfit = combined ? Part(basis, 0.10, 0.30) : 0m,
            Revenue = combined ? Money(20000, 180000) : 0m,
            TradeTax = combined ? Money(0, 9000) : 0m,
            TradeTaxCredit = combined ? Money(0, 8000) : 0m,
            InvestmentDeduction = Money(0, 12000),
            Employees = combined ? IntRange(0, 8) : 0,
            OperatingExpenses = combined ? Money(5000, 60000) : 0m,
            ParticipationPercent = Money(0, 100),
            SmallBusiness = !combined && Chance(0.4),

            // Anlage S
            Activity = Pick(s_activities),
            SelfEmployedIncome = combined ? Money(8000, 70000) : Money(0, 12000),
            SelfEmployedExpenses = Money(1000, 25000),
            SelfEmployedProfit = combined ? Part(basis, 0.08, 0.25) : Money(0, 8000),
            VatLiable = Chance(0.5),
            Prepayments = Money(0, 6000),
            ArtistsSocialFund = Money(0, 1800),
            TravelCost = Money(0, 3200),
            Entertainment = Money(0, 1200),
            SelfEmployedDepreciation = Money(0, 4500),

            // Vorsorgeaufwand
            PensionInsurance = Part(grossWage, 0.08, 0.10),
            HealthInsurance = Part(grossWage, 0.07, 0.085),
            CareInsurance = Part(grossWage, 0.015, 0.023),
            UnemploymentInsurance = Part(grossWage, 0.011, 0.013),
            LiabilityInsurance = Money(60, 350),
            AccidentInsurance = Money(80, 500),
            Riester = Money(0, 2100),
            Ruerup = Money(0, 6000),
            BasicPension = Money(0, 9000),
            SupplementaryPension = Money(0, 3000),

            // Anlage Kind
            ChildName = $"{Pick(["Mia", "Leon", "Emma", "Paul", "Hannah", "Noah"])} {person.LastName}",
            ChildBirthDate = new DateOnly(IntRange(2004, 2022), IntRange(1, 12), IntRange(1, 28)),
            ChildTaxId = $"{rng.Next(10, 99)} {rng.Next(100, 999)} {rng.Next(100, 999)} {rng.Next(100, 999)}",
            ChildBenefit = Money(2400, 3000),
            ChildAllowance = Money(0, 8952),
            CareCost = Money(0, 4000),
            SchoolFees = Money(0, 5000),
            EducationStatus = PickEnum<EducationStatus>(),
            AwayAccommodation = Chance(0.25),
            DisabilityDegree = Chance(0.1) ? IntRange(20, 100) : 0,

            // Sonderausgaben
            Donations = Money(0, 2500),
            ChurchTaxPaid = churchMember ? Money(100, 1800) : 0m,
            AlimonyPaid = Chance(0.2) ? Money(2000, 13805) : 0m,
            VocationalTraining = Money(0, 6000),
            RetirementProvision = Money(0, 25000),
            ChurchMember = churchMember,

            // Außergewöhnliche Belastungen
            MedicalCost = Money(0, 5000),
            CareCostExtraordinary = Money(0, 9000),
            DisabilityAllowance = Money(0, 7400),
            CraftsmenServices = Money(0, 6000),
            HouseholdServices = Money(0, 20000),
            CareLevel = Chance(0.15) ? IntRange(1, 5) : 0,
            LumpSumDisability = Chance(0.12),

            // Anlage R
            PensionPayments = Chance(0.3) ? Money(6000, 28000) : 0m,
            PensionStartYear = IntRange(2008, declaration.Year),
            PensionType = PickEnum<PensionType>(),
            TaxablePortion = Money(0, 22000),
            AdjustmentAmount = Money(0, 1500),

            // Anlage SO
            PrivateSaleGains = Money(0, 9000),
            RecurringIncome = Money(0, 4000),
            OtherIncome = Money(0, 6000),
            SpeculationPeriodExceeded = Chance(0.5),

            // Anlage Unterhalt
            RecipientName = $"{Pick(["Anna", "Karl", "Ute", "Georg"])} {Pick(["Bauer", "Schulz", "Klein", "Wolf"])}",
            MaintenanceAmount = Chance(0.25) ? Money(2000, 11000) : 0m,
            RecipientIncome = Money(0, 6000),
            MonthsSupported = IntRange(0, 12),
            HouseholdAbroad = Chance(0.15),

            // Anlage Energetische Maßnahmen (§35c)
            Measure = Pick(s_measures),
            MeasureTotalCost = Chance(0.4) ? Money(8000, 60000) : 0m,
            MeasureEligibleAmount = Money(0, 14000),
            MeasureCompletionYear = IntRange(declaration.Year - 1, declaration.Year),
            CertifiedCompany = Chance(0.85),

            // Lohnsteuerbescheinigung
            TaxClass = PickEnum<TaxClass>(),
            ETin = $"{(char)('A' + rng.Next(26))}{(char)('A' + rng.Next(26))}{rng.Next(100000000, 999999999)}",
            GrossWageLst = grossWage,
            WageTaxLst = wageTax,
            SolidaritySurchargeLst = Math.Round(wageTax * 0.055m, 2),
            ChurchTaxLst = churchMember ? Math.Round(wageTax * 0.09m, 2) : 0m,
            EmployerLst = Pick(s_employers),
            InsuranceDays = IntRange(180, 365)
        };

        return detail;
    }
}
