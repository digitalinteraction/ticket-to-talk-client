using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class AddPersonChoice : TrackedContentPage
	{
		private UserController userController = new UserController();
		private List<Invitation> rawInvites;

		public AddPersonChoice()
		{

            TrackedName = "Add Person Choice";

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
            newPersonButton.SetStyle();
            newPersonButton.Margin = new Thickness(0, 0, 0, 0);
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
            joinPersonButton.SetStyle();
            joinPersonButton.Margin = new Thickness(0, 0, 0, 0);
			joinPersonButton.Clicked += JoinPersonButton_Clicked;

			// Get invitations
			var task = Task.Run(() => userController.GetInvitations());

			try
			{
				rawInvites = task.Result;
			}
			catch (Exception ex)
			{
				rawInvites = new List<Invitation>();
				Debug.WriteLine(ex);
			}

			if (rawInvites != null && rawInvites.Count > 0)
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
		private void NewPersonButton_Clicked(object sender, EventArgs e)
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
		private void JoinPersonButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new SeeInvitations(rawInvites));
		}
	}
}