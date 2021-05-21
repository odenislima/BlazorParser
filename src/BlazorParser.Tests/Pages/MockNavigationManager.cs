using Microsoft.AspNetCore.Components;

namespace BlazorParser.Tests.Pages
{
    public class MockNavigationManager: NavigationManager
    {
        public MockNavigationManager()
        {
            this.Initialize("https://test.com/", "https://test.com/testlink/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            Uri = uri;
        }
    }
}
