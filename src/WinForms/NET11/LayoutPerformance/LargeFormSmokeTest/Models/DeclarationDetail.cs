namespace LargeFormSmokeTest.Models;

/// <summary>
///  Rich, per-declaration detail that backs the dense <c>DeclarationForm</c> sections. These
///  values do not exist in the source JSON (which only has summary figures); they are produced
///  deterministically by <see cref="LargeFormSmokeTest.Data.DeclarationDetailFactory"/> so the
///  same numbers appear every time a given person/year is opened.
/// </summary>
public sealed class DeclarationDetail
{
    // ---- Mantelbogen / Stammdaten ---------------------------------------------------
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxOffice { get; set; } = string.Empty;
    public int Year { get; set; }
    public string TaxId { get; set; } = string.Empty;
    public MaritalStatus MaritalStatus { get; set; }
    public ReligionAffiliation Religion { get; set; }
    public string Iban { get; set; } = string.Empty;
    public bool SubmittedElectronically { get; set; }
    public TaxObligation Obligation { get; set; }

    // ---- Summary (mirrors the Declaration) ------------------------------------------
    public decimal AssessmentBasis { get; set; }
    public decimal AssessedTax { get; set; }
    public decimal OutstandingAmount { get; set; }
    public DeclarationStatus Status { get; set; }

    // ---- Anlage N (employment) ------------------------------------------------------
    public string Employer { get; set; } = string.Empty;
    public decimal GrossWage { get; set; }
    public decimal WageTaxWithheld { get; set; }
    public decimal SolidaritySurcharge { get; set; }
    public decimal ChurchTax { get; set; }
    public int CommutingKm { get; set; }
    public int WorkDays { get; set; }
    public decimal WorkEquipment { get; set; }
    public decimal ProfessionalAssociationFees { get; set; }
    public decimal FurtherEducation { get; set; }
    public bool DoubleHousehold { get; set; }
    public bool HomeOffice { get; set; }

    // ---- Anlage KAP (investment) ----------------------------------------------------
    public decimal CapitalIncome { get; set; }
    public decimal CapitalGainsTax { get; set; }
    public decimal SolidaritySurchargeKap { get; set; }
    public decimal SaverAllowance { get; set; }
    public decimal ForeignCapitalIncome { get; set; }
    public decimal WithholdingTaxCredit { get; set; }
    public string Bank { get; set; } = string.Empty;
    public decimal LossCarryforward { get; set; }
    public bool FavourableCheck { get; set; }
    public bool ChurchTaxLiableKap { get; set; }

    // ---- Anlage V (rental & leasing) ------------------------------------------------
    public string PropertyAddress { get; set; } = string.Empty;
    public decimal RentalIncome { get; set; }
    public decimal ServiceCharges { get; set; }
    public decimal Depreciation { get; set; }
    public decimal DebtInterest { get; set; }
    public decimal MaintenanceCost { get; set; }
    public decimal PropertyTax { get; set; }
    public decimal Insurance { get; set; }
    public decimal AdminCost { get; set; }
    public int VacancyMonths { get; set; }
    public int ConstructionYear { get; set; }
    public bool FullyLet { get; set; }

    // ---- Anlage G (business) --------------------------------------------------------
    public string CompanyName { get; set; } = string.Empty;
    public string TradeType { get; set; } = string.Empty;
    public decimal BusinessProfit { get; set; }
    public decimal Revenue { get; set; }
    public decimal TradeTax { get; set; }
    public decimal TradeTaxCredit { get; set; }
    public decimal InvestmentDeduction { get; set; }
    public int Employees { get; set; }
    public decimal OperatingExpenses { get; set; }
    public decimal ParticipationPercent { get; set; }
    public bool SmallBusiness { get; set; }

