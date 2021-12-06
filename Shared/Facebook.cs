namespace Zebble
{
    using Olive;

    public static partial class Facebook
    {
        static string ApplicationId => Config.Get("Facebook.App.Id");
        static string AppDisplayName => Config.Get("Facebook.App.DisplayName", "");
    }
}