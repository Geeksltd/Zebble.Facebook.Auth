namespace Zebble
{
    public partial class Facebook
    {
        public enum EventNames
        {
            AchieveLevel,
            InAppAdClick,
            AddPaymentInfo,
            AddToCart,
            AddToWishlist,
            CompleteRegistration,
            CompleteTutorial,
            Contact,
            CustomizeProduct,
            Donate,
            FindLocation,
            InitiateCheckout,
            Rate,
            Schedule,
            SpentCredits,
            StartTrial,
            SubmitApplication,
            Subscription,
            UnlockAchievement,
            ViewContent,
            Search
        }

        public enum ParameterNames
        {
            Level,
            AdType,
            Success,
            ContentType,
            Currency,
            ContentId,
            Content,
            RegistrationMethod,
            NumItems,
            PaymentInfoAvailable,
            MaxRatingValue,
            SearchString,
            OrderId,
            Description
        }
    }
}
