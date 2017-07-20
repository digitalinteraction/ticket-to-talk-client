using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Send invitation.
	/// </summary>
	public class SendInvitation : TrackedContentPage
	{

		private Entry email;
		private Picker group;
		private Button sendButton;
		private Person person;

		string[] userGroups = ProjectResource.groups;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SendInvitation"/> class.
		/// </summary>
		public SendInvitation(Person person)
		{

            TrackedName = "Send Invitation";

			this.Title = "Invite";
			this.person = person;

			var descriptionLabel = new Label
			{
				Text = "Enter the email address of who you want to contribute to this person.",
			};
            descriptionLabel.SetSubHeaderStyle();
            descriptionLabel.VerticalOptions = LayoutOptions.Start;

			email = new Entry
			{
				Placeholder = "Contributer's email",
				WidthRequest = Session.ScreenWidth * 0.8,
                Keyboard = Keyboard.Email
			};
            email.SetStyle();
            email.TextColor = ProjectResource.color_red;
			email.TextChanged += InputChanged;
            email.VerticalOptions = LayoutOptions.Start;

			var groupsLabel = new Label
			{
				Text = "Choose a user group.",
				Margin = new Thickness(0, 10, 0, 0),
			};
            groupsLabel.SetSubHeaderStyle();
            groupsLabel.VerticalOptions = LayoutOptions.Start;

			group = new Picker
			{
				Title = "Group",
				TextColor = ProjectResource.color_red,
				WidthRequest = Session.ScreenWidth * 0.8
			};
			for (int i = 0; i < userGroups.Length; i++)
			{
				group.Items.Add(userGroups[i]);
			}
			group.SelectedIndexChanged += InputChanged;

			sendButton = new Button
			{
				Text = "Invite",
				BorderRadius = 5,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				IsEnabled = false,
				WidthRequest = Session.ScreenWidth * 0.5
			};
            sendButton.SetStyle();
            sendButton.Margin = new Thickness(0, 0, 0, 0);
			sendButton.Clicked += SendButton_Clicked;

			var buttonStack = new StackLayout
			{
				Padding = new Thickness(20),
				VerticalOptions = LayoutOptions.EndAndExpand,
				Spacing = 0,
				Children =
				{
					sendButton
				}
			};

			var content = new StackLayout
			{
				Padding = new Thickness(20),
				Spacing = 10,
				Children =
				{
					descriptionLabel,
					email,
					groupsLabel,
					group,
				}
			};

			Content = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					content,
					buttonStack
				}
			};
		}

		/// <summary>
		/// When the button is clicked, make a request to the server.
		/// </summary>
		/// <returns>The button clicked.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void SendButton_Clicked(object sender, EventArgs e)
		{
			var userController = new UserController();
			var chosenGroup = userGroups[group.SelectedIndex];

			bool sent = false;

			try
			{
				sent = await userController.SendInviteToPerson(email.Text.ToLower(), chosenGroup, person.id);
			}
			catch (Exception ex)
			{
				await DisplayAlert("No Network", ex.Message, "OK");
			}

			if (sent)
			{
				await DisplayAlert("Invite", "Invitation sent to " + email.Text, "OK");
				await Navigation.PopAsync();
			}
			else
			{
				await DisplayAlert("Invite", "Invitation could not be sent to " + email.Text, "OK");
			}

		}

		/// <summary>
		/// Emails the text changed.
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void InputChanged(object sender, EventArgs e)
		{
			var v = (!string.IsNullOrEmpty(email.Text))
				&& group.SelectedIndex != -1;

			if (v)
			{
				sendButton.BackgroundColor = ProjectResource.color_red;
				sendButton.IsEnabled = true;
			}
			else
			{
				sendButton.BackgroundColor = ProjectResource.color_grey;
				sendButton.IsEnabled = false;
			}
		}
	}
}

