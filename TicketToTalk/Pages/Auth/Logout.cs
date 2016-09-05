using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class Logout : ContentPage
	{
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