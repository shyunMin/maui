﻿#if !(__ANDROID__ || __IOS__ || WINDOWS || TIZEN)
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Controls.Compatibility
{
	public class Forms
	{
		public static void Init(IActivationState activationState)
		{
			throw new NotImplementedException();
		}
	}
}
#endif
