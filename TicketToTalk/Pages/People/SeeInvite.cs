using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// See invite.
	/// </summary>
	public class SeeInvite : ContentPage
	{
		public Person person;
		Button button;
		PersonController personController = new PersonController();
		Button rejectButton;
		Picker relation;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SeeInvite"/> class.
		/// </summary>
		/// <param name="person">Person.</param>
		public SeeInvite(Invitation invitation)
		{
			this.person = invitation.person;
			this.Title = person.name;

			var users = Task.Run(() => personController.getUsers(person.id)).Result;

			var profilePic = new CircleImage
			{
				BorderColor = ProjectResource.color_blue,
				BorderThickness = 2,
				HeightRequest = 275,
				WidthRequest = 275,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(20),
				BackgroundColor = ProjectResource.color_grey
			};

				var rawBytes = MediaController.readBytesFromFile(person.pathToPhoto);
				profilePic.Source = ImageSource.FromStream(() => new MemoryStream(rawBytes));

			var nameLabel = new Label
			{
				Text = person.name,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				//Margin = new Thickness(0, 0, 0, 5)
			};

			var detailsHeader = new Label
			{
				Text = "Details",
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var birthYearLabel = new Label
			{
				Text = String.Format("Born in {0}, {1}", person.birthPlace, person.birthYear),
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_blue,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				//Margin = new Thickness(0, 0, 0, 10)
			};

			var associatesLabel = new Label
			{
				Text = "Contributors to This Person:",
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var viewersStack = new StackLayout
			{
				Spacing = 4
			};

			foreach (User u in users)
			{
				//var userDetail = u.name + "(" + u.pivot.user_type + ")";
				var label = new Label
				{
					Text = String.Format("{0} ({1})", u.name, u.pivot.user_type),
					HorizontalTextAlignment = TextAlignment.Center,
					TextColor = ProjectResource.color_blue,
					FontSize = 14,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				};
				viewersStack.Children.Add(label);
			}

			var notesLabel = new Label
			{
				Text = "Notes on Their Condition",
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var notes = new Label
			{
				Text = person.notes,
				WidthRequest = Session.ScreenWidth * 0.75,
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};

			button = new Button
			{
				Text = "Accept Invitation",
				BackgroundColor = ProjectResource.color_grey,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				IsEnabled = false
			};
			button.Clicked += async (sender, e) =>
			{
				
				var userController = new UserController();
				var r = ProjectResource.relations[relation.SelectedIndex];
				await userController.acceptInvitation(invitation, r);

				Debug.WriteLine("SeeInvite: accepting person - " + person);

				var personController = new PersonController();
				if (personController.getPerson(invitation.person.id) == null) 
				{
					personController.addPersonLocally(invitation.person);	
				}

				SeeInvitations.invitations.Remove(invitation);

				AllProfiles.people.Add(invitation.person);

				Session.activePerson = invitation.person;
				App.Current.MainPage = new RootPage();
			};

			rejectButton = new Button
			{
				Text = "Reject Invitation",
				BackgroundColor = ProjectResource.color_red,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
			};
			rejectButton.Clicked += async (sender, e) => 
			{
				var userController = new UserController();
				var rejected = await userController.rejectInvitation(invitation.person);

				if (rejected) 
				{
					SeeInvitations.invitations.Remove(invitation);
					await Navigation.PopAsync();
				}
			};

			var relationLabel = new Label 
			{
				Text = "Select a relation to accept the invitation.",
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0, 10, 0, 0)
			};

			relation = new Picker 
			{
				Title = "Relation",
				TextColor = ProjectResource.color_red,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.8
			};
			foreach (string s in ProjectResource.relations) 
			{
				relation.Items.Add(s);
			}
			relation.SelectedIndexChanged += (sender, e) => 
			{
				if (relation.SelectedIndex != -1)
				{
					button.BackgroundColor = ProjectResource.color_blue;
					button.IsEnabled = true;
				}
				else 
				{
					button.BackgroundColor = ProjectResource.color_grey;
					button.IsEnabled = false;
				}
			};

			var imageStack = new StackLayout()
			{
				Spacing = 0,
				Children =
				{
					profilePic
				}
			};

			var infStack = new StackLayout()
			{
				Spacing = 4,
				Children =
				{
					nameLabel,
					detailsHeader,
					birthYearLabel,
				}
			};

			var stack = new StackLayout
			{
				Spacing = 12,
				Padding = new Thickness(0, 0, 0, 20),
				Children = {
					imageStack,
					infStack,
					associatesLabel,
					viewersStack,
					notesLabel,
					notes,
					relationLabel,
					relation,
					button,
					rejectButton
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};

		}
	}
}