﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace Samples
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var appBuilder = MauiApp.CreateBuilder();

#if WINDOWS
			Microsoft.Maui.Essentials.Platform.MapServiceToken =
				"RJHqIE53Onrqons5CNOx~FrDr3XhjDTyEXEjng-CRoA~Aj69MhNManYUKxo6QcwZ0wmXBtyva0zwuHB04rFYAPf7qqGJ5cHb03RCDw1jIW8l";
#endif
			appBuilder
				.ConfigureLifecycleEvents(lifecycle =>
				{
#if __IOS__
					lifecycle
						.AddiOS(iOS => iOS
							.OpenUrl((app, url, options) =>
								Microsoft.Maui.Essentials.Platform.OpenUrl(app, url, options))
							.ContinueUserActivity((application, userActivity, completionHandler) =>
								Microsoft.Maui.Essentials.Platform.ContinueUserActivity(application, userActivity, completionHandler))
							.PerformActionForShortcutItem((application, shortcutItem, completionHandler) =>
								Microsoft.Maui.Essentials.Platform.PerformActionForShortcutItem(application, shortcutItem, completionHandler)));
#elif WINDOWS
					lifecycle
						.AddWindows(windows => windows
							.OnLaunched((app, e) =>
								Microsoft.Maui.Essentials.Platform.OnLaunched(e)));
#elif TIZEN
					Microsoft.Maui.Essentials.Platform.Init(CoreUIAppContext.GetInstance(MauiApplication.Current).MainWindow);
#endif
				})
				.UseMauiApp<App>();

#if TIZEN
			appBuilder
				.Services.AddTransient((_) =>
				{
					var option = new InitializationOptions
					{
						DisplayResolutionUnit = DisplayResolutionUnit.DP(true),
					};
					return option;
				});
#endif

			return appBuilder.Build();
		}
	}
}
