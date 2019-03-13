﻿using System;
using Toggl.Giskard.Services;
using Toggl.Foundation;
using Toggl.Foundation.Analytics;
using Toggl.Foundation.Diagnostics;
using Toggl.Foundation.Login;
using Toggl.Foundation.MvvmCross;
using Toggl.Foundation.MvvmCross.Services;
using Toggl.Foundation.Services;
using Toggl.Foundation.Shortcuts;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Realm;
using Toggl.PrimeRadiant.Settings;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;
using Android.Content;
using Android.App;

namespace Toggl.Giskard
{
    public sealed class AndroidDependencyContainer : UiDependencyContainer
    {
        private const int numberOfSuggestions = 5;
        private const string clientName = "Giskard";
        private const string remoteConfigDefaultsFileName = "RemoteConfigDefaults";
        private const ApiEnvironment environment =
#if USE_PRODUCTION_API
            ApiEnvironment.Production;
#else
            ApiEnvironment.Staging;
#endif

        private readonly UserAgent userAgent;
        private readonly Lazy<SettingsStorage> settingsStorage;

        public IForkingNavigationService ForkingNavigationService { get; internal set; }

        public AndroidDependencyContainer()
            : base(environment)
        {
            var applicationContext = Application.Context;
            const string clientName = "Giskard";
            var packageInfo = applicationContext.PackageManager.GetPackageInfo(applicationContext.PackageName, 0);
            var version = packageInfo.VersionName;
            var appVersion = Version.Parse(version);

            userAgent = new UserAgent(clientName, version);
            settingsStorage = new Lazy<SettingsStorage>(() => new SettingsStorage(appVersion, KeyValueStorage.Value));
        }

        public static AndroidDependencyContainer Instance { get; set; }

        protected override IAnalyticsService CreateAnalyticsService()
            => new AnalyticsServiceAndroid();

        protected override IApiFactory CreateApiFactory()
            => new ApiFactory(environment, userAgent);

        protected override IBackgroundSyncService CreateBackgroundSyncService()
            => new BackgroundSyncServiceAndroid();

        protected override IBrowserService CreateBrowserService()
            => new BrowserServiceAndroid();

        protected override ICalendarService CreateCalendarService()
            => new CalendarServiceAndroid(PermissionsService.Value);

        protected override ITogglDatabase CreateDatabase()
            => new Database();

        protected override IDialogService CreateDialogService()
            => new DialogServiceAndroid();

        protected override IGoogleService CreateGoogleService()
            => new GoogleServiceAndroid();

        protected override IIntentDonationService CreateIntentDonationService()
            => new NoopIntentDonationServiceAndroid();

        protected override IKeyValueStorage CreateKeyValueStorage()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(clientName, FileCreationMode.Private);
            return new SharedPreferencesStorageAndroid(sharedPreferences);
        }

        protected override ILicenseProvider CreateLicenseProvider()
            => new LicenseProviderAndroid();

        protected override INotificationService CreateNotificationService()
            => new NotificationServiceAndroid();

        protected override IPasswordManagerService CreatePasswordManagerService()
            => new StubPasswordManagerService();

        protected override IPermissionsService CreatePermissionsService()
            => new PermissionsServiceAndroid();

        protected override IPlatformInfo CreatePlatformInfo()
            => new PlatformInfoAndroid();

        protected override IPrivateSharedStorageService CreatePrivateSharedStorageService()
            => new NoopPrivateSharedStorageServiceAndroid();

        protected override IRatingService CreateRatingService()
            => new RatingServiceAndroid(Application.Context);

        protected override IRemoteConfigService CreateRemoteConfigService()
            => new RemoteConfigServiceAndroid();

        protected override ISchedulerProvider CreateSchedulerProvider()
            => new AndroidSchedulerProvider();

        protected override IApplicationShortcutCreator CreateShortcutCreator()
            => new ApplicationShortcutCreator(Application.Context);

        protected override IStopwatchProvider CreateStopwatchProvider()
            => new FirebaseStopwatchProviderAndroid();

        protected override ISuggestionProviderContainer CreateSuggestionProviderContainer()
            => new SuggestionProviderContainer(
                new MostUsedTimeEntrySuggestionProvider(Database.Value, TimeService.Value, numberOfSuggestions)
            );

        protected override IForkingNavigationService CreateNavigationService()
            => ForkingNavigationService;

        protected override ILastTimeUsageStorage CreateLastTimeUsageStorage()
            => settingsStorage.Value;

        protected override IOnboardingStorage CreateOnboardingStorage()
            => settingsStorage.Value;

        protected override IUserPreferences CreateUserPreferences()
            => settingsStorage.Value;

        protected override IAccessRestrictionStorage CreateAccessRestrictionStorage()
            => settingsStorage.Value;
    }
}