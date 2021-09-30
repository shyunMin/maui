﻿using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Microsoft.Maui
{
	public class FontManager : IFontManager
	{
		readonly ConcurrentDictionary<(string family, float size, FontSlant slant), string> _fonts = new();

		readonly IFontRegistrar _fontRegistrar;
		readonly ILogger<FontManager>? _logger;

		public FontManager(IFontRegistrar fontRegistrar, ILogger<FontManager>? logger = null)
		{
			_fontRegistrar = fontRegistrar;
			_logger = logger;
		}


		public string GetFont(Font font)
		{
			var size = (float)font.FontSize;

			return GetFont(font.FontFamily, size, font.FontSlant, GetNativeFontFamily);
		}

		public string GetFontFamily(string fontFamliy)
		{
			if (string.IsNullOrEmpty(fontFamliy))
				return "";

			var cleansedFont = CleanseFontName(fontFamliy);
			if (cleansedFont == null)
				return "";

			int index = cleansedFont.LastIndexOf('-');
			if (index != -1)
			{
				string font = cleansedFont.Substring(0, index);
				string style = cleansedFont.Substring(index + 1);
				return $"{font}:style={style}";
			}
			else
			{
				return cleansedFont;
			}
		}

		string GetFont(string family, float size, FontSlant slant, Func<(string, float, FontSlant), string> factory)
		{
			return _fonts.GetOrAdd((family, size, slant), factory);
		}

		string GetNativeFontFamily((string family, float size, FontSlant slant) fontKey)
		{
			if (string.IsNullOrEmpty(fontKey.family))
				return "";

			var cleansedFont = CleanseFontName(fontKey.family);

			if (cleansedFont == null)
				return "";

			int index = cleansedFont.LastIndexOf('-');
			if (index != -1)
			{
				string font = cleansedFont.Substring(0, index);
				string style = cleansedFont.Substring(index + 1);
				return $"{font}:style={style}";
			}
			else
			{
				return cleansedFont;
			}
		}

		string? CleanseFontName(string fontName)
		{
			// First check Alias
			if (_fontRegistrar.GetFont(fontName) is string fontPostScriptName)
				return fontPostScriptName;

			var fontFile = FontFile.FromString(fontName);

			if (!string.IsNullOrWhiteSpace(fontFile.Extension))
			{
				if (_fontRegistrar.GetFont(fontFile.FileNameWithExtension()) is string filePath)
					return filePath ?? fontFile.PostScriptName;
			}
			else
			{
				foreach (var ext in FontFile.Extensions)
				{

					var formatted = fontFile.FileNameWithExtension(ext);
					if (_fontRegistrar.GetFont(formatted) is string filePath)
						return filePath;
				}
			}

			return fontFile.PostScriptName;
		}
	}
}