

using System;

namespace CDR.DataHolder.Repository.Entities.Json
{

    public class Root
    {
        public Holders[] holders { get; set; }
        public ClientCache[] clientCache { get; set; }
        public RegisterCache[] registerCache { get; set; }
    }

    public class Holders
    {
        public string holderId { get; set; }
        public Holder1 holder { get; set; }
    }

    public class Holder1
    {
        public Unauthenticated unauthenticated { get; set; }
        public Authenticated authenticated { get; set; }
    }

    public class Unauthenticated
    {
        public Banking banking { get; set; }
        public Admin admin { get; set; }
    }

    public class Banking
    {
        public Product[] products { get; set; }
    }

    public class Product
    {
        public string productId { get; set; }
        public DateTime effectiveFrom { get; set; }
        public DateTime effectiveTo { get; set; }
        public DateTime lastUpdated { get; set; }
        public string productCategory { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string brand { get; set; }
        public string brandName { get; set; }
        public string applicationUri { get; set; }
        public bool isTailored { get; set; }
        public AdditionalInformation additionalInformation { get; set; }
        public CardArt[] cardArt { get; set; }
        public Bundle[] bundles { get; set; }
        public Feature[] features { get; set; }
        public Constraint[] constraints { get; set; }
        public Eligibility[] eligibility { get; set; }
        public Fee[] fees { get; set; }
        public DepositRate[] depositRates { get; set; }
        public LendingRate[] lendingRates { get; set; }
    }

    public class AdditionalInformation
    {
        public string overviewUri { get; set; }
        public string termsUri { get; set; }
        public string eligibilityUri { get; set; }
        public string feesAndPricingUri { get; set; }
        public string bundleUri { get; set; }
    }

    public class CardArt
    {
        public string title { get; set; }
        public string imageUri { get; set; }
    }

    public class Bundle
    {
        public string name { get; set; }
        public string description { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
        public string[] productIds { get; set; }
    }

    public class Feature
    {
        public string featureType { get; set; }
        public string additionalValue { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
    }

    public class Constraint
    {
        public string constraintType { get; set; }
        public string additionalValue { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
    }

    public class Eligibility
    {
        public string eligibilityType { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
    }

    public class Fee
    {
        public string name { get; set; }
        public string feeType { get; set; }
        public string amount { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
    }

    public class DepositRate
    {
        public string depositRateType { get; set; }
        public string rate { get; set; }
        public string calculationFrequency { get; set; }
        public string applicationFrequency { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
    }

    public class LendingRate
    {
        public string lendingRateType { get; set; }
        public string rate { get; set; }
        public string comparisonRate { get; set; }
        public string calculationFrequency { get; set; }
        public string applicationFrequency { get; set; }
        public string interestPaymentDue { get; set; }
        public string repaymentType { get; set; }
        public string loanPurpose { get; set; }
        public string additionalInfo { get; set; }
        public string additionalInfoUri { get; set; }
    }

    public class Admin
    {
        public Status status { get; set; }
        public Outage[] outages { get; set; }
    }

    public class Status
    {
        public string status { get; set; }
        public string explanation { get; set; }
        public DateTime detectionTime { get; set; }
        public DateTime expectedResolutionTime { get; set; }
        public DateTime updateTime { get; set; }
    }

    public class Outage
    {
        public DateTime outageTime { get; set; }
        public string duration { get; set; }
        public bool isPartial { get; set; }
        public string explanation { get; set; }
    }

    public class Authenticated
    {
        public Customer[] customers { get; set; }
    }

    public class Customer
    {
        public string customerId { get; set; }
        public Customer1 customer { get; set; }
        public Banking1 banking { get; set; }
    }

    public class Customer1
    {
        public string customerUType { get; set; }
        public Person person { get; set; }
    }

