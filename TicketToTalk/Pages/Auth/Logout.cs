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
			Application.Current.MainPage = new Login();
		}
	}
}

