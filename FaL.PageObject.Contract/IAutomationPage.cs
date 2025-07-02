namespace FaL.PageObject.Contract;
public interface IAutomationPage
{
    void Load();
    void NavigateTo(string location);
    IAutomationPage NavigateToPageObject(string location);
    void SetAuthenticationToken(string token);
    IAutomationPage Current { get; }
    string GetPageHeader();
    void AwaitPageHeading(string heading);
    void Dispose();
}
