using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using XeroAPPFramework;


namespace XeroTest
{
    [TestClass]
    public class XeroUITests
    {
        IWebDriver driver;
        string userName = "domarp6@gmail.com";
        string password = "tem9P@ss";

        [TestInitialize]
        public void testInitialize()
        {
            driver = new FirefoxDriver();
            Logger.folderpath = System.IO.Directory.GetCurrentDirectory();
            Logger.log("Start", "Test Initialize");
        }

        [TestCleanup]
        public void testCleanup()
        {
            Logger.log("END", "Quiting Browser driver");
            driver.Quit();
        }

        //Login to Demo Xero Account, 
        //Create a Demo Organization,
        //navigate to the Sales screen 
        //And then to the Repeating tab
        [TestMethod]
        public void WorkFlowTest()
        {
            #region Variables
                LoginPage loginPage;
                HomePage homePage;
                SetupPage setupPage;
                SalesPage salesPage;
                SettingsPage settingsPage;
                string  demoOrg = "DemoOrg";
            #endregion

            Logger.log("Message", "WorkFLOW TestCase");
            loginPage = new LoginPage(driver);
            loginPage.Login(userName, password);

            homePage = new HomePage(driver);
            homePage.GotoMyXeroHomePage();

            //Cerate a new Organization
            homePage.AddanOrganization(demoOrg, "New Zealand");

            //Start the setup process with filling minimal details.
            setupPage = new SetupPage(driver);
            setupPage.completeSetup(new SetupPage.SetupParams
                {
                    DisplayName = demoOrg,
                    TradingName = demoOrg,
                    AccountsToSelect = new string[] { "Sales", "Cleaning", "Rent", "Loan", "Salaries" }
                });

            //Goto Settings/Inventory page
            homePage.NavigateToPage("Settings/Inventory_Items");

            //Create Inventory Item
            settingsPage = new SettingsPage(driver);
            settingsPage.CreateInventoryItems(new SettingsPage.InventoryItemParam
            {
                itemCode = "dummy",
                forPurchase_UnitPrice = "30",
                forPurchase_Account = "Cleaning"
            });

            //Navigate to Sales page
            homePage.NavigateToPage("Accounts/Sales");
            salesPage = new SalesPage(driver);
            //Create repeating invoice
            salesPage.CreateRepeatingInvoice(new SalesPage.RepeatingInvoiceParam
            {
                repeatTimes = "1",
                repeatEvery = "Month",
                invoiceDate = System.DateTime.Today.AddDays(4).ToString("dd MMM yyyy"),
                dueDate = "7",
                dueDateText = "of the following month",
                invoiceTo = "Dummy_dumbo",
                approval = SalesPage.Approval.Approve,
                itemName = "dummy",
                item_desc = "dummy data",
                item_quantity = "5",
                item_unitPrice = "100",
                item_accountType = "Cleaning"
            });

            //Goto Home page first 
            homePage.GotoMyXeroHomePage();

            //Navigate to Sales page
            homePage.NavigateToPage("Accounts/Sales");

            //Navigate to Repeating Tab under invoices and Verify the page is loaded or not.
            salesPage=new SalesPage(driver);
            Logger.log("Message", "Verifying Whether Reporting page is loaded or not");
            Assert.IsTrue(salesPage.OpenInvoicesTab(SalesPage.InvoicesTab.Repeating, true), "ERROR: Reporting page is not loaded");

        }

