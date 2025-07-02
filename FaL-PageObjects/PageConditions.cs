using OpenQA.Selenium;

namespace FaL_PageObjects;
public static class PageConditions
{
    public static Func<IWebDriver, IWebElement?> ElementToBeClickable(By locator)
    {
        return (driver) =>
        {
            try
            {
                var element = driver.FindElement(locator);
                if (element != null && element.Enabled)
                {
                    return element;
                }
                else
                {
                    return null;
                }
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        };
    }

    public static Func<IWebDriver, IWebElement?>ElementToBeClickable(IWebElement element)
    {
        return (driver) =>
        {
            try
            {
                if (element != null && element.Enabled)
                {
                    return element;
                }
                else
                {
                    return null;
                }
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        };
    }

    public static Func<IWebDriver, IWebElement?> ElementIsVisible(By locator)
    {
        return (driver) =>
        {
            try
            {
                var element = driver.FindElement(locator);
                return ElementIfVisible(element);
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        };
    }

    private static IWebElement? ElementIfVisible(IWebElement element)
    {
        if (element != null && element.Displayed)
        {
            return element;
        }
        return null;
    }
}
