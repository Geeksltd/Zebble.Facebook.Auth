namespace Zebble
{
    using System.Collections.Generic;
    using System.Linq;
    using Olive;

    partial class Facebook
    {
        // Note: public_profile returns:
        // id, name, first_name, last_name, age_range, link, gender, locale, picture, timezone, updated_time, verified;

        // Full list of permissions: https://developers.facebook.com/docs/facebook-login/permissions/
        static readonly Dictionary<string, string> FieldPermissions = new Dictionary<string, string>
        {
            {"email", "email"},
            {"bio", "user_about_me"},
            {"birthday","user_birthday" }
            // TODO: Add the rest of the mapping here.
        };

        public static string[] GetRequiredPermissions(Field[] fields)
        {
            return GetRequiredPermissions(fields.Select(x => x.ToString().ToLower()).ToArray());
        }

        /// <summary>
        /// Returns the list of login permissions that should be requested to read the full data in the specified fields.
        /// </summary>
        public static string[] GetRequiredPermissions(string[] fields)
        {
            return fields.Select(x => FieldPermissions.GetOrDefault(x).Or("public_profile")).Trim().Distinct().ToArray();
        }

        /// <summary>
        /// For the full list of fields see https://developers.facebook.com/docs/graph-api/reference/user
        /// </summary>
        public enum Field
        {
            First_name,
            Last_name,
            /// <summary>
            /// The person's primary email address listed on their profile. This field will not be returned if no valid email address is available.
            /// </summary>
            Email,
            /// <summary>
            /// The person's birthday as MM/DD/YYYY. However, people can control who can see the year they were born separately from the month and day so this string can be only the year (YYYY) or the month + day (MM/DD)
            /// </summary>
            Birthday,
            /// <summary>
            /// The gender selected by this person, 'male' or 'female'. This value will be omitted if the gender is set to a custom value.
            /// </summary>
            Gender,
            /// <summary>
            /// The person's current location as entered by them on their profile. This field is not related to check-ins
            /// </summary>
            Location,

            /// <summary>
            /// The age segment for this person expressed as a minimum and maximum age. For example, more than 18, less than 21.
            /// </summary>
            Age_range
        }
    }
}
