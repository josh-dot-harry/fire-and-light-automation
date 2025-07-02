namespace FaL.PageObject.Contract;
public interface IAutomationFactory
{
    IWebDriverProxy GetWebDriverProxy();
    TConfig GetConfiguration<TConfig>() where TConfig : class;
    IAutomationPage CreateAutomationPage<T>() where T : class;
}
