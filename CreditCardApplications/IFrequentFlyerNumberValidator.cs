using System;

namespace CreditCardApplications
{
    public interface ILicenseData
    {
        string LicenseKey { get; }
    }

    public interface IServiceInformation
    {
        ILicenseData License { get; }
    }

    public interface IFrequentFlyerNumberValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
        // string LicenseKey { get; }
        IServiceInformation ServiceInformation { get; } // ServiceInformation can get to the LicenseKey property through the IServiceInformation property
        ValidationMode ValidationMode { get; set; }
    }
}