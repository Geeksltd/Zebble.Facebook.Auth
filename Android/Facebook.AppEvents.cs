namespace Zebble
{
    using Android.OS;
    using System;
    using System.Collections.Generic;
    using SDK = Xamarin.Facebook;
    using Java.Util;
    using Java.Math;
    using Olive;

    public partial class Facebook
    {
        static SDK.AppEvents.AppEventsLogger Logger = SDK.AppEvents.AppEventsLogger.NewLogger(UIRuntime.CurrentActivity);

        public static void IsLoggingBehaviorEnabled(SDK.LoggingBehavior loggingBehavior) => SDK.FacebookSdk.IsLoggingBehaviorEnabled(loggingBehavior);

        public static void CallEvent(EventNames eventName, Dictionary<ParameterNames, object> @params, double? valueToSum = null)
        {
            var bundle = new Bundle();
            foreach (var param in @params)
            {
                if (param.Key == ParameterNames.NumItems || param.Key == ParameterNames.MaxRatingValue ||
                    param.Key == ParameterNames.PaymentInfoAvailable || param.Key == ParameterNames.Success)
                    bundle.PutInt(param.Key.ToParameterName(), Convert.ToInt32(param.Value));
                else
                    bundle.PutString(param.Key.ToParameterName(), param.Value.ToString());
            }

            Log(eventName.ToEventName(), bundle, valueToSum);
        }

        public static void CallEvent(string eventName, Dictionary<string, string> @params, double? valueToSum = null)
        {
            var bundle = new Bundle();
            foreach (var param in @params) bundle.PutString(param.Key, param.Value);

            Log(eventName, bundle, valueToSum);
        }

        public static void CallEvent(string eventName, Dictionary<string, int> @params, double? valueToSum = null)
        {
            var bundle = new Bundle();
            foreach (var param in @params) bundle.PutInt(param.Key, param.Value);

            Log(eventName, bundle, valueToSum);
        }

        public static void CallLogPurchase(double purchaseAmount, string currency, Dictionary<string, int> @params = null)
        {
            if (@params != null)
            {
                var bundle = new Bundle();
                foreach (var param in @params) bundle.PutInt(param.Key, param.Value);

                Logger.LogPurchase(BigDecimal.ValueOf(purchaseAmount), currency.To<Currency>(), bundle);
            }
            else
                Logger.LogPurchase(BigDecimal.ValueOf(purchaseAmount), currency.To<Currency>());
        }

        static string ToEventName(this EventNames eventName)
        {
            switch (eventName)
            {
                case EventNames.AchieveLevel:
                    return SDK.AppEvents.AppEventsConstants.EventNameAchievedLevel;
                case EventNames.InAppAdClick:
                    return SDK.AppEvents.AppEventsConstants.EventNameAdClick;
                case EventNames.AddPaymentInfo:
                    return SDK.AppEvents.AppEventsConstants.EventNameAddedPaymentInfo;
                case EventNames.AddToCart:
                    return SDK.AppEvents.AppEventsConstants.EventNameAddedToCart;
                case EventNames.AddToWishlist:
                    return SDK.AppEvents.AppEventsConstants.EventNameAddedToWishlist;
                case EventNames.CompleteRegistration:
                    return SDK.AppEvents.AppEventsConstants.EventNameCompletedRegistration;
                case EventNames.CompleteTutorial:
                    return SDK.AppEvents.AppEventsConstants.EventNameCompletedTutorial;
                case EventNames.Contact:
                    return SDK.AppEvents.AppEventsConstants.EventNameContact;
                case EventNames.CustomizeProduct:
                    return SDK.AppEvents.AppEventsConstants.EventNameCustomizeProduct;
                case EventNames.Donate:
                    return SDK.AppEvents.AppEventsConstants.EventNameDonate;
                case EventNames.FindLocation:
                    return SDK.AppEvents.AppEventsConstants.EventNameFindLocation;
                case EventNames.InitiateCheckout:
                    return SDK.AppEvents.AppEventsConstants.EventNameInitiatedCheckout;
                case EventNames.Rate:
                    return SDK.AppEvents.AppEventsConstants.EventNameRated;
                case EventNames.Schedule:
                    return SDK.AppEvents.AppEventsConstants.EventNameSchedule;
                case EventNames.SpentCredits:
                    return SDK.AppEvents.AppEventsConstants.EventNameSpentCredits;
                case EventNames.StartTrial:
                    return SDK.AppEvents.AppEventsConstants.EventNameStartTrial;
                case EventNames.SubmitApplication:
                    return SDK.AppEvents.AppEventsConstants.EventNameSubmitApplication;
                case EventNames.Subscription:
                    return SDK.AppEvents.AppEventsConstants.EventNameSubscribe;
                case EventNames.UnlockAchievement:
                    return SDK.AppEvents.AppEventsConstants.EventNameUnlockedAchievement;
                case EventNames.ViewContent:
                    return SDK.AppEvents.AppEventsConstants.EventNameViewedContent;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventName));
            }
        }

        static string ToParameterName(this ParameterNames paramName)
        {
            switch (paramName)
            {
                case ParameterNames.Level:
                    return SDK.AppEvents.AppEventsConstants.EventParamLevel;
                case ParameterNames.AdType:
                    return SDK.AppEvents.AppEventsConstants.EventParamAdType;
                case ParameterNames.Success:
                    return SDK.AppEvents.AppEventsConstants.EventParamSuccess;
                case ParameterNames.ContentType:
                    return SDK.AppEvents.AppEventsConstants.EventParamContentType;
                case ParameterNames.Currency:
                    return SDK.AppEvents.AppEventsConstants.EventParamCurrency;
                case ParameterNames.ContentId:
                    return SDK.AppEvents.AppEventsConstants.EventParamContentId;
                case ParameterNames.Content:
                    return SDK.AppEvents.AppEventsConstants.EventParamContent;
                case ParameterNames.RegistrationMethod:
                    return SDK.AppEvents.AppEventsConstants.EventParamRegistrationMethod;
                case ParameterNames.NumItems:
                    return SDK.AppEvents.AppEventsConstants.EventParamNumItems;
                case ParameterNames.PaymentInfoAvailable:
                    return SDK.AppEvents.AppEventsConstants.EventParamPaymentInfoAvailable;
                case ParameterNames.MaxRatingValue:
                    return SDK.AppEvents.AppEventsConstants.EventParamMaxRatingValue;
                case ParameterNames.SearchString:
                    return SDK.AppEvents.AppEventsConstants.EventParamSearchString;
                case ParameterNames.OrderId:
                    return SDK.AppEvents.AppEventsConstants.EventParamOrderId;
                case ParameterNames.Description:
                    return SDK.AppEvents.AppEventsConstants.EventParamDescription;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paramName));
            }
        }

        static void Log(string eventName, Bundle bundle, double? valueToSum = null)
        {
            if (valueToSum.HasValue)
                Logger.LogEvent(eventName, valueToSum.Value, bundle);
            else
                Logger.LogEvent(eventName, bundle);
        }
    }
}