﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maui.Controls.Sample.Controls;
using Maui.Controls.Sample.Pages;
using Maui.Controls.Sample.Services;
using Maui.Controls.Sample.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Components.WebView.Maui;
#endif

namespace Maui.Controls.Sample
{
	public class CustomButton : Button { }

	public static class MauiProgram
	{
		enum PageType { Main, Blazor, Shell, Template }
		readonly static PageType _pageType = PageType.Main;

		public static MauiApp CreateMauiApp()
		{
			var appBuilder = MauiApp.CreateBuilder();

			appBuilder.UseMauiApp<XamlApp>();
			var services = appBuilder.Services;

			appBuilder
				.ConfigureMauiHandlers(handlers =>
				{
#if __ANDROID__
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat.ButtonRenderer));
#elif __IOS__
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.iOS.ButtonRenderer));
#elif WINDOWS
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.UWP.ButtonRenderer));
#elif TIZEN
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.Tizen.ButtonRenderer));
#endif
				});

			// Use a "third party" library that brings in a massive amount of controls
			appBuilder.UseBordelessEntry();
			appBuilder.ConfigureEffects(builder =>
			{
				builder.Add<FocusRoutingEffect, FocusPlatformEffect>();
			});

			appBuilder.Configuration.AddInMemoryCollection(
				new Dictionary<string, string>
					{
						{"MyKey", "Dictionary MyKey Value"},
						{":Title", "Dictionary_Title"},
						{"Position:Name", "Dictionary_Name" },
						{"Logging:LogLevel:Default", "Warning"}
					});

#if NET6_0_OR_GREATER
			appBuilder
				.RegisterBlazorMauiWebView();
			services.AddBlazorWebView();
#endif

			services.AddLogging(logging =>
			{
#if WINDOWS
				logging.AddDebug();
#else
				logging.AddConsole();
#endif
			});
#if TIZEN
			services.AddTransient<InitializationOptions>((_) => 
			{
				var option = new InitializationOptions
				{
					DisplayResolutionUnit = DisplayResolutionUnit.DP(true),
					UseSkiaSharp = true
				};
				return option;
			});
