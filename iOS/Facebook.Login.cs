namespace Zebble
{
    using SDK = global::Facebook;
    using Foundation;
    using System.Threading.Tasks;
    using System;
    using Newtonsoft.Json.Linq;
    using UIKit;
    using Olive;

    public partial class Facebook
    {
        static bool IsLoginCall = false;
        static readonly AsyncEvent<JObject> UserInfoFetched = new AsyncEvent<JObject>();
        static SDK.LoginKit.LoginManager LoginManager;
        private static string[] CurrentParameters;

        public static User CurrentUser;
        public static AccessToken CurrentAccessToken;

        public static readonly AsyncEvent OnCancel = new AsyncEvent();
        public static readonly AsyncEvent<string> OnError = new AsyncEvent<string>();
        public static readonly AsyncEvent<User> OnSuccess = new AsyncEvent<User>();

        public static int ProfileImageWidth = 200, ProfileImageHeight = 200;

        public static Task Login(params Field[] requestedFields)
        {
            IsLoginCall = true;
            CurrentParameters = GetRequiredPermissions(requestedFields);
            LoginManager = new SDK.LoginKit.LoginManager();
            LoginManager.LogIn(CurrentParameters, UIViewController,
                new SDK.LoginKit.LoginManagerLoginResultBlockHandler(RequestTokenHandler));

            return Task.CompletedTask;
        }

        public static async Task GetInfo(Action<JObject> onCompleted)
        {
            IsLoginCall = false;

            var accessToken = await RefreshAccessToken();

            var param = NSDictionary.FromObjectAndKey(CurrentParameters.ToString(",").ToNs(), "fields".ToNs());
            var request = new SDK.CoreKit.GraphRequest("me", param, accessToken.TokenString, null, "GET");
            request.Start(new SDK.CoreKit.GraphRequestBlockHandler(GraphCallback));

            UserInfoFetched.ClearHandlers();
            UserInfoFetched.Handle(onCompleted);
        }

        public static Task LogOut()
        {
            LoginManager.LogOut();
            IsLoginCall = false;
            CurrentUser = null;
            CurrentAccessToken = null;
            return Task.CompletedTask;
        }

        public static void FinishedLaunching(UIApplication application, NSDictionary options)
        {
            SDK.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(application, options);
        }

        public static void OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSDictionary options)
        {
            SDK.CoreKit.ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, options);
        }

        static async void RequestTokenHandler(SDK.LoginKit.LoginManagerLoginResult result, NSError error)
        {
            if (error != null)
            {
                Log.For(typeof(Facebook)).Error("An error occurred in Facebook :[" + error + "]");
                await OnError.Raise(error.ToString());
            }
            else if (result?.IsCancelled ?? true)
            {
                await OnCancel.Raise();
            }
            else
            {
                try
                {
                    var er = result.ToString();
                    var accessToken = result.Token;
                    if (accessToken.IsExpired) accessToken = await RefreshAccessToken();

                    CurrentAccessToken = new AccessToken
                    {
                        TokenString = accessToken.TokenString,
                        AppId = accessToken.AppId,
                        UserId = accessToken.UserId
                    };

                    var param = NSDictionary.FromObjectAndKey(CurrentParameters.ToString(",").ToNs(), "fields".ToNs());
                    var request = new SDK.CoreKit.GraphRequest("me", param, accessToken.TokenString, null, "GET");
                    request.Start(new SDK.CoreKit.GraphRequestBlockHandler(GraphCallback));
                }
                catch (Exception e)
                {
                    Log.For(typeof(Facebook)).Error(e);
                    throw;
                }
            }
        }

        static async void GraphCallback(SDK.CoreKit.GraphRequestConnection connection, NSObject result, NSError error)
        {
            if (error == null)
            {
                var data = new JObject();
                foreach (var item in (NSDictionary)result)
                {
                    data.Add(item.Key.ToString(), new JValue(item.Value.ToString()));
                }
                if (IsLoginCall)
                {
                    CurrentUser = new User(data)
                    {
                        AccessToken = CurrentAccessToken
                    };
                    await OnSuccess.Raise(CurrentUser);
                }
                else
                    await UserInfoFetched.Raise(data);
            }
            else
                await OnSuccess.Raise(null);
        }

        static async Task<SDK.CoreKit.AccessToken> RefreshAccessToken()
        {
            var accessToken = SDK.CoreKit.AccessToken.CurrentAccessToken;
            if (accessToken.IsExpired)
            {
                var token = await SDK.CoreKit.AccessToken.RefreshCurrentAccessTokenAsync();
                accessToken = token.Result as SDK.CoreKit.AccessToken;
            }

            return accessToken;
        }
    }
}