namespace Zebble
{
    using SDK = Xamarin.Facebook;

    public partial class Facebook
    {
        static Facebook()
        {
            UIRuntime.OnActivityResult.Handle(data => OnActivityResult(data.Item1, (int)data.Item2, data.Item3));
        }

        public static void Initialize()
        {
            SDK.FacebookSdk.ApplicationId = ApplicationId;
            SDK.FacebookSdk.ApplicationName = AppDisplayName;
        }
    }
}