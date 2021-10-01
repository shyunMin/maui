﻿using System;
using NativeView = ElmSharp.EvasObject;
using EColor = ElmSharp.Color;
using Tizen.UIExtensions.Common;

namespace Microsoft.Maui.Handlers
{
	public partial class ContentViewHandler : ViewHandler<IContentView, ContentCanvas>
	{
		INativeViewHandler? _contentHandler;

		protected override ContentCanvas CreateNativeView()
		{
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a Page");
			_ = NativeParent ?? throw new InvalidOperationException($"{nameof(NativeParent)} cannot be null");

			var view = new ContentCanvas(NativeParent, VirtualView)
			{
				CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
				CrossPlatformArrange = VirtualView.CrossPlatformArrange
			};

			view.BackgroundColor = (DeviceInfo.GetDeviceType() == DeviceType.TV) ? EColor.Transparent : EColor.White;

			view.Show();
			return view;
		}

		public override Graphics.Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return VirtualView.CrossPlatformMeasure(widthConstraint, heightConstraint);
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			NativeView.CrossPlatformMeasure = VirtualView.CrossPlatformMeasure;
			NativeView.CrossPlatformArrange = VirtualView.CrossPlatformArrange;
		}

		public static void MapContent(ContentViewHandler handler, IContentView page)
		{
			handler.UpdateContent();
		}

		void UpdateContent()
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			NativeView.Children.Clear();
			_contentHandler?.Dispose();
			_contentHandler = null;

			if (VirtualView.PresentedContent is IView view)
			{
				NativeView.Children.Add(view.ToNative(MauiContext));
				if (view.Handler is INativeViewHandler thandler)
				{
					thandler?.SetParent(this);
					_contentHandler = thandler;
				}
			}
		}
	}
}
