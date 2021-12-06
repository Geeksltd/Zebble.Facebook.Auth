namespace Zebble
{
    using System.Threading.Tasks;
    using Olive;

    public class FacebookDialog : Stack
    {
        internal string RequestedUrl;
        internal bool IsDefaultDialog;

        internal readonly AsyncEvent<string> Succeeded = new AsyncEvent<string>();
        internal readonly AsyncEvent Canceled = new AsyncEvent();

        public readonly Stack Container = new Stack { Id = "FacebookContainer" };

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            if (IsDefaultDialog)
                await InitializeDefaultDialog();

            await InitializeFacebook();

            Container.ZIndex(1000);
            await Add(Container);
        }

        public Task ShowDialog() => View.Root.Add(this);

        public Task CloseDialog()
        {
            this.Visible(value: false);
            return Canceled.Raise();
        }

        async Task InitializeDefaultDialog()
        {
            Container.Width.BindTo(View.Root.Width, x => x * 0.8f);
            Container.Height.BindTo(View.Root.Height, x => x * 0.8f);

            Container.CenterAlign(delayUntilRender: true)
                     .MiddleAlign(delayUntilRender: true)
                     .Background(color: Colors.White)
                     .Border(1, color: "#4267b2", radius: 5);

            await Container.Add(new Button().Text("Cancel").Background(color: Colors.Black).TextColor(Colors.White).On(x => x.Tapped, () => CloseDialog()));
        }

        async Task InitializeFacebook()
        {
            var browser = new WebView(RequestedUrl).Size(100.Percent());

            browser.BrowserNavigated.Handle(() =>
            {
                var uri = browser.Url.AsUri();

                if (uri.AbsolutePath.EndsWith("/login_success.html"))
                {
                    var token = uri.Fragment.OrEmpty().TrimStart("#").Split('&')
                    .FirstOrDefault(x => x.StartsWith("access_token="))?.TrimStart("access_token=");

                    Succeeded.Raise(token);
                    this.Visible(value: false);
                    return Task.CompletedTask;
                }
                else
                    return Task.CompletedTask;
            });

            await Container.Add(browser);
        }
    }
}
