﻿using System;
using TChromium = Tizen.WebView.Chromium;
using TWebView = Tizen.WebView.WebView;

namespace Microsoft.Maui.Handlers
{
	public partial class WebViewHandler : ViewHandler<IWebView, MauiWebView>
	{
		protected virtual double MinimumSize => 44d;

		TWebView NativeWebView => NativeView.WebView;

		public static void MapSource(WebViewHandler handler, IWebView webView)
		{
			IWebViewDelegate? webViewDelegate = handler.NativeView as IWebViewDelegate;
			handler.NativeView?.UpdateSource(webView, webViewDelegate);
		}

		protected override MauiWebView CreateNativeView()
		{
			_ = NativeParent ?? throw new InvalidOperationException($"{nameof(NativeParent)} should have been set by base class.");
			return new MauiWebView(NativeParent)
			{
				MinimumHeight = MinimumSize.ToScaledPixel(),
				MinimumWidth = MinimumSize.ToScaledPixel()
			};
		}

		protected override void ConnectHandler(MauiWebView nativeView)
		{
			TChromium.Initialize();
			MauiApplication.Current.Terminated += (sender, arg) => TChromium.Shutdown();
			NativeWebView.LoadFinished += OnLoadFinished;
		}

		protected override void DisconnectHandler(MauiWebView nativeView)
		{
			NativeWebView.StopLoading();
			NativeWebView.LoadFinished -= OnLoadFinished;
			base.DisconnectHandler(nativeView);
		}

		void OnLoadFinished(object? sender, EventArgs e)
		{
			NativeWebView.SetFocus(true);
		}
	}
}