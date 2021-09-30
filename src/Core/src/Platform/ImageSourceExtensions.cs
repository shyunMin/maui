﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

#if __IOS__ || MACCATALYST
using NativeImage = UIKit.UIImage;
#elif MONOANDROID
using NativeImage = Android.Graphics.Drawables.Drawable;
#elif WINDOWS
using NativeImage = Microsoft.UI.Xaml.Media.ImageSource;
#elif TIZEN
using NativeImage = Tizen.UIExtensions.ElmSharp.Image;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID && !TIZEN)
using NativeImage = System.Object;
#endif

namespace Microsoft.Maui
{
	public static class ImageSourceExtensions
	{
		public static void LoadImage(this IImageSource source, IMauiContext mauiContext, Action<IImageSourceServiceResult<NativeImage>?>? finished = null)
		{
			LoadImageResult(source.GetNativeImageAsync(mauiContext), finished)
						.FireAndForget(mauiContext.Services.CreateLogger<IImageSource>(), nameof(LoadImage));
		}

		static async Task LoadImageResult(Task<IImageSourceServiceResult<NativeImage>?> task, Action<IImageSourceServiceResult<NativeImage>?>? finished = null)
		{
			var result = await task;
			finished?.Invoke(result);
		}

		public static Task<IImageSourceServiceResult<NativeImage>?> GetNativeImageAsync(this IImageSource imageSource, IMauiContext mauiContext)
		{
			if (imageSource == null)
				return new Task<IImageSourceServiceResult<NativeImage>?>(() => null);
			var services = mauiContext.Services;
			var provider = services.GetRequiredService<IImageSourceServiceProvider>();
			var imageSourceService = provider.GetRequiredImageSourceService(imageSource);
			return imageSource.GetNativeImageAsync(mauiContext, imageSourceService);
		}

		public static Task<IImageSourceServiceResult<NativeImage>?> GetNativeImageAsync(this IImageSource imageSource, IMauiContext mauiContext, IImageSourceService imageSourceService)
		{
#if __IOS__ || MACCATALYST
			return imageSourceService.GetImageAsync(imageSource);
#elif MONOANDROID
			return imageSourceService.GetDrawableAsync(imageSource, mauiContext.Context!);
#elif WINDOWS
			return imageSourceService.GetImageSourceAsync(imageSource);
#elif TIZEN
			var nativeImage = new NativeImage(mauiContext.Context!.NativeParent!);
			return imageSourceService.GetImageAsync(imageSource, nativeImage);
#else
			throw new NotImplementedException();
#endif
		}
	}
}