        [TestMethod]
        public void Repeating_SearchTests()
        {
            #region Variables
            LoginPage loginPage;
            HomePage homePage;
            SalesPage salesPage;
            string BlahOrg = "Blah Blah";
            #endregion

            Logger.log("Message", "Repeating Search Tests");
            loginPage = new LoginPage(driver);
            loginPage.Login("domarp6@gmail.com", "tem9P@ss");

            homePage = new HomePage(driver);
            homePage.GotoMyXeroHomePage();

            //Open Blah Blah Org 
            homePage.SelectOrganization(BlahOrg);

            //Navigate to Sales page
            homePage.NavigateToPage("Accounts/Sales");

            //Select Repeating tab in Invoices page
            salesPage = new SalesPage(driver);
            salesPage.OpenInvoicesTab(SalesPage.InvoicesTab.Repeating, false);

            //!!!!! Search for Contact Name!!!!!!
            Logger.log("Message", "Searching for Contact Name: 'Brahmi'");
            salesPage.SearchRepeatingInvoices(new SalesPage.SearchRepeatingParam { RefORContactName = "Brahmi" });

            //Verifying how many rows were returned
            Assert.IsTrue(VerifyNoOfRows().Equals(1), "Search results is not accurate");

            //Clear search
            salesPage.ClearSearchRepeatingFields();
            //!!!! Search for Next Invoice date 8th July 2014
            Logger.log("Message", "Searching for Next Invoice date 8th July 2014");
            salesPage.SearchRepeatingInvoices(new SalesPage.SearchRepeatingParam { StartWithin = "Next invoice date", startdate = "8 Jul 2014" });
            
            //Verifying how many rows were returned
            Assert.IsTrue(VerifyNoOfRows().Equals(1), "Search results is not accurate");

            //Clear search
            salesPage.ClearSearchRepeatingFields();
            //!!!! Search for Next Invoice date 8th July 2014
            Logger.log("Message", "Searching for StartWithIn Anyday and EndDate: 28 Jun 2014");
            salesPage.SearchRepeatingInvoices(new SalesPage.SearchRepeatingParam { StartWithin = "Any date", enddate = "28 Jun 2014" });

            //Verifying how many rows were returned
            Assert.IsTrue(VerifyNoOfRows().Equals(3), "Search results is not accurate");

        }        

        //Check summary before running test
        /// <summary>
        /// !!!$$!--NOTE--!!!!$$!!:
        /// Make sure below Repeating Invoices are with "Invoice Will Be" before running this test case
        /// Allu - Approved
        /// Brahmi - Saved as draft
        /// Jack - Saved as draft
        /// bompr - Approved & Sent
        /// Jumbo - Approved
        /// </summary>
        [TestMethod]
        public void Repeating_AcctionsTests()
        {
            #region Variables
            LoginPage loginPage;
            HomePage homePage;
            SalesPage salesPage;
            string BlahOrg = "Blah Blah";
            #endregion

            Logger.log("Message", "Repeating Invoices - Applying Actions Tests");
            loginPage = new LoginPage(driver);
            loginPage.Login(userName, password);

            homePage = new HomePage(driver);
            homePage.GotoMyXeroHomePage();

            //Open Blah Blah Org 
            homePage.SelectOrganization(BlahOrg);

            //Navigate to Sales page
            homePage.NavigateToPage("Accounts/Sales");

            salesPage = new SalesPage(driver);
            if (salesPage.OpenInvoicesTab(SalesPage.InvoicesTab.Repeating, true))
            {
                //!!!TEstings Approve action!!!!
                Logger.log("Message", "Testing Approve Action on Repeating Invoices");
                salesPage.ApplyActionsOnRepeatingInvoice(new string[] { "Brahmi", "Jack" }, SalesPage.Repeating_Actions.Approve);

                //VErify Confirmation message
                VerifyConfirmationMessage(2, SalesPage.Repeating_Actions.Approve);

                //!! Testing Save As Draft action!!!!
                Logger.log("Message", "Testing 'Save as Draft' Action on Repeating Invoices");
                salesPage.ApplyActionsOnRepeatingInvoice(new string[] { "Brahmi", "Jack" }, SalesPage.Repeating_Actions.SaveAsDraft);

                //Verifying confirmation message
                VerifyConfirmationMessage(2, SalesPage.Repeating_Actions.SaveAsDraft);

                //!!!!Testing Approve for sending action!!!
                Logger.log("Message", "Testing 'Approve for sending' Action on Repeating Invoices");
                salesPage.ApplyActionsOnRepeatingInvoice(new string[] { "Jack" }, SalesPage.Repeating_Actions.ApproceForSending, "domarp6@gmail.com");

                //Verifying confirmation message
                VerifyConfirmationMessage(1, SalesPage.Repeating_Actions.ApproceForSending);
            }
            else
            {
                //TODO: Assert.Fail Testings was not done for Repearting Actions. 
            }
        }

