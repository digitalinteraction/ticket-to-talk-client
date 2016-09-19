using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class AddPersonChoice : ContentPage
	{
		UserController userController = new UserController();
		List<Invitation> rawInvites;

		public AddPersonChoice()
		{
			Title = "New Person";
			var newPersonButton = new Button
			{
				Text = "Add Someone New",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
				BackgroundColor = ProjectResource.color_red,
				TextColor = ProjectResource.color_white,
				WidthRequest = 200
			};
			newPersonButton.Clicked += NewPersonButton_Clicked;

			var joinPersonButton = new Button
			{
				Text = "See Invitations",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand,
				BackgroundColor = ProjectResource.color_grey,
				TextColor = ProjectResource.color_white,
				WidthRequest = 200,
				IsEnabled = false
			};
			joinPersonButton.Clicked += JoinPersonButton_Clicked;

			// Get invitations
			rawInvites = Task.Run(() => userController.getInvitations()).Result;

			if (rawInvites.Count > 0)
			{
				joinPersonButton.Text = string.Format("See Invitations({0})", rawInvites.Count);
				joinPersonButton.BackgroundColor = ProjectResource.color_red;
				joinPersonButton.IsEnabled = true;
			}

			Content = new StackLayout
			{
				Spacing = 12,
				Children =
				{
					newPersonButton,
					joinPersonButton
				}
			};
		}

		/// <summary>
		/// News the person button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void NewPersonButton_Clicked(object sender, EventArgs e)
		{
			var nav = new NavigationPage(new AddPerson(null));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;

			Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Joins the person button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void JoinPersonButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new SeeInvitations(rawInvites));
		}
	}
}