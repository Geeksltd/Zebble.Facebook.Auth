namespace Zebble
{
    using SDK = global::Facebook.CoreKit;
    using UIKit;

    public partial class Facebook
    {
        public static UIViewController UIViewController;

        static Facebook()
        {
            UIRuntime.OnOpenUrlWithOptions.Handle(args => OpenUrl(args.Item1, args.Item2, args.Item3, args.Item4));
        }

        public static void Initialize()
        {
            SDK.Settings.AppId = ApplicationId;
            SDK.Settings.DisplayName = AppDisplayName;
        }
    }
}