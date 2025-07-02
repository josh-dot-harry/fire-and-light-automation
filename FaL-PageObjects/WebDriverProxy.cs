using FaL.PageObject.Contract;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace FaL_PageObjects;

public class WebDriverProxy : IWebDriverProxy
{
    private IWebDriver _driver;
    public IDictionary<string, object> Vars { get; private set; }
    private IJavaScriptExecutor _js;

    public WebDriverProxy()
    {
        _driver = new ChromeDriver();
        _js = (IJavaScriptExecutor)_driver;
        Vars = new Dictionary<string, object>();
    }

    public void LoadPage(string url)
    {
        _driver.Navigate().GoToUrl(url);
        _driver.Manage().Window.Maximize();
    }

    public void LoadPage(string url, int maxRedirects = 10, double timemoutSeconds = 30)
    {
        var startTime = DateTime.Now;
        var visitedUrls = new HashSet<string>();
        var redirectCount = 0;
        string contentUrl = url;
        bool pageLoaded = false;

        try
        {
            _driver.Navigate().GoToUrl(url);

            while (redirectCount < maxRedirects
                && (DateTime.Now - startTime).TotalSeconds < timemoutSeconds
                && !pageLoaded)
            {
                Thread.Sleep(500); // Allow time for the page to load

                string newUrl = _driver.Url;
                if (newUrl == contentUrl)
                {
                    if (IsDomReady())
                    {
                        pageLoaded = true; // Page is loaded and no more redirects
                    }
                    continue;
                }

                //we've detected a redirect
                redirectCount++;

                if (visitedUrls.Contains(contentUrl))
                {
                    throw new InvalidOperationException($"Redirect loop detected at URL: {newUrl} after {redirectCount} redirects");
                }
                visitedUrls.Add(newUrl);
                contentUrl = newUrl;

                if (newUrl.Contains("login.microsoftonline.com") && redirectCount > 3)
                {
                    throw new InvalidOperationException("Possible authentication issue detected: Mutliple redirects to identity provider");
                }
            }

            if (redirectCount >= maxRedirects)
            {
                throw new InvalidOperationException($"Maximum redirect limit of {maxRedirects} reached without loading the page.");
            }

            if ((DateTime.Now - startTime).TotalSeconds >= timemoutSeconds)
            {
                throw new TimeoutException($"Page load timed out after {timemoutSeconds} seconds.");
            }

            //Maximize the window after loading the page
            _driver.Manage().Window.Maximize();
        }
        catch (Exception ex) when (!(ex is InvalidOperationException || ex is TimeoutException))
        {
            throw new InvalidOperationException($"Failed to load page: {ex.Message}", ex);
        }
    }

    public T GetWebDriver<T>() where T : class
    {
        return (T)_driver;
    }

    public void Dispose()
    {
        _driver?.Dispose();
    }

    public T AwaitPageElement<T>(string locator, LocatorType type, double timespan = 10) where T : class
    {
        By by = GetLocatorByType(locator, type);
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(timespan));
        waiter.IgnoreExceptionTypes(typeof(ElementNotInteractableException), typeof(NoSuchElementException));
        var result = waiter.Until(d => d.FindElement(by));
        WaitForDOM();

        return (T)result;
    }

    public T AwaitPageElementValue<T>(string locator, LocatorType type, double timespan = 10) where T : class
    {
        By by = GetLocatorByType(locator, type);
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(timespan));
        waiter.IgnoreExceptionTypes(typeof(ElementNotInteractableException), typeof(NoSuchElementException));
        var result = waiter.Until(d =>
        {
            var el = d.FindElement(by);
            if (el.Text != null && el.Text.Length > 0) return el;
            return null;
        });

        WaitForDOM();

        return (T)result;

    }

    public IEnumerable<T> AwaitMultiplePageElements<T>(string locator, LocatorType type, double timespan = 10) where T : class
    {
        By by = GetLocatorByType(locator, type);
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(timespan));
        waiter.IgnoreExceptionTypes(typeof(ElementNotInteractableException), typeof(NoSuchElementException));
        var result = waiter.Until(d => d.FindElements(by));

        WaitForDOM();

        return (IEnumerable<T>)result;
    }

    public T GetClickableElement<T>(string locator, LocatorType type, double timespan = 10) where T : class
    {
        By by = GetLocatorByType(locator, type);
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(timespan));
        waiter.IgnoreExceptionTypes(typeof(ElementNotInteractableException), typeof(NoSuchElementException));
        var inputElement = waiter.Until(PageConditions.ElementToBeClickable(by));

        WaitForDOM();

        //scroll into view
        _js.ExecuteScript("arguments[0].scrollIntoView(true);", inputElement);

        return (T)inputElement;
    }

    public void RemoveOverlays()
    {
        Actions actions = new Actions(_driver);
        actions.SendKeys(Keys.Escape).Perform();
        _driver.FindElement(By.TagName("body")).Click();
        Thread.Sleep(500);
    }

    public bool IsDomReady()
    {
        var waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var complete = waiter.Until(d => _js.ExecuteScript("return document.readyState").Equals("complete"));
        return complete;
    }

    private void WaitForDOM()
    {
        IsDomReady();
    }

    private static By GetLocatorByType(string locator, LocatorType type)
    {
        By by;
        switch (type)
        {
            case LocatorType.Css:
                by = By.CssSelector(locator);
                break;
            case LocatorType.XPath:
                by = By.XPath(locator);
                break;
            case LocatorType.TagName:
                by = By.TagName(locator);
                break;
            case LocatorType.ClassName:
                by = By.ClassName(locator);
                break;
            default: throw new InvalidOperationException();
        }
        return by;
    }
}


