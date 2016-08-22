using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace ConsoleApplication
{

    public static class Program
    {
        public static IWebDriver driver;
        public static string DefaultUrl = "https://www.packtpub.com/packt/offers/free-learning";
        public static string RegisterBar = "//*[@id=\"account-bar-login-register\"]";
        public static string LoginButton = "//*[@id=\"account-bar-login-register\"]/a[1]/div";
        //        public static string UsernameFieldPath = "//*[@id=\"email\"]";
        public static string UsernameFieldPath =
            "#packt-user-login-form > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > input:nth-child(1)";
        public static string PasswordFieldPath = "#packt-user-login-form > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > input:nth-child(1)";
        public static string Username = "*****";
        public static string Password = "*****";

        static void Main(string[] args)
        {
            try
            {
                var firefox = FirefoxProfile;
                driver = new FirefoxDriver(firefox);
                driver.Manage().Window.Maximize();
                var username = Username;
                var password = Password;

                driver.Navigate().GoToUrl(DefaultUrl);
                Thread.Sleep(5000);
                var js = (IJavaScriptExecutor) driver;
                var script = "var list = document.getElementsByTagName(\"w-div\"); for (index = 0; index < list.length; ++index) {list[index].remove()}";
                js.ExecuteScript(script);
                
                var registerBar = driver.FindElement(By.XPath(RegisterBar));
                if (registerBar.Displayed)
                {
                    LogIn(driver, username, password, LoginButton);
                }

                var claimedBook = ClaimBook();
                Thread.Sleep(60000);
                driver.Quit();
                SendEmail(String.Format("Book {0} was downloaded successfully", claimedBook));
            }
            catch (Exception ex)
            {
                SendEmail(ex.ToString());
                Environment.Exit(156);
                
            }

        }

        private static FirefoxProfile FirefoxProfile
        {
            get
            {
                var firefox = new FirefoxProfile();
                firefox.SetPreference("browser.download.folderList", 2);
                firefox.SetPreference("browser.download.dir", "C:\\Users\\iviliev\\Desktop");
                firefox.SetPreference("browser.helperApps.neverAsk.openFile", "application/pdf");
                firefox.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/pdf");
                firefox.SetPreference("browser.helperApps.alwaysAsk.force", false);
                firefox.SetPreference("pdfjs.disabled", true);
                firefox.SetPreference("plugin.scan.Acrobat", "99.0");
                firefox.SetPreference("plugin.scan.plid.all", false);
                firefox.SetPreference("browser.download.manager.closeWhenDone", true);
                return firefox;
            }
        }

        private static string ClaimBook()
        {
            var claimBook =
                driver.FindElement(
                    By.CssSelector(
                        "#deal-of-the-day > div > div > div.dotd-main-book-summary.float-left > div.dotd-main-book-form.cf > div.float-left.free-ebook"));

            Actions actions = new Actions(driver);
            Thread.Sleep(1000);
            actions.MoveToElement(claimBook).Build().Perform();
            Thread.Sleep(3000);
            actions.MoveToElement(claimBook).Click().Perform();
            

            var lastBook = driver.FindElement(By.XPath("//*[@id=\"product-account-list\"]/div[1]"));
            lastBook.Click();
            var bookName = lastBook.Text.Split('\n').First().TrimEnd();

            var pdfButton =
                driver.FindElement(By.XPath("//*[@id=\"product-account-list\"]/div[1]/div[2]/div[2]/a[1]/div/div[3]"));
            pdfButton.Click();

            return bookName;
        }

        public static void LogIn(IWebDriver webDriver, string username, string password, string loginButtonXpath)
        {
            var loginButton = driver.FindElement(By.XPath(loginButtonXpath));
            loginButton.Click();

            var usernameField = driver.FindElement(By.CssSelector(UsernameFieldPath));
            usernameField.Clear();
            usernameField.SendKeys(username);

            var passField = driver.FindElement(By.CssSelector(PasswordFieldPath));
            passField.Clear();
            passField.SendKeys(password);

            var submit = driver.FindElement(By.CssSelector("#packt-user-login-form > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > input:nth-child(1)"));
            submit.Click();
        }

        public static void SendEmail(string body)
        {
            //AFB Email
            string receiverEmail = "*****";
            var msg = new MailMessage { From = new MailAddress("*****") };
            msg.To.Add(new MailAddress(receiverEmail));
            //Title
            msg.Subject = "Book was downloaded or an exception was thrown";
            //Body
            msg.Body = body;

            SmtpClient client = new SmtpClient();

            client.Send(msg);
        }
    }
}



