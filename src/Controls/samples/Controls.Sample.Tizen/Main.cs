using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;

namespace Maui.Controls.Sample.Tizen
{
	class Program : MauiApplication
	{
		protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

		protected override void OnCreate()
		{
			base.OnCreate();
		}

		static void Main(string[] args)
		{
			var app = new Program();
			app.Run(args);
		}
	}
}
