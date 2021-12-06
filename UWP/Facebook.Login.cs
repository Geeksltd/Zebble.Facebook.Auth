namespace Zebble
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Olive;

    public partial class Facebook
    {
        /// <summary>
        /// It allows you to customize its visual style.
        /// </summary>
        public static FacebookDialog CurrentDialog { get; set; }

        static string GetLoginUrl(string requestedPermissions)
        {
            var returnUrl = ("https://" + "www.facebook.com/connect/login_success.html").UrlEncode();

            return $"https://graph.facebook.com/oauth/authorize" +
                $"?client_id={ApplicationId}" +
                $"&redirect_uri={returnUrl}" +
                $"&scope={requestedPermissions.UrlEncode()}" +
                $"&type=user_agent&display=popup";
        }

        /// <summary>
        /// Opens a facebook login dialog to get the user to sign in. It will then retrieve the user's data.
        /// If login was not successful then it will return null.
        /// Full list of fields: https://developers.facebook.com/docs/graph-api/reference/user
        /// </summary>
        public static async Task<User> Register(params Field[] fields)
        {
            var token = await Login(GetRequiredPermissions(fields).ToString(","));
            if (token.IsEmpty()) return null;

            return await GetUserInfo(token, fields);
        }

        public static Task<string> Login(params Field[] requestedFields)
        {
            return Login(GetRequiredPermissions(requestedFields).ToString(","));
        }

        /// <summary>
        /// Opens a login dialog and returns the access token if it was successful, otherwise null.
        /// </summary> 
        /// <param name="permissions">The permissions to get from the user, so the access token can be used to retrieve such data later on.
        /// See the full list of permissions here: https://developers.facebook.com/docs/facebook-login/permissions/.
        /// Use GetRequiredPermissions(...) to get the list.</param>
        public static async Task<string> Login(string requestedPermissions)
        {
            var task = new TaskCompletionSource<string>();

            var url = GetLoginUrl(requestedPermissions);

            if (CurrentDialog == null) CurrentDialog = new FacebookDialog { IsDefaultDialog = true };

            CurrentDialog.RequestedUrl = url;
            CurrentDialog.Canceled.Handle(() =>
            {
                CurrentDialog.RemoveSelf();
                CurrentDialog = null;
                task.SetResult(null);
            });
            CurrentDialog.Succeeded.Handle(token => task.SetResult(token));

            await CurrentDialog.ShowDialog();

            return await task.Task;
        }

        public static async Task<User> GetUserInfo(string accessToken, Field[] fields)
        {
            var url = "https://" + "graph.facebook.com/me?" +
                "fields=" + fields.Select(x => x.ToString().ToLower()).ToString(",") +
                "&access_token=" + accessToken;

            return await Thread.Pool.Run(async () =>
            {
                using var client = new HttpClient();
                var json = await client.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<JObject>(json);
                return new User(data) { AccessToken = new AccessToken { TokenString = accessToken } };
            });
        }
    }
}
