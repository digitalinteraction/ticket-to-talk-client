using System.Collections.Generic;
using System.Diagnostics;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// App
	/// </summary>
	public class App : Application
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.App"/> class.
		/// </summary>
		public App()
		{
			GoogleAnalytics.Current.Config.TrackingId = "UA-99833865-1";
			GoogleAnalytics.Current.Config.AppId = "ticket-to-talk";
			GoogleAnalytics.Current.Config.AppName = "ticket-to-talk";
			GoogleAnalytics.Current.Config.AppVersion = "1.0";
			GoogleAnalytics.Current.InitTracker();

			var periodController = new PeriodController();
			periodController.InitStockPeriods();

			var nav = new NavigationPage(new Login());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			MainPage = nav;
		}
	}
}

