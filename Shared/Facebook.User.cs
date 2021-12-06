namespace Zebble
{
    using System;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using Olive;

    partial class Facebook
    {
        public class User
        {
            public readonly JObject Data;
            public User(JObject data) { Data = data; }

            public override string ToString() => Data.ToString();

            public string this[string key] => Data.Property(key)?.Value?.ToStringOrEmpty();

            public string this[Field field] => Data.Property(field.ToString().ToLower())?.Value?.ToStringOrEmpty();

            /// <summary>
            ///  The user's Login access token. This can be used for advanced or custom integration with Facebook Api.
            /// </summary>
            public AccessToken AccessToken { get; set; }

            public Range<int> AgeRange
            {
                get
                {
                    var range = Data.Property("age_range");
                    if (range?.Value == null) return new Range<int>(-1, -1);

                    var min = (range.Value as JObject)?.Property("min")?.Value?.ToString().TryParseAs<int>() ?? -1;
                    var max = (range.Value as JObject)?.Property("max")?.Value?.ToString().TryParseAs<int>() ?? min;

                    return new Range<int>(min, max);
                }
            }

            /// <summary>
            /// The id of this person's user account. This ID is unique to each app and cannot be used across different apps.
            /// </summary>
            public string Id => this["id"];

            public string FirstName => this["first_name"];
            public string LastName => this["last_name"];
            public string FullName => new[] { FirstName, LastName }.Trim().ToString(" ");
            public string Email => this["email"];
            public string Location => this["location"];
            public string PictureUrl => $"https://graph.facebook.com/{Id}/picture";

            /// <summary>
            /// The gender selected by this person, 'male' or 'female'. This value will be omitted if the gender is set to a custom value.
            /// </summary>
            public string Gender => this["gender"];

            /// <summary>
            /// The person's birthday as MM/DD/YYYY. However, people can control who can see the year they were born separately from the month and day so this string can be only the year (YYYY) or the month + day (MM/DD)
            /// </summary>
            public string BirthdayInfo => this["birthday"];

            public DateTime? Birthday
            {
                get
                {
                    try
                    {
                        var parts = BirthdayInfo.OrEmpty().Split('/');
                        if (parts.Count() != 2) return null;
                        return new DateTime(parts.Last().To<int>(), parts.First().To<int>(), parts.ElementAt(1).To<int>());
                    }
                    catch { return null; }
                }
            }
        }
    }
}
