namespace Zebble
{
    using System;

    public partial class Facebook
    {
        public partial class AccessToken
        {
            public string TokenString { get; set; }
            public string[] Permissions { get; set; }
            public string[] DeclinedPermissions { get; set; }
            public string[] ExpiredPermissions { get; set; }
            public string AppId { get; set; }
            public string UserId { get; set; }
            public DateTime ExpirationDate { get; set; }
            public DateTime RefreshDate { get; set; }
            public DateTime DataAccessExpirationDate { get; set; }
        }
    }
}
