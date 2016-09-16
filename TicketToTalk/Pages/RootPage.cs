// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 12/08/2016
//
// RootPage.cs

using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Root page.
	/// </summary>
	public class RootPage : MasterDetailPage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.RootPage"/> class.
		/// </summary>
		public RootPage()
		{
			Title = "Root";
			var menuPage = new MenuPage();

			menuPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as NavMenuItem);

			Master = menuPage;

			var nav = new NavigationPage(new AllProfiles());
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;

			Detail = nav;
		}

		/// <summary>
		/// Navigates to.
		/// </summary>
		/// <param name="menu">Menu.</param>
		void NavigateTo(NavMenuItem menu)
		{
			var displayPage = (Page)Activator.CreateInstance(menu.TargetType);

			var nav = new NavigationPage(displayPage);
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			Detail = nav;

			IsPresented = false;
		}
	}
}


