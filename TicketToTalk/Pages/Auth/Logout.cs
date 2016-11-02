using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Logout.
	/// </summary>
	public class Logout : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Logout"/> class.
		/// </summary>
		public Logout()
		{
			Session.activeUser = null;
			Session.activePerson = null;
			Session.Token = null;

			var nav = new NavigationPage(new Login());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Application.Current.MainPage = nav;
		}
	}
}