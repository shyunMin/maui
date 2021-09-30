using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Hosting
{
	public static partial class AppHostBuilderExtensions
	{
		static readonly Dictionary<Type, Type> DefaultMauiControlHandlers = new Dictionary<Type, Type>
		{
#if WINDOWS || __ANDROID__ || TIZEN
			{ typeof(Shell), typeof(ShellHandler) },
#endif
			{ typeof(ActivityIndicator), typeof(ActivityIndicatorHandler) },
			{ typeof(BoxView), typeof(ShapeViewHandler) },
			{ typeof(Button), typeof(ButtonHandler) },
			{ typeof(CheckBox), typeof(CheckBoxHandler) },
			{ typeof(DatePicker), typeof(DatePickerHandler) },
			{ typeof(Editor), typeof(EditorHandler) },
			{ typeof(Entry), typeof(EntryHandler) },
			{ typeof(GraphicsView), typeof(GraphicsViewHandler) },
			{ typeof(Image), typeof(ImageHandler) },
			{ typeof(Label), typeof(LabelHandler) },
			{ typeof(Layout), typeof(LayoutHandler) },
			{ typeof(Picker), typeof(PickerHandler) },
			{ typeof(ProgressBar), typeof(ProgressBarHandler) },
			{ typeof(ScrollView), typeof(ScrollViewHandler) },
			{ typeof(SearchBar), typeof(SearchBarHandler) },
			{ typeof(Slider), typeof(SliderHandler) },
			{ typeof(Stepper), typeof(StepperHandler) },
			{ typeof(Switch), typeof(SwitchHandler) },
			{ typeof(TimePicker), typeof(TimePickerHandler) },
			{ typeof(Page), typeof(PageHandler) },
			{ typeof(WebView), typeof(WebViewHandler) },
			{ typeof(Border), typeof(BorderHandler) },
			{ typeof(IContentView), typeof(ContentViewHandler) },
			{ typeof(Shapes.Ellipse), typeof(ShapeViewHandler) },
			{ typeof(Shapes.Line), typeof(ShapeViewHandler) },
			{ typeof(Shapes.Path), typeof(ShapeViewHandler) },
			{ typeof(Shapes.Polygon), typeof(ShapeViewHandler) },
			{ typeof(Shapes.Polyline), typeof(ShapeViewHandler) },
			{ typeof(Shapes.Rectangle), typeof(ShapeViewHandler) },
			{ typeof(Shapes.RoundRectangle), typeof(ShapeViewHandler) },
			{ typeof(Window), typeof(WindowHandler) },
			{ typeof(ImageButton), typeof(ImageButtonHandler) },
#if __ANDROID__ || __IOS__
			{ typeof(RefreshView), typeof(RefreshViewHandler) },
			{ typeof(IndicatorView), typeof(IndicatorViewHandler) },
#endif
#if __ANDROID__  || WINDOWS
			{ typeof(NavigationPage), typeof(NavigationPageHandler) },
#endif
#if __ANDROID__
			{ typeof(TabbedPage), typeof(Controls.Handlers.TabbedPageHandler) },
#endif
		};

		public static IMauiHandlersCollection AddMauiControlsHandlers(this IMauiHandlersCollection handlersCollection)
			=> handlersCollection.AddHandlers(DefaultMauiControlHandlers);
	}
}
