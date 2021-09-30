﻿#nullable enable

using System;
using System.ComponentModel;
using ElmSharp;
using Tizen.UIExtensions.Shell;
using EColor = ElmSharp.Color;
using TSearchBar = Tizen.UIExtensions.ElmSharp.SearchBar;
using TTextChangedEventArgs = Tizen.UIExtensions.Common.TextChangedEventArgs;

namespace Microsoft.Maui.Controls.Platform
{
	public class ShellSearchView : IDisposable
	{
		bool disposedValue;
		ShellSearchResultList? _searchResultList;

		public ShellSearchView(SearchHandler searchHandler, IMauiContext context)
		{
			Element = searchHandler;
			MauiContext = context;

			Element.FocusChangeRequested += OnFocusChangedRequested;
			Element.PropertyChanged += OnElementPropertyChanged;
			(Element as ISearchHandlerController).ListProxyChanged += OnSearchResultListChanged;

			if (NativeParent != null)
			{
				Control = new TSearchBar(NativeParent)
				{
					IsSingleLine = true,
				};
				Control.Show();
				Control.SetInputPanelReturnKeyType(InputPanelReturnKeyType.Search);
				Control.TextChanged += OnTextChanged;
				Control.Activated += OnActivated;
				Control.Focused += OnFocused;
				Control.Unfocused += OnFocused;
			}

			UpdateKeyboard();
			UpdatePlaceholder();
			UpdatePlaceholderColor();
			UpdateHorizontalTextAlignment();
			UpdateTextColor();
			UpdateFontAttributes();
			UpdateFontFamily();
			UpdateFontSize();
			UpdateBackgroundColor();
			UpdateQuery();
			UpdateIsSearchEnabled();
			UpdateSearchResult();
		}

		public SearchHandler Element { get; }
		
		public EvasObject? NativeView => Control;

		protected IMauiContext MauiContext { get; private set; }

		protected EvasObject? NativeParent
		{
			get => MauiContext.Context?.BaseLayout;
		}

		ISearchHandlerController SearchHandlerController => Element;
		
		TSearchBar? Control { get; }

		~ShellSearchView()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Element.FocusChangeRequested -= OnFocusChangedRequested;
					Element.PropertyChanged -= OnElementPropertyChanged;
					(Element as ISearchHandlerController).ListProxyChanged -= OnSearchResultListChanged;

