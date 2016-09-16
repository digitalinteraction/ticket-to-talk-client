using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Send invitation.
	/// </summary>
	public class SendInvitation : ContentPage
	{

		Entry email;
		Picker group;
		Button sendButton;
		Person person;

		string[] userGroups = ProjectResource.groups;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SendInvitation"/> class.
		/// </summary>
		public SendInvitation(Person person)
		{
			this.Title = "Invite";
			this.person = person;

			var descriptionLabel = new Label
			{
				Text = "Enter the email address of who you want to contribute to this person.",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Center
			};

			email = new Entry
			{
				Placeholder = "Contributer's email",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_red,
				WidthRequest = Session.ScreenWidth * 0.8
			};
			email.TextChanged += InputChanged;

			var groupsLabel = new Label
			{
				Text = "Choose a user group.",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HorizontalTextAlignment = TextAlignment.Center,
				Margin = new Thickness(0, 10, 0, 0),
			};

			group = new Picker
			{
				Title = "Group",
				TextColor = ProjectResource.color_red,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
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
		async void SendButton_Clicked(object sender, EventArgs e)
		{
			var userController = new UserController();
			var chosenGroup = userGroups[group.SelectedIndex];

			var sent = await userController.sendInviteToPerson(email.Text.ToLower(), chosenGroup, person.id);

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
		void InputChanged(object sender, EventArgs e)
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

