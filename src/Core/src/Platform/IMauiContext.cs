using System;

namespace Microsoft.Maui
{
	public interface IMauiContext
	{
		IServiceProvider Services { get; }

		IMauiHandlersServiceProvider Handlers { get; }

#if __ANDROID__
		Android.Content.Context? Context { get; }
#elif TIZEN
		CoreUIAppContext? Context { get; }
		ElmSharp.Window? Window { get; }
#endif
	}
}