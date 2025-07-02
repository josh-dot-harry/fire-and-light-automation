namespace FaL.PageObject.Contract;
public interface IWebDriverProxy
{
    void LoadPage(string url);
    void LoadPage(string url, int maxRedirects = 10, double timemoutSeconds = 30);
    T GetWebDriver<T>() where T : class;
    T AwaitPageElement<T>(string locator, LocatorType locatorType, double timespan = 10) where T : class;
    T AwaitPageElementValue<T>(string locator, LocatorType type, double timespan = 10) where T: class;
    IEnumerable<T> AwaitMultiplePageElements<T>(string locator, LocatorType type, double timespan = 10) where T : class;
    T GetClickableElement<T>(string locator, LocatorType type, double timespan = 10) where T : class;
    void RemoveOverlays();
    void Dispose();
    bool IsDomReady();
}
