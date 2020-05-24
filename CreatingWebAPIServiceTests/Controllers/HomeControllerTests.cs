using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreatingWebAPIService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreatingWebAPIService.Models;

namespace CreatingWebAPIService.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        [TestMethod()]
        public void CheckCountryEnableabilityTest()
        {
            // Check Country Enableability Test
            //RequestModel requestModel = new RequestModel
            //{
            //    SiteApiKey = "1234-5678-9010-3232",
            //    Ip = "24.125.27.4",
            //    WhiteList = new List<Country> { new Country { CountryName = "United States" } }
            //};

            HomeController homeController = new HomeController();
            bool returnValue = false;
            // Act
            try
            {
                // returnValue = homeController.CheckCountryEnableability(requestModel);
                returnValue = homeController.CountryIPLookup("24.125.27.4", new List<Country> { new Country { CountryName = "United States" } });
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                // Assert
                StringAssert.Contains(e.Message, e.Message);
            }

            if(returnValue)
                Assert.AreEqual(true, returnValue);
            else
                Assert.Fail("Unexpected Value Received");

        }
    }
}