using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace fitApp.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
					//initialize calendar
					XamForms.Controls.iOS.Calendar.Init();
					LoadApplication(new App());

			// initialize Oxyplot renderers
			OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();

			return base.FinishedLaunching(app, options);
		}
	}
}