        [TestMethod]
        public void Repeating_CreateRItest()
        {
            #region Variables
            LoginPage loginPage;
            HomePage homePage;
            SalesPage salesPage;
            string BlahOrg = "Blah Blah";
            #endregion

            Logger.log("Message", "Test negative senario for Create Repeating Invoices");
            loginPage = new LoginPage(driver);
            loginPage.Login(userName, password);

            homePage = new HomePage(driver);
            homePage.GotoMyXeroHomePage();

            //Open Blah Blah Org 
            homePage.SelectOrganization(BlahOrg);

            //Navigate to Sales page
            homePage.NavigateToPage("Accounts/Sales");
            salesPage = new SalesPage(driver);

            //Creating a Repeating Invoice without AccountType, to get the Error/Warning messge
            Logger.log("Message", "Trying to create RI with out Account Type is filled");
            salesPage.CreateRepeatingInvoice(new SalesPage.RepeatingInvoiceParam
            {
                repeatTimes = "1",
                repeatEvery = "Month",
                invoiceDate = System.DateTime.Today.AddDays(4).ToString("dd MMM yyyy"),
                dueDate = "7",
                dueDateText = "of the following month",
                invoiceTo = "Dummy_dumbo",
                approval = SalesPage.Approval.Approve,
                itemName = "Buy Groceries",
                item_desc = "dummy data",
                item_quantity = "5",
                item_unitPrice = "10",
            }, waitForNotification: false);

            //Verify Error/Warning message
            Logger.log("Message", "Verifying Repeating Invoices warning or Error Message");
            this.VerifyWarningMessage();

        }

        public int VerifyNoOfRows()
        {
            Logger.log("Message", "Verifying no of search rows returned");
            return this.driver.FindElement(By.XPath(".//*[@id='ext-gen48']/tbody")).FindElements(By.TagName("tr")).Count;
        }

        public void VerifyConfirmationMessage(int nooftransactions, SalesPage.Repeating_Actions action)
        {
            string confirmationMessage = "";
            string formulti = "";
            XeroPage xeroPage = new XeroPage(driver);

            Logger.log("Message", "Verifying Conformation Message for action applied :" +action.ToString());

            switch (action)
            {
                case SalesPage.Repeating_Actions.SaveAsDraft:
                    {
                        if (nooftransactions > 1) formulti = "s";
                        confirmationMessage = string.Format("{0} repeating transaction{1} saved as draft", nooftransactions, formulti);
                    }
                    break;
                case SalesPage.Repeating_Actions.Approve:
                    {
                        if (nooftransactions > 1) formulti = "s";
                        confirmationMessage = string.Format("{0} repeating transaction{1} approved", nooftransactions, formulti);
                    }
                    break;
                case SalesPage.Repeating_Actions.ApproceForSending:
                    {
                        if (nooftransactions > 1) formulti = "s";
                        confirmationMessage = string.Format("{0} invoice{1} has been approved for sending", nooftransactions, formulti);
                    }
                    break;
            }

            if (action != SalesPage.Repeating_Actions.ApproceForSending)
            {
              
                xeroPage.WaitForControlExists(By.XPath(".//*[@id='notify01']"));
                var realMessage = this.driver.FindElement(By.XPath(".//*[@id='ext-gen117']")).Text;
                Logger.log("Message", "Verifying Confirmation message. Real Message: "+realMessage);
                Assert.IsTrue(realMessage.Equals(confirmationMessage), "Confirmation message is not as Expected and showing as: " + realMessage);
            }
            else
            {
                xeroPage.WaitForControlExists(By.XPath(".//*[@id='marked']"));
                var realMessage = this.driver.FindElement(By.XPath(".//*[@id='ext-gen117']")).Text;
                Logger.log("Message", "Verifying Confirmation Message. RealMessage: "+realMessage);
                Assert.IsTrue(realMessage.Equals(confirmationMessage), "Confirmation message is not as Expected and showing as: " + realMessage);
            }
        }

        public void VerifyWarningMessage()
        {
            //wait for warning message
            XeroPage xeroPage = new XeroPage(driver);
            Logger.log("Message", "Waiting for Warning Message");
            xeroPage.WaitForControlExists(By.XPath(".//*[@id='notify01']"), 20);

            //Verify message heading
            Assert.IsTrue(this.driver.FindElement(By.XPath(".//*[@id='ext-gen47']")).Text.Equals("2 errors occurred for the following reasons:"), "Error Message heading is not appropriate");

            //Verify the Reasons
            Assert.IsTrue(this.driver.FindElement(By.XPath(".//*[@id='notify01']/div/ul/li[1]")).Text.Equals("Account must be valid."), "Reason 1 is not as Expected");
            Assert.IsTrue(this.driver.FindElement(By.XPath(".//*[@id='notify01']/div/ul/li[2]")).Text.Equals("Tax rate must be valid."), "Reason 2 is not as Expected");
        }

    }
}