    // ---- Anlage S (self-employment) -------------------------------------------------
    public string Activity { get; set; } = string.Empty;
    public decimal SelfEmployedIncome { get; set; }
    public decimal SelfEmployedExpenses { get; set; }
    public decimal SelfEmployedProfit { get; set; }
    public bool VatLiable { get; set; }
    public decimal Prepayments { get; set; }
    public decimal ArtistsSocialFund { get; set; }
    public decimal TravelCost { get; set; }
    public decimal Entertainment { get; set; }
    public decimal SelfEmployedDepreciation { get; set; }

    // ---- Vorsorgeaufwand (pension expenses) -----------------------------------------
    public decimal PensionInsurance { get; set; }
    public decimal HealthInsurance { get; set; }
    public decimal CareInsurance { get; set; }
    public decimal UnemploymentInsurance { get; set; }
    public decimal LiabilityInsurance { get; set; }
    public decimal AccidentInsurance { get; set; }
    public decimal Riester { get; set; }
    public decimal Ruerup { get; set; }
    public decimal BasicPension { get; set; }
    public decimal SupplementaryPension { get; set; }

    // ---- Anlage Kind ----------------------------------------------------------------
    public string ChildName { get; set; } = string.Empty;
    public DateOnly ChildBirthDate { get; set; }
    public string ChildTaxId { get; set; } = string.Empty;
    public decimal ChildBenefit { get; set; }
    public decimal ChildAllowance { get; set; }
    public decimal CareCost { get; set; }
    public decimal SchoolFees { get; set; }
    public EducationStatus EducationStatus { get; set; }
    public bool AwayAccommodation { get; set; }
    public int DisabilityDegree { get; set; }

    // ---- Sonderausgaben -------------------------------------------------------------
    public decimal Donations { get; set; }
    public decimal ChurchTaxPaid { get; set; }
    public decimal AlimonyPaid { get; set; }
    public decimal VocationalTraining { get; set; }
    public decimal RetirementProvision { get; set; }
    public bool ChurchMember { get; set; }

    // ---- Außergewöhnliche Belastungen -----------------------------------------------
    public decimal MedicalCost { get; set; }
    public decimal CareCostExtraordinary { get; set; }
    public decimal DisabilityAllowance { get; set; }
    public decimal CraftsmenServices { get; set; }
    public decimal HouseholdServices { get; set; }
    public int CareLevel { get; set; }
    public bool LumpSumDisability { get; set; }

    // ---- Anlage R (pensions) --------------------------------------------------------
    public decimal PensionPayments { get; set; }
    public int PensionStartYear { get; set; }
    public PensionType PensionType { get; set; }
    public decimal TaxablePortion { get; set; }
    public decimal AdjustmentAmount { get; set; }

    // ---- Anlage SO (other income) ---------------------------------------------------
    public decimal PrivateSaleGains { get; set; }
    public decimal RecurringIncome { get; set; }
    public decimal OtherIncome { get; set; }
    public bool SpeculationPeriodExceeded { get; set; }

    // ---- Anlage Unterhalt -----------------------------------------------------------
    public string RecipientName { get; set; } = string.Empty;
    public decimal MaintenanceAmount { get; set; }
    public decimal RecipientIncome { get; set; }
    public int MonthsSupported { get; set; }
    public bool HouseholdAbroad { get; set; }

    // ---- Anlage Energetische Maßnahmen (§35c) ---------------------------------------
    public string Measure { get; set; } = string.Empty;
    public decimal MeasureTotalCost { get; set; }
    public decimal MeasureEligibleAmount { get; set; }
    public int MeasureCompletionYear { get; set; }
    public bool CertifiedCompany { get; set; }

    // ---- Lohnsteuerbescheinigung (wage-tax statement) -------------------------------
    public TaxClass TaxClass { get; set; }
    public string ETin { get; set; } = string.Empty;
    public decimal GrossWageLst { get; set; }
    public decimal WageTaxLst { get; set; }
    public decimal SolidaritySurchargeLst { get; set; }
    public decimal ChurchTaxLst { get; set; }
    public string EmployerLst { get; set; } = string.Empty;
    public int InsuranceDays { get; set; }
}
