#if __IOS__ || MACCATALYST
using NativeView = UIKit.UIView;
#elif __ANDROID__
using NativeView = Android.Views.View;
#elif WINDOWS
using NativeView = Microsoft.UI.Xaml.FrameworkElement;
#elif TIZEN
using NativeView = ElmSharp.EvasObject;
#elif NETSTANDARD
using NativeView = System.Object;
#endif

namespace Microsoft.Maui
{
	public static class ViewHandlerExtensions
	{
		public static NativeView? GetWrappedNativeView(this IViewHandler viewHandler) =>
			(NativeView?)(viewHandler.ContainerView ?? viewHandler.NativeView);
	}
}
