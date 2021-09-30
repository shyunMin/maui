using System;
using Microsoft.Maui.Controls.Compatibility.Platform.Tizen;
using ElmSharp;
using ViewHandler = Microsoft.Maui.Handlers.EViewHandler<Microsoft.Maui.IView, ElmSharp.EvasObject>;

namespace Microsoft.Maui.Controls.Compatibility
{
	public class RendererToHandlerShim : ViewHandler
	{
		internal IVisualElementRenderer VisualElementRenderer { get; private set; }

		public static IViewHandler CreateShim(object renderer)
		{
			if (renderer is IViewHandler handler)
				return handler;

			if (renderer is IVisualElementRenderer ivr)
				return new RendererToHandlerShim(ivr);

			return new RendererToHandlerShim();
		}

		public RendererToHandlerShim() : base(ViewHandler.ViewMapper)
		{
		}

		public RendererToHandlerShim(IVisualElementRenderer visualElementRenderer) : this()
		{
			if (visualElementRenderer != null)
				SetupRenderer(visualElementRenderer);
		}

		public void SetupRenderer(IVisualElementRenderer visualElementRenderer)
		{
			VisualElementRenderer = visualElementRenderer;
			VisualElementRenderer.ElementChanged += OnElementChanged;

			if (VisualElementRenderer.Element is IView view)
			{
				view.Handler = this;
				SetVirtualView(view);
			}
			else if (VisualElementRenderer.Element != null)
				throw new Exception($"{VisualElementRenderer.Element} must implement: {nameof(IView)}");
		}

		void OnElementChanged(object sender, VisualElementChangedEventArgs e)
		{
			if (e.OldElement is IView view)
				view.Handler = null;

			if (e.NewElement is IView newView)
			{
				newView.Handler = this;
				this.SetVirtualView(newView);
			}
			else if (e.NewElement != null)
				throw new Exception($"{e.NewElement} must implement: {nameof(IView)}");
		}

		protected override EvasObject CreateNativeView()
		{
			return VisualElementRenderer.NativeView;
		}

		protected override void ConnectHandler(EvasObject nativeView)
		{
			base.ConnectHandler(nativeView);
			VirtualView.Handler = this;
		}

		protected override void DisconnectHandler(EvasObject nativeView)
		{
			Platform.Tizen.Platform.SetRenderer(VisualElementRenderer.Element, VisualElementRenderer);

			VisualElementRenderer.SetElement(null);

			base.DisconnectHandler(nativeView);
			VirtualView.Handler = null;
		}

		public override void SetVirtualView(IView view)
		{
			if (VisualElementRenderer == null)
			{
				var renderer = Internals.Registrar.Registered.GetHandlerForObject<IVisualElementRenderer>(view) ?? new DefaultRenderer();

				SetupRenderer(renderer);
			}

			if (VisualElementRenderer.Element != view)
			{
				VisualElementRenderer.SetElement((VisualElement)view);
			}
			else
			{
				base.SetVirtualView(view);
			}

			Platform.Tizen.Platform.SetRenderer(VisualElementRenderer.Element, VisualElementRenderer);
		}

		public override void UpdateValue(string property)
		{
			base.UpdateValue(property);
			if (property == "Frame")
			{
				NativeArrange(VisualElementRenderer.Element.Bounds);
			}
		}
	}
}
