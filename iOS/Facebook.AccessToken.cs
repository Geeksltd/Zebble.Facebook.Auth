namespace Zebble
{
    using Foundation;
    using System;
    using SDK = global::Facebook.CoreKit;

    public partial class Facebook
    {
        public partial class AccessToken
        {
            static readonly DateTime NsRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

            NSDate ToNSDate(DateTime dt)
            {
                var total = (dt - NsRef).TotalSeconds;
                return NSDate.FromTimeIntervalSinceReferenceDate(total);
            }

            internal SDK.AccessToken ToSDKAccessToken()
            {
                return new SDK.AccessToken(TokenString, Permissions, DeclinedPermissions, ExpiredPermissions, AppId, UserId,
                    ToNSDate(ExpirationDate), ToNSDate(RefreshDate), ToNSDate(DataAccessExpirationDate));
            }
        }
    }
}