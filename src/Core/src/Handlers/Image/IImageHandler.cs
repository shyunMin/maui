#if __IOS__ || MACCATALYST
using NativeView = UIKit.UIImageView;
#elif MONOANDROID
using NativeView = Android.Widget.ImageView;
#elif WINDOWS
using NativeView = Microsoft.UI.Xaml.Controls.Image;
#elif TIZEN
using NativeView = Tizen.UIExtensions.ElmSharp.Image;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID && !TIZEN)
using NativeView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public partial interface IImageHandler : IViewHandler
	{
		IImage TypedVirtualView { get; }
		ImageSourcePartLoader SourceLoader { get; }
		NativeView TypedNativeView { get; }
	}
}