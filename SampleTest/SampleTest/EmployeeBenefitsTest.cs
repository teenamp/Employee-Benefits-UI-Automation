using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeBenefitsTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        public void Test_Add_Employee_And_Verify_Benefits()
        {
            var rowSet = new HashSet<string>();

            // Browser Driver
            //TODO: Need to fetch the browser type from config file and create the instence of RemoteWebDriver at run time
            var webDriver = new ChromeDriver();
            //Navigate to the site
            //TODO: Need to move this to config file or database
            webDriver.Navigate().GoToUrl("https://wmxrwq14uc.execute-api.us-east-1.amazonaws.com/Prod/Account/Login");

            var username = webDriver.FindElement(By.Id("Username"));
            //TODO: Need to move this to config file or database
            username.SendKeys("TestUser83");

            var password = webDriver.FindElement(By.Id("Password"));
            //TODO: Need to move this to config file or database
            password.SendKeys("!5AR}ySC-ivw");

            var lnklogin = webDriver.FindElement(By.CssSelector(".btn-primary"));

            //Login-operation
            lnklogin.Submit();

            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(500);
            var rowBeforeAdd = webDriver.FindElements(By.XPath("//*[@id='employeesTable']/tbody/tr"));
            var beforeCount = rowBeforeAdd.Count;

            foreach (var row in rowBeforeAdd)
            {
                var tds = row.FindElements(By.TagName("td"));
                //Add ID's to a hash set to find the newly added employee after add button click
                rowSet.Add(tds[0].Text);
            }

            // Add Employee-Assertion
            var addbutton = webDriver.FindElement(By.Id("add"));
            Assert.That(addbutton.Displayed, Is.True);

            addbutton.Click();

            var firstname = webDriver.FindElement(By.Id("firstName"));
            //TODO: Need to move this to config file or database
            firstname.SendKeys("Teena");

            var lastname = webDriver.FindElement(By.Id("lastName"));
            //TODO: Need to move this to config file or database
            lastname.SendKeys("Mathew");

            var dependants = webDriver.FindElement(By.Id("dependants"));
            //TODO: Need to move this to config file or database
            dependants.SendKeys("2");

            var addemployee = webDriver.FindElement(By.XPath("//div//button[@id = 'addEmployee']"));
            addemployee.Click();

            //Assertion
            Thread.Sleep(1000);

            var rowsAfterAdd = webDriver.FindElements(By.XPath("//*[@id='employeesTable']/tbody/tr"));

            var afterCount = rowsAfterAdd.Count;

            if (beforeCount < afterCount)
            {               
                foreach (var row in rowsAfterAdd)
                {
                    var tds = row.FindElements(By.TagName("td"));
                    if (!rowSet.Contains(tds[0].Text))
                    {
                        //var firstName = tds[1].Text;
                        var benefitscost = float.Parse(tds[6].Text);
                        var expectedBenefitsCost = GetBenefitsCost(int.Parse(tds[3].Text));
                        //Verify benifit calculated for the newly added employee
                        Assert.AreEqual(Math.Round(benefitscost), Math.Round(expectedBenefitsCost));
                    }
                }
            } webDriver.Close();
        }

        private double GetBenefitsCost(int numOfdependants)
        {
            var costOfBenefitsPerYear = 1000.00;
            var dependentCostPerYear = 500.00;

            return (costOfBenefitsPerYear + dependentCostPerYear * numOfdependants) / 26.00; ;
        }
    }
}