#endif

			services.AddSingleton<ITextService, TextService>();
			services.AddTransient<MainViewModel>();

			services.AddTransient<IWindow, Window>();

			services.AddTransient(
				serviceType: typeof(Page),
				implementationType: _pageType switch
				{
					PageType.Template => typeof(TemplatePage),
					PageType.Shell => typeof(AppShell),
					PageType.Main => typeof(CustomNavigationPage),
					PageType.Blazor =>
#if NET6_0_OR_GREATER
								typeof(BlazorPage),
#else
								throw new NotSupportedException("Blazor requires .NET 6 or higher."),
#endif
					_ => throw new Exception(),
				});

			appBuilder
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("Dokdo-Regular.ttf", "Dokdo");
					fonts.AddFont("LobsterTwo-Regular.ttf", "Lobster Two");
					fonts.AddFont("LobsterTwo-Bold.ttf", "Lobster Two Bold");
					fonts.AddFont("LobsterTwo-Italic.ttf", "Lobster Two Italic");
					fonts.AddFont("LobsterTwo-BoldItalic.ttf", "Lobster Two BoldItalic");
					fonts.AddFont("ionicons.ttf", "Ionicons");
					fonts.AddFont("SegoeUI.ttf", "Segoe UI");
					fonts.AddFont("SegoeUI-Bold.ttf", "Segoe UI Bold");
					fonts.AddFont("SegoeUI-Italic.ttf", "Segoe UI Italic");
					fonts.AddFont("SegoeUI-Bold-Italic.ttf", "Segoe UI Bold Italic");
				})
				.ConfigureEssentials(essentials =>
				{
					essentials
						.UseVersionTracking()
						.UseMapServiceToken("YOUR-KEY-HERE")
						.AddAppAction("test_action", "Test App Action")
						.AddAppAction("second_action", "Second App Action")
						.OnAppAction(appAction =>
						{
							Debug.WriteLine($"You seem to have arrived from a special place: {appAction.Title} ({appAction.Id})");
						});
				})
				.ConfigureLifecycleEvents(events =>
				{
					events.AddEvent<Action<string>>("CustomEventName", value => LogEvent("CustomEventName"));

#if __ANDROID__
					// Log everything in this one
					events.AddAndroid(android => android
						.OnActivityResult((a, b, c, d) => LogEvent(nameof(AndroidLifecycle.OnActivityResult), b.ToString()))
						.OnBackPressed((a) => LogEvent(nameof(AndroidLifecycle.OnBackPressed)))
						.OnConfigurationChanged((a, b) => LogEvent(nameof(AndroidLifecycle.OnConfigurationChanged)))
						.OnCreate((a, b) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
						.OnDestroy((a) => LogEvent(nameof(AndroidLifecycle.OnDestroy)))
						.OnNewIntent((a, b) => LogEvent(nameof(AndroidLifecycle.OnNewIntent)))
						.OnPause((a) => LogEvent(nameof(AndroidLifecycle.OnPause)))
						.OnPostCreate((a, b) => LogEvent(nameof(AndroidLifecycle.OnPostCreate)))
						.OnPostResume((a) => LogEvent(nameof(AndroidLifecycle.OnPostResume)))
						.OnPressingBack((a) => LogEvent(nameof(AndroidLifecycle.OnPressingBack)) && false)
						.OnRequestPermissionsResult((a, b, c, d) => LogEvent(nameof(AndroidLifecycle.OnRequestPermissionsResult)))
						.OnRestart((a) => LogEvent(nameof(AndroidLifecycle.OnRestart)))
						.OnRestoreInstanceState((a, b) => LogEvent(nameof(AndroidLifecycle.OnRestoreInstanceState)))
						.OnResume((a) => LogEvent(nameof(AndroidLifecycle.OnResume)))
						.OnSaveInstanceState((a, b) => LogEvent(nameof(AndroidLifecycle.OnSaveInstanceState)))
						.OnStart((a) => LogEvent(nameof(AndroidLifecycle.OnStart)))
						.OnStop((a) => LogEvent(nameof(AndroidLifecycle.OnStop))));

					// Add some cool features/things
					var shouldPreventBack = 1;
					events.AddAndroid(android => android
						.OnResume(a =>
						{
							LogEvent(nameof(AndroidLifecycle.OnResume), "shortcut");
						})
						.OnPressingBack(a =>
						{
							LogEvent(nameof(AndroidLifecycle.OnPressingBack), "shortcut");

							return shouldPreventBack-- > 0;
						})
						.OnBackPressed(a => LogEvent(nameof(AndroidLifecycle.OnBackPressed), "shortcut"))
						.OnRestoreInstanceState((a, b) =>
						{
							LogEvent(nameof(AndroidLifecycle.OnRestoreInstanceState), "shortcut");

							Debug.WriteLine($"{b.GetString("test2", "fail")} == {b.GetBoolean("test", false)}");
						})
						.OnSaveInstanceState((a, b) =>
						{
							LogEvent(nameof(AndroidLifecycle.OnSaveInstanceState), "shortcut");

							b.PutBoolean("test", true);
							b.PutString("test2", "yay");
						}));
#elif __IOS__
					// Log everything in this one
					events.AddiOS(ios => ios
						.ContinueUserActivity((a, b, c) => LogEvent(nameof(iOSLifecycle.ContinueUserActivity)) && false)
						.DidEnterBackground((a) => LogEvent(nameof(iOSLifecycle.DidEnterBackground)))
						.FinishedLaunching((a, b) => LogEvent(nameof(iOSLifecycle.FinishedLaunching)) && true)
						.OnActivated((a) => LogEvent(nameof(iOSLifecycle.OnActivated)))
						.OnResignActivation((a) => LogEvent(nameof(iOSLifecycle.OnResignActivation)))
						.OpenUrl((a, b, c) => LogEvent(nameof(iOSLifecycle.OpenUrl)) && false)
						.PerformActionForShortcutItem((a, b, c) => LogEvent(nameof(iOSLifecycle.PerformActionForShortcutItem)))
						.WillEnterForeground((a) => LogEvent(nameof(iOSLifecycle.WillEnterForeground)))
						.WillTerminate((a) => LogEvent(nameof(iOSLifecycle.WillTerminate))));
#elif WINDOWS
					// Log everything in this one
					events.AddWindows(windows => windows
						.OnNativeMessage((a, b) => LogEvent(nameof(WindowsLifecycle.OnNativeMessage)))
						.OnActivated((a, b) => LogEvent(nameof(WindowsLifecycle.OnActivated)))
						.OnClosed((a, b) => LogEvent(nameof(WindowsLifecycle.OnClosed)))
						.OnLaunched((a, b) => LogEvent(nameof(WindowsLifecycle.OnLaunched)))
						.OnVisibilityChanged((a, b) => LogEvent(nameof(WindowsLifecycle.OnVisibilityChanged))));
#elif TIZEN
					events.AddTizen(tizen => tizen
						.OnAppControlReceived((a, b) => LogEvent(nameof(TizenLifecycle.OnAppControlReceived)))
						.OnCreate((a) => LogEvent(nameof(TizenLifecycle.OnCreate)))
						.OnDeviceOrientationChanged((a, b) => LogEvent(nameof(TizenLifecycle.OnDeviceOrientationChanged)))
						.OnLocaleChanged((a, b) => LogEvent(nameof(TizenLifecycle.OnLocaleChanged)))
						.OnLowBattery((a, b) => LogEvent(nameof(TizenLifecycle.OnLowBattery)))
						.OnLowMemory((a, b) => LogEvent(nameof(TizenLifecycle.OnLowMemory)))
						.OnPause((a) => LogEvent(nameof(TizenLifecycle.OnPause)))
						.OnPreCreate((a) => LogEvent(nameof(TizenLifecycle.OnPreCreate)))
						.OnRegionFormatChanged((a, b) => LogEvent(nameof(TizenLifecycle.OnRegionFormatChanged)))
						.OnResume((a) => LogEvent(nameof(TizenLifecycle.OnResume)))
						.OnTerminate((a) => LogEvent(nameof(TizenLifecycle.OnTerminate))));
#endif

					static bool LogEvent(string eventName, string type = null)
					{
						Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? "" : $" ({type})")}");
						return true;
					}

#if __ANDROID__
					Microsoft.Maui.Handlers.ButtonHandler.NativeViewFactory = (handler) => 
					{
						return new Google.Android.Material.Button.MaterialButton(handler.Context) 
						{ 
							CornerRadius = 50, SoundEffectsEnabled = true 
						};
					};
#endif
				});

			return appBuilder.Build();
		}
	}
}
