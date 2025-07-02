using FaL.PageObject.Contract;

namespace FaL_PageObjects;

internal class AutomationFactory : IAutomationFactory
{
    private readonly IWebDriverProxy _webDriverProxy;

    public AutomationFactory(IAutomationConfig config)
    {
        Config = config;
        _webDriverProxy = new WebDriverProxy();
    }

    public IAutomationConfig Config { get; }

    public IAutomationPage CreateAutomationPage<T>() where T : class
    {
        var type = typeof(T);
        if (!typeof(T).IsAssignableTo(type)) throw new InvalidOperationException("Invalid page object interface");

        var instance = Activator.CreateInstance(type, this);
        if (instance == null) throw new InvalidOperationException("Invalid page object instance");

        return (IAutomationPage)instance;
    }

    public TConfig GetConfiguration<TConfig>() where TConfig : class
    {
        return (TConfig)Config;
    }

    public IWebDriverProxy GetWebDriverProxy() => _webDriverProxy;
}


