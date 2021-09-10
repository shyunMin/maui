using System;
using System.Runtime.InteropServices;
using Tizen.Applications;

namespace Microsoft.Maui.Essentials
{
	public static partial class DeviceDisplay
	{
		[DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_request_lock")]
		static extern void RequestKeepScreenOn(int type = 1, int timeout = 0);

		[DllImport("libcapi-system-device.so.0", EntryPoint = "device_power_release_lock")]
		static extern void ReleaseKeepScreenOn(int type = 1);

		static bool keepScreenOn = false;

		static bool PlatformKeepScreenOn
		{
			get => keepScreenOn;
			set
			{
				if (value)
					RequestKeepScreenOn();
				else
					ReleaseKeepScreenOn();
				keepScreenOn = value;
			}
		}

		static CoreUIApplication CoreUIApplication
		{
			get
			{
				return Application.Current as CoreUIApplication;
			}
		}

		static int displayWidth = Platform.GetFeatureInfo<int>("screen.width");
		static int displayHeight = Platform.GetFeatureInfo<int>("screen.height");
		static int displayDpi = DeviceInfo.Idiom == DeviceIdiom.TV ? 72 : Platform.GetFeatureInfo<int>("screen.dpi");
		static DisplayOrientation displayOrientation = GetNaturalDisplayOrientation();
		static DisplayRotation displayRotation = DisplayRotation.Rotation0;
		static DisplayInfo GetMainDisplayInfo()
		{
			return new DisplayInfo(
				width: displayWidth,
				height: displayHeight,
				density: displayDpi / 160.0,
				orientation: displayOrientation,
				rotation: displayRotation
				);
		}

		static void StartScreenMetricsListeners()
		{
			if (CoreUIApplication != null)
			{
				CoreUIApplication.DeviceOrientationChanged += OnRotationChanged;
			}
		}

		static void StopScreenMetricsListeners()
		{
			if (CoreUIApplication != null)
			{
				CoreUIApplication.DeviceOrientationChanged -= OnRotationChanged;
			}
		}

		static DisplayOrientation GetNaturalDisplayOrientation()
		{
			if (displayHeight >= displayWidth)
			{
				return DisplayOrientation.Portrait;
			}
			else
			{
				return DisplayOrientation.Landscape;
			}
		}

		static void OnRotationChanged(object s, DeviceOrientationEventArgs e)
		{
			switch (e.DeviceOrientation)
			{
				case DeviceOrientation.Orientation_0:
					displayRotation = DisplayRotation.Rotation0;
					displayOrientation = GetNaturalDisplayOrientation();
					break;
				case DeviceOrientation.Orientation_90:
					displayRotation = DisplayRotation.Rotation90;
					displayOrientation = GetNaturalDisplayOrientation() == DisplayOrientation.Portrait ? DisplayOrientation.Landscape : DisplayOrientation.Portrait;
					break;
				case DeviceOrientation.Orientation_180:
					displayRotation = DisplayRotation.Rotation180;
					displayOrientation = GetNaturalDisplayOrientation();
					break;
				case DeviceOrientation.Orientation_270:
					displayRotation = DisplayRotation.Rotation270;
					displayOrientation = GetNaturalDisplayOrientation() == DisplayOrientation.Portrait ? DisplayOrientation.Landscape : DisplayOrientation.Portrait;
					break;
				default:
					displayRotation = DisplayRotation.Unknown;
					displayOrientation = DisplayOrientation.Unknown;
					break;
			}
			OnMainDisplayInfoChanged(GetMainDisplayInfo());
		}
	}
}
