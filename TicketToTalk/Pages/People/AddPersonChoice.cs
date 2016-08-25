using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TicketToTalk;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class AddPersonChoice : ContentPage
	{
		UserController userController = new UserController();
		PersonController personController = new PersonController();
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
			foreach (Invitation invite in rawInvites)
			{
				invite.pathToPhoto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
														 personController.downloadPersonProfilePictureForInvite(invite.person));
				Debug.WriteLine("PersonPath: " + invite.person.pathToPhoto);
				invite.person_name = invite.person.name;
			}

			if (rawInvites.Count > 0 ) 
			{
				joinPersonButton.Text = String.Format("See Invitations({0})", rawInvites.Count);
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

		void NewPersonButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new AddPerson(null));
			Navigation.RemovePage(this);
		}

		void JoinPersonButton_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new SeeInvitations(rawInvites));
			Navigation.RemovePage(this);
		}
	}
}