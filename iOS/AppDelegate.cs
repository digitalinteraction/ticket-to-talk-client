using System;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using UIKit;

namespace TicketToTalk.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
			ImageCircleRenderer.Init();

			var swidth = "" + UIScreen.MainScreen.Bounds.Width;
			Session.ScreenWidth = Int32.Parse(swidth);
			Session.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;

			LoadApplication(new App());

			UITabBar.Appearance.SelectedImageTintColor = UIColor.FromRGB(255, 94, 91);
			UITabBar.Appearance.BackgroundColor = UIColor.FromRGB(0, 206, 203);

			Console.WriteLine(Session.ScreenWidth);

			return base.FinishedLaunching(app, options);
		}
	}
}

