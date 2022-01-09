﻿using Bit.App.Effects;
using Bit.App.Models;
using Bit.App.Resources;
using Bit.Core.Abstractions;
using Bit.Core.Utilities;
using Xamarin.Forms;

namespace Bit.App.Pages
{
    public class TabsPage : TabbedPage
    {
        private readonly IMessagingService _messagingService;
        private readonly IKeyConnectorService _keyConnectorService;
        
        private NavigationPage _groupingsPage;
        private NavigationPage _sendGroupingsPage;
        private NavigationPage _generatorPage;

        public TabsPage(AppOptions appOptions = null, PreviousPageInfo previousPage = null)
        {
            _messagingService = ServiceContainer.Resolve<IMessagingService>("messagingService");
            _keyConnectorService = ServiceContainer.Resolve<IKeyConnectorService>("keyConnectorService");

            _groupingsPage = new NavigationPage(new GroupingsPage(true, previousPage: previousPage))
            {
                Title = AppResources.MyVault,
                IconImageSource = "lock.png"
            };
            Children.Add(_groupingsPage);

            _sendGroupingsPage = new NavigationPage(new SendGroupingsPage(true, null, null, appOptions))
            {
                Title = AppResources.Send,
                IconImageSource = "paper_plane.png",
            };
            Children.Add(_sendGroupingsPage);

#if !FEATURE_GENERATORPAGE_OFF
            _generatorPage = new NavigationPage(new Page())
            {
                Title = AppResources.Generator,
                IconImageSource = "refresh.png"
            };
            Children.Add(_generatorPage);
#endif
            var settingsPage = new NavigationPage(new SettingsPage(this))
            {
                Title = AppResources.Settings,
                IconImageSource = "cog.png"
            };
            Children.Add(settingsPage);

            if (Device.RuntimePlatform == Device.Android)
            {
                Effects.Add(new TabBarEffect());

                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetToolbarPlacement(this,
                    Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ToolbarPlacement.Bottom);
                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);
                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSmoothScrollEnabled(this, false);
            }

            if (appOptions?.GeneratorTile ?? false)
            {
                appOptions.GeneratorTile = false;
                ResetToGeneratorPage();
            }
            else if (appOptions?.MyVaultTile ?? false)
            {
                appOptions.MyVaultTile = false;
            }
            else if (appOptions?.CreateSend != null)
            {
                ResetToSendPage();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (await _keyConnectorService.UserNeedsMigration())
            {
                _messagingService.Send("convertAccountToKeyConnector");
            }
        }

        public void ResetToVaultPage()
        {
            CurrentPage = _groupingsPage;
        }

        public void ResetToGeneratorPage()
        {
            CurrentPage = _generatorPage;
        }
        
        public void ResetToSendPage()
        {
            CurrentPage = _sendGroupingsPage;
        }

        protected async override void OnCurrentPageChanged()
        {
            if (CurrentPage is NavigationPage navPage)
            {
                _messagingService.Send("updatedTheme");
                if (navPage.RootPage is GroupingsPage groupingsPage)
                {
                    // Load something?
                }
#if !FEATURE_GENERATORPAGE_OFF
                else if (navPage.RootPage is GeneratorPage genPage)
                {
                    await genPage.InitAsync();
                }
#endif
                else if (navPage.RootPage is SettingsPage settingsPage)
                {
                    await settingsPage.InitAsync();
                }
            }
        }
    }
}
