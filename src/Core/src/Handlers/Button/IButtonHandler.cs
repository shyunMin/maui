#if __IOS__ || MACCATALYST
using NativeView = UIKit.UIButton;
#elif MONOANDROID
using NativeView = Google.Android.Material.Button.MaterialButton;
#elif WINDOWS
using NativeView = Microsoft.Maui.MauiButton;
#elif TIZEN
using NativeView = Tizen.UIExtensions.ElmSharp.Button;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID && !TIZEN)
using NativeView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public partial interface IButtonHandler : IViewHandler
	{
		IButton TypedVirtualView { get; }
		NativeView TypedNativeView { get; }
		ImageSourcePartLoader ImageSourceLoader { get; }
	}
}