					if (Control != null)
					{
						Control.TextChanged -= OnTextChanged;
						Control.Activated -= OnActivated;
						Control.Focused -= OnFocused;
						Control.Unfocused -= OnFocused;
						Control.Unrealize();
					}
				}
				disposedValue = true;
			}
		}

		void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Element.Keyboard))
			{
				UpdateKeyboard();
			}
			else if (e.PropertyName == nameof(Element.Placeholder))
			{
				UpdatePlaceholder();
			}
			else if (e.PropertyName == nameof(Element.PlaceholderColor))
			{
				UpdatePlaceholderColor();
			}
			else if (e.PropertyName == nameof(Element.HorizontalTextAlignment))
			{
				UpdateHorizontalTextAlignment();
			}
			else if (e.PropertyName == nameof(Element.TextColor))
			{
				UpdateTextColor();
			}
			else if (e.PropertyName == nameof(Element.FontAttributes))
			{
				UpdateFontAttributes();
			}
			else if (e.PropertyName == nameof(Element.FontFamily))
			{
				UpdateFontFamily();
			}
			else if (e.PropertyName == nameof(Element.FontSize))
			{
				UpdateFontSize();
			}
			else if (e.PropertyName == nameof(Element.BackgroundColor))
			{
				UpdateBackgroundColor();
			}
			else if (e.PropertyName == nameof(Element.Query))
			{
				UpdateQuery();
			}
			else if (e.PropertyName == nameof(Element.IsSearchEnabled))
			{
				UpdateIsSearchEnabled();
			}
			else if (e.PropertyName == nameof(Element.ShowsResults))
			{
				UpdateSearchResult();
			}
		}

		void OnSearchResultListChanged(object sender, ListProxyChangedEventArgs e)
		{
			UpdateSearchResult();
		}

		void InitializeSearchResultList()
		{
			if (_searchResultList != null)
			{
				return;
			}
			_searchResultList = new ShellSearchResultList(MauiContext);
			_searchResultList.Show();
			_searchResultList.ItemSelected += OnResultItemSelected;
		}

		void OnResultItemSelected(object sender, GenListItemEventArgs e)
		{
			var data = (e.Item.Data as View)?.BindingContext;

			if (data != null)
			{
				SearchHandlerController.ItemSelected(data);
				Device.BeginInvokeOnMainThread(() =>
				{
					DeinitializeSearchResultList();
				});
			}
		}

		void DeinitializeSearchResultList()
		{
			if (_searchResultList == null)
			{
				return;
			}

			_searchResultList.ItemSelected -= OnResultItemSelected;
			_searchResultList.Unrealize();
			_searchResultList = null;
		}

		void UpdateSearchResult()
		{
			if (SearchHandlerController == null)
				return;

			if (!Element.ShowsResults)
			{
				DeinitializeSearchResultList();
				return;
			}

			if (Control != null &&
				Control.IsFocused && SearchHandlerController.ListProxy != null &&
				SearchHandlerController.ListProxy.Count > 0 &&
				Element.ItemTemplate != null)
			{
				InitializeSearchResultList();
				if (_searchResultList != null)
				{
					_searchResultList.ItemTemplate = Element.ItemTemplate;
					_searchResultList.ItemsSource = SearchHandlerController.ListProxy;
					UpdateSearchResultLayout();
				}
			}
			else
			{
				DeinitializeSearchResultList();
			}
		}

		void UpdateIsSearchEnabled()
		{
			if (Control == null)
				return;

			Control.IsEnabled = Element.IsSearchEnabled;
		}

		void UpdateQuery()
		{
			if (Control == null)
				return;

			Control.Text = (Element.Query != null) ? Element.Query : "";
		}

		void UpdateFontAttributes()
		{
			if (Control == null)
				return;

			Control.FontAttributes = Element.FontAttributes.ToNative();
		}

		void UpdateFontFamily()
		{
			if (Control == null)
				return;

			Control.FontFamily = Element.FontFamily;
		}

		void UpdateFontSize()
		{
			if (Control == null)
				return;

			Control.FontSize = Element.FontSize;
		}

		void UpdateBackgroundColor()
		{
			if (Control == null)
				return;

			var color = Element.BackgroundColor.ToNativeEFL();
			Control.BackgroundColor = color == EColor.Default ? EColor.White : color;
		}

		void UpdateTextColor()
		{
			if (Control == null)
				return;

			Control.TextColor = Element.TextColor.ToNative();
		}

		void UpdateHorizontalTextAlignment()
		{
			if (Control == null)
				return;

			Control.HorizontalTextAlignment = Element.HorizontalTextAlignment.ToNative();
		}

		void OnFocusChangedRequested(object sender, VisualElement.FocusRequestArgs e)
		{
			if (Control == null)
				return;

			Control.SetFocus(e.Focus);
			e.Result = true;
		}

		void UpdateKeyboard()
		{
			if (Control == null)
				return;

			Control.Keyboard = Element.Keyboard.ToNative();
		}

		void UpdatePlaceholder()
		{
			if (Control == null)
				return;

			Control.Placeholder = Element.Placeholder;
		}
		void UpdatePlaceholderColor()
		{
			if (Control == null)
				return;

			Control.PlaceholderColor = Element.PlaceholderColor.ToNative();
		}

		void OnFocused(object sender, EventArgs e)
		{
			if (Control == null)
				return;

			Element.SetIsFocused(Control.IsFocused);
			if (Control.IsFocused)
			{
				UpdateSearchResult();
			}
			else
			{
				if (_searchResultList != null)
				{
					_searchResultList.Hide();
				}
				Device.BeginInvokeOnMainThread(() =>
				{
					Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
					{
						DeinitializeSearchResultList();
						return false;
					});
				});
			}
		}

		void OnActivated(object sender, EventArgs e)
		{
			if (Control == null)
				return;

			Control.HideInputPanel();
			(Element as ISearchHandlerController).QueryConfirmed();
		}

		void OnTextChanged(object sender, TTextChangedEventArgs e)
		{
			Element.SetValueCore(SearchHandler.QueryProperty, (sender as TSearchBar)?.Text);
		}

		void UpdateSearchResultLayout()
		{
			if (_searchResultList != null && NativeView != null)
			{
				var bound = NativeView.Geometry;
				bound.Y += NativeView.Geometry.Height;
				_searchResultList.Geometry = bound;
				_searchResultList.UpdateLayout();
			}
		}
	}
}
