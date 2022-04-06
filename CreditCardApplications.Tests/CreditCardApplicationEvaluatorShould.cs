using System;
using Xunit;
using Moq;
using System.Collections.Generic;
using Moq.Protected;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {

        private Mock<IFrequentFlyerNumberValidator> mockValidator;
        private CreditCardApplicationEvaluator sut;

        public CreditCardApplicationEvaluatorShould()
        {
            mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            sut = new CreditCardApplicationEvaluator(mockValidator.Object);
        }

        [Fact]
        public void AcceptHighIncomeApplications()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplication()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.DefaultValue = DefaultValue.Mock;
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);  // Before adding this the program never evaluated application.Age

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            // mockValidator.Setup(x => x.IsValid("x")).Returns(true);  // Returns true only if the value passed in is "x"
            // mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);  // Returns true when any string is passed to it
            // mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);  // Returns true when the value passed in starts with "y"
            // mockValidator.Setup(x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive))).Returns(true); // Returns true when value passed in is within specified range
            // mockValidator.Setup(x => x.IsValid(It.IsIn("z", "y", "x"))).Returns(true);  // Returns true when value passed in is in list of acceptable values
            //mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);  // Returns true if there is a match on the regular expression

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "y"
            };

            var decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplication()
        {
            // Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);    // Adding LicenseKey field broke test
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();  // Using default Loose behavior

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    bool isValid = true;    // This is the value that the mocked system will return in the out
        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));   // The overload version of this method doesn't have a return type so .Returns is not needed

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42
        //    };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        [Fact]
        public void ReferWhenLicenseKeyExpired()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            // mockValidator.Setup(x => x.LicenseKey).Returns("EXPIRED");
            // mockValidator.Setup(x => x.LicenseKey).Returns(GetLicenseExpiryString); // Replacing the literal with a call to the GetLicenseExpiryString function

            //var mockLicenseData = new Mock<ILicenseData>(); // Create a mock version of the license data
            //mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED");    // Set the LicenseKey property of License to return EXPIRED

            //var mockServiceInfo = new Mock<IServiceInformation>();  // Create a mock IServiceInformation (next step in hierarchy)
            //mockServiceInfo.Setup(x => x.License).Returns(mockLicenseData.Object);  // Set the License property to return the mockLicenseData created earlier

            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();  // Create the mock IFrequentFlyerNumberValidator
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object); // Set the ServiceInformation property to return the mockServiceInfo object

            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED");   // Simplification of creating the three mocks above

            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseExpiryString()
        {
            // E.g. read from a vendor-supplied constants file
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookupForOlderApplicants()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            // mockValidator.SetupProperty(x => x.ValidationMode); // Tell Moq to remember the state of the ValidationMode property
            //mockValidator.SetupAllProperties();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);
            
            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once); // Verify that the IsValid mock object is called
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenseKeyForLowIncomeApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.Evaluate(application);

            mockValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);    // Verify that a property is set

            // mockValidator.VerifyNoOtherCalls();
        }

        [Fact]
        public void ReferWhenFrequentFlyerValidationError()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws(new Exception("Custom message"));

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void IncrementLookupCount()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true)
                         .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty); // Raise an event automatically

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { FrequentFlyerNumber = "x", Age = 25 };

            sut.Evaluate(application);

            // mockValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);  // Raise an event manually

            Assert.Equal(1, sut.ValidatorLookupCount);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_ReturnValuesSequence()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            // mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>())) // Allows you to chain together multiple return methods to return different results
                         .Returns(false)
                         .Returns(true);

            //var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision firstDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);

            CreditCardApplicationDecision secondDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_MultipleCallsSequence()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var frequentFlyerNumbersPassed = new List<string>();
            mockValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumbersPassed)));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application1 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.Evaluate(application1);
            sut.Evaluate(application2);
            sut.Evaluate(application3);

            // Assert that IsValid was called three times with "aa", "bb", and "cc"
            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumbersPassed);
        }

        [Fact]
        public void ReferFraudRisk()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            Mock<FraudLookup> mockFraudLookup = new Mock<FraudLookup>();
            //mockFraudLookup.Setup(x => x.IsFraudRisk(It.IsAny<CreditCardApplication>())).Returns(true);   // Unable to use this after changing FraudLookup
            
            mockFraudLookup.Protected().Setup<bool>("CheckApplication", ItExpr.IsAny<CreditCardApplication>()).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object,
                                                         mockFraudLookup.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHumanFraudRisk, decision);
        }

        [Fact]
        public void LinqToMocks()
        {
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            IFrequentFlyerNumberValidator mockValidator = Mock.Of<IFrequentFlyerNumberValidator>
                (
                    validitor =>
                    validitor.ServiceInformation.License.LicenseKey == "OK" &&
                    validitor.IsValid(It.IsAny<string>()) == true
                );
            // Override sut in class field to demonstrate LINQ setup
            var sut = new CreditCardApplicationEvaluator(mockValidator);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }
    }
}