    public class Person
    {
        public DateTime lastUpdateTime { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string[] middleNames { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string occupationCode { get; set; }
        public string occupationCodeVersion { get; set; }
        public Phonenumber[] phoneNumbers { get; set; }
        public Emailaddress[] emailAddresses { get; set; }
        public Physicaladdress[] physicalAddresses { get; set; }
    }

    public class Phonenumber
    {
        public bool isPreferred { get; set; }
        public string purpose { get; set; }
        public string countryCode { get; set; }
        public string areaCode { get; set; }
        public string number { get; set; }
        public string extension { get; set; }
        public string fullNumber { get; set; }
    }

    public class Emailaddress
    {
        public bool isPreferred { get; set; }
        public string purpose { get; set; }
        public string address { get; set; }
    }

    public class Physicaladdress
    {
        public string addressUType { get; set; }
        public Simple simple { get; set; }
        public string purpose { get; set; }
    }

    public class Simple
    {
        public string mailingName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string postcode { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }

    public class Banking1
    {
        public Account[] accounts { get; set; }
        public Directdebit[] directDebits { get; set; }
        public Payee[] payees { get; set; }
    }

    public class Account
    {
        public Account1 account { get; set; }
        public Balance balance { get; set; }
        public Transaction[] transactions { get; set; }
    }

    public class Account1
    {
        public string accountId { get; set; }
        public DateTime creationDate { get; set; }
        public string displayName { get; set; }
        public string nickname { get; set; }
        public string openStatus { get; set; }
        public bool isOwned { get; set; }
        public string maskedNumber { get; set; }
        public string productCategory { get; set; }
        public string productName { get; set; }
        public string bsb { get; set; }
        public string accountNumber { get; set; }
        public string bundleName { get; set; }
        public string depositRate { get; set; }
        public Depositrate1[] depositRates { get; set; }
        public Feature1[] features { get; set; }
        public Fee1[] fees { get; set; }
        public Address[] addresses { get; set; }
    }

    public class Depositrate1
    {
        public string depositRateType { get; set; }
        public string rate { get; set; }
        public string calculationFrequency { get; set; }
        public string applicationFrequency { get; set; }
        public string additionalInfo { get; set; }
    }

    public class Feature1
    {
        public string featureType { get; set; }
        public string additionalValue { get; set; }
        public bool isActivated { get; set; }
    }

    public class Fee1
    {
        public string name { get; set; }
        public string feeType { get; set; }
        public string amount { get; set; }
        public string additionalValue { get; set; }
    }

    public class Address
    {
        public string addressUType { get; set; }
        public Simple simple { get; set; }
    }

    public class Balance
    {
        public string accountId { get; set; }
        public string currentBalance { get; set; }
        public string availableBalance { get; set; }
    }

    public class Transaction
    {
        public string transactionId { get; set; }
        public bool isDetailAvailable { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public DateTime postingDateTime { get; set; }
        public string amount { get; set; }
        public string reference { get; set; }
        public string merchantName { get; set; }
        public string merchantCategoryCode { get; set; }
        public string apcaNumber { get; set; }
        public Extendeddata extendedData { get; set; }
    }

    public class Extendeddata
    {
        public string service { get; set; }
        public string payee { get; set; }
        public string extensionUType { get; set; }
        public X2p101payload x2p101Payload { get; set; }
    }

    public class X2p101payload
    {
        public string extendedDescription { get; set; }
        public string endToEndId { get; set; }
    }

    public class Directdebit
    {
        public Authorisedentity authorisedEntity { get; set; }
        public DateTime lastDebitDateTime { get; set; }
        public string lastDebitAmount { get; set; }
    }

    public class Authorisedentity
    {
        public string description { get; set; }
        public string financialInstitution { get; set; }
        public string abn { get; set; }
        public string acn { get; set; }
    }

    public class Payee
    {
        public string payeeId { get; set; }
        public string nickname { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public DateTime creationDate { get; set; }
        public string payeeUType { get; set; }
        public string payeeAccountUType { get; set; }
        public PayeeAccount account { get; set; }
    }

    public class PayeeAccount
    {
        public string accountName { get; set; }
        public string bsb { get; set; }
        public string accountNumber { get; set; }
    }

    public class ClientCache
    {
        public string clientId { get; set; }
        public string legalEntityId { get; set; }
        public string orgId { get; set; }
        public string softwareId { get; set; }
        public string legalEntityName { get; set; }
        public string orgName { get; set; }
        public string clientName { get; set; }
        public string clientDescription { get; set; }
        public string clientUri { get; set; }
        public string[] redirectUris { get; set; }
        public string sectorIdentifierUri { get; set; }
        public string logoUri { get; set; }
        public string tosUri { get; set; }
        public string policyUri { get; set; }
        public string jwksUri { get; set; }
        public string revocationUri { get; set; }
        public string recipientBaseUri { get; set; }
        public string softwareRoles { get; set; }
        public string[] scopes { get; set; }
    }

    public class RegisterCache
    {
        public string legalEntityId { get; set; }
        public string legalEntityName { get; set; }
        public string accreditationNumber { get; set; }
        public string accreditationLevel { get; set; }
        public string logoUri { get; set; }
        public string status { get; set; }
        public DateTime lastUpdated { get; set; }
        public DataRecipientBrand[] dataRecipientBrands { get; set; }
    }

    public class DataRecipientBrand
    {
        public string dataRecipientBrandId { get; set; }
        public string brandName { get; set; }
        public string logoUri { get; set; }
        public string status { get; set; }
        public SoftwareProduct[] softwareProducts { get; set; }
    }

    public class SoftwareProduct
    {
        public string softwareProductId { get; set; }
        public string softwareProductName { get; set; }
        public string softwareProductDescription { get; set; }
        public string logoUri { get; set; }
        public string status { get; set; }
    }
}