using Bunit;
using Xunit;
using BlazorParser.Client.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using BlazorParser.Client.ViewModels;
using BlazorParser.Tests.Pages.Stubs;

namespace BlazorParser.Tests.Pages
{
    public class EventsViewTests : TestContext
    {
        [Fact]
        public void it_should_render_events_view_without_crashing()
        {
            JSInterop.Mode = JSRuntimeMode.Loose;                      

            var item  = new ServiceDescriptor(typeof(IEventsViewModel), new EventsViewModelStub());

            Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            Services.Add(item);

            var sut = RenderComponent<EventsView>();

            Assert.NotNull(sut);
            sut.Find("#inputPanelTitle").MarkupMatches("<h3 id=\"inputPanelTitle\">Choose file</h3>");
            Assert.NotNull(sut.Find("#eventTable"));
            Assert.NotNull(sut.Find("#errorTable"));
        }
    }
}
