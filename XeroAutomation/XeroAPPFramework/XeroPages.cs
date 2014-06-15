using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace XeroAPPFramework
{
    public class XeroPage
    {
        public IWebDriver driver;       
        
        public XeroPage(IWebDriver driver)
        {
            this.driver = driver;            
        }

        /// <summary>
        /// Generic method which Sets Text box vaule using sendkeys
        /// </summary>
        /// <param name="locator">identifier for TextBox</param>
        /// <param name="newValue">Value to be filled</param>
        /// <param name="clearFirst">Option to Clear the TextBox before filling. ByDefault this is set to False</param>
        /// <param name="sendTabKey">Option to Send TabKey on TextBox after filling. ByDefault this is set to False</param>
        protected void SetTextBoxValue(By locator, string newValue, bool clearFirst = false, bool sendTabKey = false)
        {            
            IWebElement textbox = driver.FindElement(locator);
            Logger.log("Message", "Setting webelement wtih value: "+newValue);

            if (clearFirst)
                textbox.Clear();
            
            textbox.SendKeys(newValue);
            
            if (sendTabKey)
                textbox.SendKeys(Keys.Tab);
        }
               
        /// <summary>
        /// Generic method which Gets Value/Text from TextBox
        /// </summary>
        /// <param name="locator">identifier to find TextBox</param>
        /// <returns>IF TextBox is enabled returns Vaule, Else returns Null</returns>
        protected string GetTextBoxValue(By locator)
        {
            IWebElement textbox = driver.FindElement(locator);
            
            if (textbox.Enabled)
                return textbox.Text;
            else
                return null;
        }

        /// <summary>
        /// Generic method to set a Check Box Vaule
        /// </summary>
        /// <param name="locator">identifier to find CheckBox</param>
        /// <param name="newValue">Boolean value to set for CheckBox</param>
        protected void SetCheckBoxValue(By locator, bool newValue)
        {
            // Find the checkbox element by its locator
            IWebElement checkbox = driver.FindElement(locator);
            Logger.log("Message", "Setting CheckBox value to: "+newValue.ToString());

            //Sets only if the new value is different from existing vaule.
            if (newValue == true)
            {
                if (checkbox.Selected == false) checkbox.Click();
            }

            //Sets only if the new value is different from existing vaule.
            if (newValue == false)
            {
                if (checkbox.Selected == true) checkbox.Click();
            }
        }

        /// <summary>
        /// GEneric method to set SelectValue/Dropdown value
        /// </summary>
        /// <param name="locator">identifier to find SelectValue object</param>
        /// <param name="newValue">new value to set in selectvalue object</param>
        protected void SetSelectValue(By locator, string newValue)
        {
            IWebElement select = driver.FindElement(locator);

            //Find SelectValue Object and set its value.
            var selectElement = new SelectElement(select);
            selectElement.SelectByText(newValue);
        }

        /// <summary>
        /// Waits for a specific control to Exists.
        /// </summary>
        /// <param name="locator">identifier to locate the object</param>
        /// <param name="timeoutInSeconds">timeout to wait for object. Default timeout is 30s</param>
        /// <returns>If element is not found Exception will be thrown.</returns>
        public void WaitForControlExists(By locator, int timeoutInSeconds = 30)
        {
            Logger.log("Message", "Wait for Control Exists, Find control with locator:"+locator.ToString());
            try
            {
                WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(ExpectedConditions.ElementIsVisible(locator));
            }
            //Incase any exception thrown during finding the element, throws an Exception.
            catch (OpenQA.Selenium.WebDriverTimeoutException e)
            {
                Logger.log("Warning", "Unable to wait for control exists and Visible. EXCEPTION::"+e.InnerException.ToString());
                throw e.InnerException;
            }            
        }                

        /// <summary>
        /// Checks whether a Control is exists or not.
        /// </summary>
        /// <param name="locator">identifier to locate the object</param>
        /// <param name="timeoutInSeconds">timeout to wait for Object. Default timeout is 30s</param>
        /// <returns>if element is exists, returns Element. Elese returns Null</returns>
        public IWebElement IsControlExists(By locator, int timeoutInSeconds = 30)
        {
            Logger.log("Message", "Checking Is Control Exists");
            IWebElement element;
            try
            {
                WebDriverWait wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(timeoutInSeconds));
                element= wait.Until(ExpectedConditions.ElementIsVisible(locator));
            }
            catch (OpenQA.Selenium.WebDriverTimeoutException e)
            {
                Logger.log("Warning", "Control is not Existing. EXCEPTION: " + e.InnerException.ToString());
                return null;
            }
            return element;
        }

        /// <summary>
        /// Generic Click Contorl method. Waits for the element to be found before clicking.
        /// </summary>
        /// <param name="locator">identifier to find the object</param>
        /// <param name="timeoutInSeconds">time out to wait for Element</param>
        protected void ClickControl(By locator, int timeoutInSeconds = 30)
        {
            Logger.log("Message", "Clicking webelement");
            this.SelectControl(locator, timeoutInSeconds).Click();
        }

        protected IWebElement SelectControl(By locator, int timeoutInSeconds = 30)
        {
            return this.IsControlExists(locator, timeoutInSeconds);
        }
                
    }

    /// <summary>
    /// A class represting the functionalities of Xero Login Page
    /// </summary>
    public class LoginPage : XeroPage
    {
        public LoginPage(IWebDriver driver): base (driver)
        {
            driver.Manage().Cookies.DeleteAllCookies();//delete cookies
            driver.Navigate().GoToUrl("http://login.xero.com");
            driver.Manage().Window.Maximize();
            Logger.log("Message", "Waiting for Login Page to Load");
            //Waits for LoginPage to Load. Verifies by SubmitButton
            this.WaitForControlExists(By.Id("submitButton"));

        }

        /// <summary>
        /// Email field. It can get or set email field's text.
        /// </summary>
        public string email
        {
            get { return this.GetTextBoxValue(By.Id("email")); }
            set { this.SetTextBoxValue(By.Id("email"),value); }
        }

        /// <summary>
        /// Password field. It can get or set password field's text.
        /// </summary>
        public string password
        {
            get { return this.GetTextBoxValue(By.Id("password")); }
            set { this.SetTextBoxValue(By.Id("password"), value); }
        }

        /// <summary>
        /// Xero Page's Login method. 
        /// </summary>
        /// <param name="email">email address to login</param>
        /// <param name="password">password to login</param>
        /// <returns>If error message shown returns False, Else returns true</returns>
        public bool Login(string email, string password)
        {
            this.email = email;
            this.password = password;
            this.ClickControl(By.Id("submitButton")); 
           
            //verify any error/notification
            IWebElement errorMessge = this.IsControlExists(By.XPath(".//*[@id='contentTop']/div[2]"), 10);
            if (errorMessge != null)
            {
                Logger.log("Warning", "Invalid Login details");
                //this.driver.FindElement(By.XPath(".//*[@id='contentTop']/div[2]/ul/li")).Text;

                return false;
            }
            else
            {
                Logger.log("Message", "Successfully Logged in");
                return true;
            }
        }        
    }

    /// <summary>
    /// A class representing functionalities of Xero Home Page
    /// </summary>
    public class HomePage : XeroPage
    {
        public HomePage(IWebDriver driver): base(driver)
        {
            Logger.log("Message", "Waiting for Home Page to Load");
            if(driver.Url == "https://my.xero.com/!xkcD/Dashboard")
                this.WaitForControlExists(By.ClassName("delete"), 60);
            else if(driver.Url == "https://go.xero.com/Dashboard/")
                this.WaitForControlExists(By.Id("ext-gen26"), 10);

        }
        
        /// <summary>
        /// Add an Organization creates a new organization under trail
        /// </summary>
        /// <param name="orgName">Name of the new Organization</param>
        /// <param name="orgLocation">Location or where the new organization will pay tax</param>
        public void AddanOrganization(string orgName, string orgLocation)
        {
            //Clicking on AddanOrganization button
            Logger.log("Message", "Clicking Add an Organization button");
            this.ClickControl(By.Id("ext-gen1034"));

            //waiting for page to load with fields
            Logger.log("Message", "Waiting for AddOrganization page to load");
            this.WaitForControlExists(By.Id("Name"));

            //Set Organization name
            this.SetTextBoxValue(By.Id("Name"), orgName);
            
            //set Organization country  
            this.SetTextBoxValue(By.Id("Country_value"), orgLocation, true, true);
                                   
            //click start trail button
            Logger.log("Message", "Clicking on Start trail button");
            this.ClickControl(By.Id("ext-gen6"));          
            
        }

        /// <summary>
        /// Select an Organization to Load from the list of Organizations under User
        /// </summary>
        /// <param name="orgName">Organization name to select</param>
        public void SelectOrganization(string orgName)
        {
            var organizationGrid = driver.FindElement(By.Id("grid1"));
            Logger.log("Message", "Selecting an Organization :"+orgName);
            var orgToSelect = organizationGrid.FindElement(By.XPath(".//*[@id='gridview-1033-body']/tr/td/div/div/a[text()='" + orgName + "']"));
            if (orgToSelect.Enabled)
                orgToSelect.Click();
            else
            {
                Logger.log("Warning", "Unable to find the Organization");
            }
            
        }

        /// <summary>
        /// Deletes an organization from the list of organization under User.
        /// </summary>
        /// <param name="orgName">Organization name to delete</param>
        public void DeleteOrganization(string orgName)
        {
            Logger.log("Message", "Deleting an Organization: "+orgName);
            var organizationGrid = driver.FindElement(By.Id("grid1"));

            var orgRowToDelete = organizationGrid.FindElement(By.XPath(".//*[@id='gridview-1033-body']/tr[./td/div/div/a/text()='" + orgName + "']"));

            if (orgRowToDelete.Enabled)
            {
                orgRowToDelete.FindElement(By.ClassName("delete")).Click();
                Logger.log("Message", "Clicked on Delete button ");
                //TODO: Handle Propmpt Message and give a reason for deletion
            }
            else
            {
                Logger.log("Warning", "Unable to find the Organization to delete");
            }
        }

        /// <summary>
        /// Navigate to sepecific Page.
        /// </summary>
        /// <param name="path">Path to the Page. use '/' as delimeter</param>
        /// <example> 
        /// 1. this.NavigateToPage("Accounts/Sales"); to access Sales page under Accounts menu
        /// 2. this.NavigateToPage("Reports/Aged Payables"); to access Aged Payables page under Reports menu
        /// </example>
        public void NavigateToPage(string path)
        {
            Logger.log("Message", "Navigate to Page: "+path);
            //Split the path using delimeter.
            string[] splitPath = path.Split('/');

            if (splitPath.Length > 1)
            {
                //First click on the menu Name and then click on required menu option.
                this.ClickControl(By.XPath(".//*[@id='" + splitPath[0] + "']"));
                //using Mouse hover for workarround.
                var dashboardElement = this.IsControlExists(By.XPath(".//*[@id='Dashboard']"));
                var element = this.IsControlExists(By.XPath(".//*[@id='" + splitPath[0] + "']"));
                Actions action = new Actions(driver);
                action.MoveToElement(dashboardElement).Perform();
                System.Threading.Thread.Sleep(1000);

                action.MoveToElement(element).Perform();
                System.Threading.Thread.Sleep(2000);
                this.ClickControl(By.XPath(".//*[@id='xero-nav']/div[2]/div[2]/div/ul/li[./a/text() = '" + splitPath[0] + "']/ul/li/a[text() = '" + splitPath[1].Replace('_', ' ') + "']"));
            }
            else if (splitPath.Length <= 1)
            {
                this.ClickControl(By.XPath(".//*[@id='" + splitPath[0] + "']"));
            }
        }

        /// <summary>
        /// Takes to MyXero Homepage.
        /// </summary>
        public void GotoMyXeroHomePage()
        {
            Logger.log("Message", "Going TO MY Xero Home Page");
            //Using Workarround to load the My Xero Page
            driver.Navigate().GoToUrl("https://my.xero.com/!xkcD/Dashboard");
            //waiting for Grid to Load.
            Logger.log("Message", "Waiting for MyXero Home page to load"); 
            this.WaitForControlExists(By.ClassName("delete"), 60);
        }
    }

    /// <summary>
    /// A Class representing functionalities of Xero's Organization Setup Page
    /// </summary>
    public class SetupPage : XeroPage
    {
        public SetupPage(IWebDriver driver)
            : base(driver)
        {
            //waiting for "Click the next button message" to appear. 
            Logger.log("Message", "Waiting for Setup Page to Load");
            this.WaitForControlExists(By.XPath(".//*[@id='frmMain']/p"), 80);
        }

        /// <summary>
        /// Parameters required to setup a new Organization
        /// NOTE: handled only basic/minimum fields for creating an Organization.
        /// </summary>
        public class SetupParams
        {
            public string DisplayName {get; set;}
            public string TradingName {get;set;}
            public string[] AccountsToSelect { get; set; }
        }

        /// <summary>
        /// A method to complete setup of a new Organization
        /// </summary>
        /// <param name="parameters">Pass required parameters to setup an Organization</param>
        public void completeSetup(SetupParams parameters)
        {
            Logger.log("Message", "Starting Organization Setup");
            string btnXpath = ".//*[@id='frmMain']/div[3]/div/div[2]/span/button";
            var btnNext = driver.FindElement(By.XPath(btnXpath));

            Logger.log("Message", "Clicking on Next button of First page");                           
            //Click next on the first tab
            btnNext.Click();
            
            //Enter organization details
            Logger.log("Message", "Entering Organization details");
            this.SetTextBoxValue(By.Id("Name"), parameters.DisplayName, true);
            this.SetTextBoxValue(By.Id("LegalName"), parameters.TradingName, true);

            //click next on organization settings tab
            Logger.log("Message", "Click Next on Organization Settings Tab");
            btnNext = driver.FindElement(By.XPath(btnXpath));
            btnNext.Click();

            //wait for GST registered? popup and click on "No" button
            Logger.log("Message", "Wait for GST Registered POPup and Click No button");
            this.WaitForControlExists(By.Id("financialWindowContentpopover"));
            this.ClickControl(By.XPath(".//*[@id='financialWindowContentpopover']/div[2]/div/div/div/div/div/div[1]/span/button"));

            //Click next on Financial settings tab
            Logger.log("Message", "Click next on Financial settings Tab");
            btnNext = driver.FindElement(By.XPath(btnXpath));
            btnNext.Click();

            //wait for Invoice settings? popup and click on "No, Skip this step" button
            Logger.log("Message", "Waiting for Invoice settings Popup and Click No, Skip this setup");
            this.WaitForControlExists(By.Id("invoicesWindowContentpopover"));
            this.ClickControl(By.XPath(".//*[@id='ext-gen41']"));

            //Wait for Invite Users? popup and click on "No, Skip this step" button
            Logger.log("Message", "Waiting for Invite Users popup and Click No button");
            this.WaitForControlExists(By.Id("questionpopover"), 60);
            this.ClickControl(By.XPath(".//*[@id='ext-gen37']"));

            //wait for add currency and click Next on Currencies tab
            Logger.log("Message", "Waiting for Add Currencies and Click on Next button");
            this.WaitForControlExists(By.Id("ext-gen15"));
            btnNext = driver.FindElement(By.XPath(".//*[@id='ext-gen17']/span"));
            btnNext.Click();

            //wait for ChartofAccounts options and Click next on this tab
            Logger.log("Message", "Wait for ChartOfAccounts Options and Click Next on this Tab");
            this.WaitForControlExists(By.XPath(".//*[@id='frmMain']/div[2]/fieldset"));
            btnNext = driver.FindElement(By.XPath(btnXpath));
            btnNext.Click();

            //wait for AddAccount button in Chart of accounts tab
            Logger.log("Message", "Waiting for AddAccount button in ChartOfAccounts Tab");
            this.WaitForControlExists(By.Id("ext-gen18"));

            //select all required accounts in the table.
            Logger.log("Message", "Select required accounts from the table");
            foreach (string account in parameters.AccountsToSelect)
            {
                var row = driver.FindElement(By.XPath(".//*[@id='chartOfAccounts']/tbody/tr[./td/a/ text()='"+account+"']"));
                row.FindElement(By.Id("WillDelete")).Click();
            }

            //Click Next on ChartofAccounts and wait for NoBankAccountsAdded then Click "Yes, Continue anyway"
            Logger.log("Message", "Click Next and Wait for NoBankAccount Added popup.");
            btnNext = driver.FindElement(By.XPath(".//*[@id='frmMain']/div[4]/div/div[2]/span/button"));
            btnNext.Click();
            this.WaitForControlExists(By.Id("noBankAccountsAddedpopover"));
            this.ClickControl(By.XPath(".//*[@id='noBankAccountsAddedpopover']/div[2]/div/div/div/div/div/div[2]/span/button"));

            //wait for conversion date div and then Click Next
            Logger.log("Message", "Waiting for Conversion date and then Click Next");
            this.WaitForControlExists(By.Id("content"));
            btnNext = driver.FindElement(By.XPath(btnXpath));
            btnNext.Click();

            //wait for Account balances div and then click next
            Logger.log("Message", "Waiting for Account Balances and then Click next");
            this.WaitForControlExists(By.Id("addNewLineItemButton"));
            
            btnNext = driver.FindElement(By.XPath(btnXpath));
            btnNext.Click();

            //wait for setup complete text and Click Finish button in Done Tab
            Logger.log("Message", "Waiting for SetUp Complete message and then Click on Finish button");
            this.WaitForControlExists(By.XPath(".//*[@id='frmMain']/div[1]/h1[text()='Set up Complete!']"));
            btnNext = driver.FindElement(By.XPath(btnXpath));
            btnNext.Click();

            //wait for creation process to complete. wait for Dashboard tab to appear
            Logger.log("Message", "Waiting for Dashboard to Load");
            this.WaitForControlExists(By.Id("Dashboard"));
            
        }
    }

    public class SalesPage : XeroPage
    {
        public SalesPage(IWebDriver driver)
            : base(driver)
        {
            //waiting for Money coming in Section to load.
            Logger.log("Message", "Waiting for Sales Page to load");
            this.WaitForControlExists(By.XPath(".//*[@id='ext-comp-1022']"));
        }
        public enum InvoicesTab { Paid, Repeating, See_all};

        /// <summary>
        /// Clicks on one of the Invoices Tabs to Open
        /// </summary>
        /// <param name="invoicesTabName">TabName to click. Available Options: Paid, Repeating, See all</param>
        public bool OpenInvoicesTab(InvoicesTab invoicesTabName, bool verify)
        {
            string tabName = invoicesTabName.ToString().Replace("_", " ");

            this.ClickControl(By.XPath(".//*[@id='ext-gen1018']/div[4]/div/div/h2/div/a[text()='" + tabName + "']"));
            Logger.log("Message", "Clicked on Invoices tab:" +tabName);

            //Verification Part
            if (verify)
            {
                Logger.log("Message", "Verifying Whether tab page is selected or not");
                IWebElement selectedTab = this.IsControlExists(By.XPath(".//*[@id='frmMain']/div/div/ul/li[@class='selected']/a"));
                if (selectedTab != null && selectedTab.Text == tabName)
                {
                    Logger.log("Message", "Successfully Selected choosed tab:"+tabName);
                    return true;
                }
                else
                {
                    Logger.log("Warning", "Failed to select Choosed tab: "+tabName);
                    return false;
                }
            }
            else
                return true;
        }

        public enum Approval { Save_AS_Draft, Approve, Approve_And_Send } 
        public enum Repeating_Actions { SaveAsDraft, Approve, ApproceForSending, Delete }

        public class RepeatingInvoiceParam
        {
            public string repeatTimes { get; set; }
            public string repeatEvery { get; set; }
            /// <summary>
            /// Invoice Date format (20 Jun 2014)
            /// </summary>
            public string invoiceDate { get; set; }
            /// <summary>
            /// dur date format (1-31)
            /// </summary>
            public string dueDate { get; set; }
            public string dueDateText { get; set; }
            public Approval approval { get; set; }
            public string invoiceTo { get; set; }
            public string itemName { get; set; }
            public string item_desc { get; set; }
            public string item_quantity { get; set; }
            public string item_unitPrice { get; set; }
            public string item_accountType { get; set; }
        }
        public class SearchRepeatingParam
        {
            public string RefORContactName { get; set; }
            public string StartWithin { get; set; }
            public string startdate { get; set; }
            public string enddate { get; set; }
        }

        /// <summary>
        /// Create a Repeating Invoice
        /// NOTE: Only Item per Invoice can be added. Not Implemented For Multiple items in One Invoice 
        /// </summary>
        /// <param name="param">Parameters for creating a repeating invoice. NOTE: all item related data and approval  are mandatory to provide.</param>
        public bool CreateRepeatingInvoice(RepeatingInvoiceParam param, bool waitForNotification = true)
        {
            Logger.log("Message", "Creating Repeating Invoice");
            //First click on +New dropdown button and then Click "Repeating Invoice"
            IWebElement btnNewExt = this.driver.FindElement(By.XPath(".//*[@id='ext-gen1037']/span"));
            btnNewExt.Click();
            this.ClickControl(By.XPath(".//*[@id='ext-gen1036']/li[2]/a"));

            Logger.log("Message", "Filling relevant data for creating Repeating Invoice");
            //wait for newreporting invoice to load
            if (this.IsControlExists(By.XPath(".//*[@id='optionsSchedule']")).Displayed)
            {
                if (!string.IsNullOrEmpty(param.repeatTimes))
                    this.SetTextBoxValue(By.XPath(".//*[@id='PeriodUnit']"),param.repeatTimes, true);

                if (!string.IsNullOrEmpty(param.repeatEvery))
                    this.SetTextBoxValue(By.XPath(".//*[@id='TimeUnit_value']"), param.repeatEvery, true, true);

                if (!string.IsNullOrEmpty(param.invoiceDate))
                    this.SetTextBoxValue(By.XPath(".//*[@id='StartDate']"), param.invoiceDate, false, true);

                if (!string.IsNullOrEmpty(param.dueDate))
                    this.SetTextBoxValue(By.XPath(".//*[@id='DueDateDay']"), param.dueDate);

                if (!string.IsNullOrEmpty(param.dueDateText))
                    this.SetTextBoxValue(By.XPath(".//*[@id='DueDateType_value']"), param.dueDateText, true, true);

                if (param.approval != null)
                {
                    switch (param.approval)
                    {
                        case Approval.Approve:
                            this.ClickControl(By.XPath(".//*[@id='saveAsAutoApproved']"));
                            break;
                        case Approval.Approve_And_Send:
                            this.ClickControl(By.XPath(".//*[@id='saveAsAutoApprovedAndEmail']"));
                            break;
                        case Approval.Save_AS_Draft:
                            this.ClickControl(By.XPath(".//*[@id='saveAsDraft']"));
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(param.invoiceTo))
                    this.SetTextBoxValue(By.XPath(".//*[@id='invoiceForm']/div[1]/div[1]/div/input[2]"), param.invoiceTo);

                //fill ItemName
                this.ClickControl(By.XPath(".//*[@id='ext-gen19']/div[1]/table/tbody/tr/td[2]/div"));
                System.Threading.Thread.Sleep(500);
                this.SetTextBoxValue(By.XPath(".//*[@id='ext-comp-1001']"), param.itemName, false, true);
                System.Threading.Thread.Sleep(500);

                //fill item desc
                this.SetTextBoxValue(By.XPath(".//*[@id='ext-comp-1002']"), param.item_desc,false, true);

                //fill Quantity
                System.Threading.Thread.Sleep(500);
                this.SetTextBoxValue(By.XPath(".//*[@id='ext-comp-1004']"), param.item_quantity,false,true);

                //fill unitprice
                System.Threading.Thread.Sleep(500);
                this.SetTextBoxValue(By.XPath(".//*[@id='ext-comp-1005']"), param.item_unitPrice,false, true);
                this.driver.FindElement(By.XPath(".//*[@id='ext-comp-1006']")).SendKeys(Keys.Tab);

                //fill Account
                System.Threading.Thread.Sleep(500);
                if(!String.IsNullOrEmpty(param.item_accountType))
                    this.SetTextBoxValue(By.XPath(".//*[@id='ext-comp-1011']"), param.item_accountType, false, true);
                
                //Click on Save button
                Logger.log("Message", "Clicking on Save button");
                this.ClickControl(By.XPath(".//*[@id='frmMain']/div[2]/div[2]/div/div[3]/div/span[1]/button"));

                //wait for notification to laod.
                if (waitForNotification)
                {
                    Logger.log("Message", "Waiting for notification to load");
                    this.WaitForControlExists(By.XPath(".//*[@id='notify01']"));
                }
                return true;
            }
            else
            {
                return false;                
            }
        }

        /// <summary>
        /// Search for Repeating Invoices in Repeating tab page
        /// </summary>
        /// <param name="param">parameters for searching repeating Invoices</param>
        /// <returns>True if search is completed successfull and False if Search box is not loaded</returns>
        public bool SearchRepeatingInvoices(SearchRepeatingParam param)
        {
            //Click on Search button and wait for search contactName box to show.
            Logger.log("Message", "Clicking on Search button on Repeating invoices");
            this.ClickControl(By.XPath(".//*[@id='ext-gen47']"), 10);
            if (this.IsControlExists(By.XPath(".//*[@id='sb_txtReference']"), 20) != null)
            {
                Logger.log("Message", "Search box is successfully loaded");

                //Set EnterReference or Contact Name field
                if (!string.IsNullOrEmpty(param.RefORContactName))
                    this.SetTextBoxValue(By.XPath(".//*[@id='sb_txtReference']"), param.RefORContactName);

                //Set start with in text
                if (!string.IsNullOrEmpty(param.StartWithin))
                    this.SetTextBoxValue(By.XPath(".//*[@id='sb_drpWithin_value']"), param.StartWithin, true, true);

                //set startDate 
                if (!string.IsNullOrEmpty(param.startdate))
                    this.SetTextBoxValue(By.XPath(".//*[@id='sb_dteStartDate']"), param.startdate);

                //set end date
                if (!string.IsNullOrEmpty(param.enddate))
                    this.SetTextBoxValue(By.XPath(".//*[@id='sb_dteEndDate']"), param.enddate); 

                //Click on Search button
                Logger.log("Message", "Clicking search submit button");
                this.ClickControl(By.XPath(".//*[@id='sbSubmit_']"));
                                
                return true;
            }
            else
            {
                Logger.log("Warning", "Search box is not showed or loaded in Repeating Invoices page");
                return false;
            }
        }

        /// <summary>
        /// Clear search inputs in Repeating fields tab page
        /// </summary>
        /// <returns>True if successfully Clears search inputs and False if unable to find Clear button</returns>
        public bool ClearSearchRepeatingFields()
        {
            Logger.log("Message", "Clicking Clear button on Repeating invoices Search Box");
            var clearButton = this.IsControlExists(By.XPath(".//*[@id='sbContainer_']/div[5]/label/a"),20);

            if (clearButton != null)
            {
                clearButton.Click();
                System.Threading.Thread.Sleep(1000);

                return true;
            }
            else
            {
                Logger.log("Error", "Unable to Clear the Search in RepeatingInvoices page");
                return false;
            }

        }

        /// <summary>
        /// Closes search repeating Box 
        /// </summary>
        public void CloseSearchRepeatingBox()
        {
            Logger.log("Message", "Clicking on Close search box of Repeating invoices");
            this.IsControlExists(By.XPath(".//*[@id='sbContainer_']/a[1]"), 10).Click();
        }

        /// <summary>
        /// Apply Actions like "Save as Draft", "Approve", "Approve for sending" and "Delete" to repeating invoices.
        /// </summary>
        /// <param name="invoiceNames">Repeating Invoices to select</param>
        /// <param name="action">Type of Action to apply</param>
        /// <param name="approveSendingEmail">If 'Approve for sending' is selected as action, then produce EMail to enter</param>
        public void ApplyActionsOnRepeatingInvoice(string[] invoiceNames, Repeating_Actions action, string approveSendingEmail = null)
        {
            //Select Invoice Names first
            foreach (string invoice in invoiceNames)
            {
                Logger.log("Message", "Selecting Invoices to Apply Action");
                this.ClickControl(By.XPath(".//*[@id='ext-gen48']/tbody/tr/td[1]/input[../../td/a/ text()='" + invoice + "']"));
            }

            //Click on the selected action to apply.
            switch (action)
            {
                case Repeating_Actions.SaveAsDraft:
                    {
                        this.ClickControl(By.XPath(".//*[@id='frmMain']/div[1]/div/div/div/a[./span/text()='Save as Draft']"));
                        this.popupConfirmation(true, true);
                    }
                    break;
                case Repeating_Actions.Approve:
                    {
                        this.ClickControl(By.XPath(".//*[@id='frmMain']/div[1]/div/div/div/a[./span/text()='Approve']"));
                        this.popupConfirmation(true, true);
                    }
                    break;
                case Repeating_Actions.ApproceForSending:
                    {
                        this.ClickControl(By.XPath(".//*[@id='frmMain']/div[1]/div/div/div/a[./span/text()='Approve for Sending']"));

                        //wait for Email Window
                        var emailWnd = this.IsControlExists(By.Id("__emailWindow"));
                        if (emailWnd != null)
                        {
                            emailWnd.FindElement(By.Id("ContactEmail")).Clear();
                            emailWnd.FindElement(By.Id("ContactEmail")).SendKeys(approveSendingEmail);                                                        
                            
                            //then Click on Save button
                            emailWnd.FindElement(By.Id("popupSend")).Click();
                            
                            //wait for notification
                            this.WaitForControlExists(By.XPath(".//*[@id='marked']"));
                        }
                    }
                    break;
                case Repeating_Actions.Delete:
                    {
                        this.ClickControl(By.XPath(".//*[@id='frmMain']/div[1]/div/div/div/a[./span/text()='Delete']"));
                        this.popupConfirmation(true, true);
                    }
                    break;                
            }

            //Wait for PopUp Window to appear
            if (Repeating_Actions.ApproceForSending != action)
            {
                Logger.log("Message", "Waiting for Popup window to appear");
                this.WaitForControlExists(By.XPath(".//*[@id='notify01']"));
            }
        }

        /// <summary>
        /// Handling popup confirmation while applying action on Repeating invoices
        /// </summary>
        /// <param name="ClickOK">True to click ok and false for clicking No</param>
        /// <param name="captureMessage">Capture message and write in Log</param>
        private void popupConfirmation(bool ClickOK, bool captureMessage)
        {
            //wait for popupConfirmation Window
            Logger.log("Message", "Waiting for Popup Confirmation Dialog");
            if (this.IsControlExists(By.Id("__popupSpin")) != null)
            {
                if (captureMessage)
                {
                    string message = this.GetTextBoxValue(By.XPath(".//*[@id='__popupSpin']/div[2]/div[1]/div/div/div/div/p"));
                    Logger.log("Message", "PopUp Conformation message shows as: " + message);
                }

                if (ClickOK)
                {
                    Logger.log("Message", "Clicking on OK Button");
                    this.ClickControl(By.XPath(".//*[@id='create02']/a"));
                }
                else
                {
                    Logger.log("Message", "Clicking on Cancel Button");
                    this.ClickControl(By.XPath(".//*[@id='create01']/a"));
                }
            }

        }
    }

    public class SettingsPage: XeroPage
    {
        public SettingsPage(IWebDriver driver)
            : base(driver)
        {
          
        }

        public class InventoryItemParam
        {
            public string itemCode { get; set; }
            public string forPurchase_UnitPrice { get; set; }
            public string forPurchase_Account { get; set; }
            public string forSales_UnitPrice { get; set; }
            public string forSales_Account { get; set; }
        }

        /// <summary>
        /// Create an Inventory item
        /// </summary>
        /// <param name="param">parameters for creating an Inventory Item.</param>
        public void CreateInventoryItems(InventoryItemParam param)
        {
            Logger.log("Message", "Creating Inventory Items");
            //wait for AddItem and then click
            this.IsControlExists(By.XPath(".//*[@id='addNav']/div[1]/span/a")).Click();

            //wait for Add Inventory Item window
            this.WaitForControlExists(By.XPath(".//*[@id='__addPriceListWindow']"));

            //set itemcode
            this.WaitForControlExists(By.XPath(".//*[@id='Code']"));
            this.SetTextBoxValue(By.XPath(".//*[@id='Code']"), param.itemCode);

            //set forPurchase unit
            if(!string.IsNullOrEmpty(param.forPurchase_UnitPrice))
                this.SetTextBoxValue(By.XPath(".//*[@id='PurchasesUnitPrice']"), param.forPurchase_UnitPrice);

            //set forPurchase Account
            if(!string.IsNullOrEmpty(param.forPurchase_Account))
                this.SetTextBoxValue(By.XPath(".//*[@id='PurchasesAccount_value']"), param.forPurchase_Account, false, true);

            //set forSales Unit
            if (!string.IsNullOrEmpty(param.forSales_UnitPrice))
                this.SetTextBoxValue(By.XPath(".//*[@id='UnitPrice']"), param.forSales_UnitPrice);

            //set forSalse Account
            if (!string.IsNullOrEmpty(param.forSales_Account))
                this.SetTextBoxValue(By.XPath(".//*[@id='Account_value']"), param.forSales_Account, false, true);

            //Click on Save button
            Logger.log("Message", "Clicking on Save button");
            this.ClickControl(By.XPath(".//*[@id='content']/a[1]"));

            //wait for add inventory window to disappear
            Logger.log("Message", "Waiting for Add Inventory window to Disappear");
            this.WaitForControlExists(By.XPath(".//*[@id='wholemsgPriceList']"));

        }
    }
}
