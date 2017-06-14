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
	public class SeeInvite : TrackedContentPage
	{
		private Person person;
		private Button button;
		private PersonController personController = new PersonController();
		private Button rejectButton;
		private Picker relation;

		public static bool isInTutorial = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SeeInvite"/> class.
		/// </summary>
		/// <param name="person">Person.</param>
		public SeeInvite(Invitation invitation)
		{

            TrackedName = "See Invite";

			person = invitation.person;
			Title = person.name;

            Padding = ProjectResource.Padding;

			var users = Task.Run(() => personController.GetUsers(person.id)).Result;

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
			profilePic.SetBinding(Image.SourceProperty, "imageSource");
			profilePic.BindingContext = invitation;

			var nameLabel = new Label
			{
				Text = person.name,
			};
            nameLabel.SetHeaderStyle();

			var detailsHeader = new Label
			{
				Text = "Details",
				Margin = new Thickness(0, 10, 0, 0)
			};
            detailsHeader.SetSubHeaderStyle();

			var birthYearLabel = new Label
			{
				Text = string.Format("Born in {0}, {1}", person.birthPlace, person.birthYear),
			};
            birthYearLabel.SetBodyStyle();
            birthYearLabel.TextColor = ProjectResource.color_red;

			var associatesLabel = new Label
			{
				Text = "Contributors to This Person:",
				Margin = new Thickness(0, 0, 0, 0)
			};
            associatesLabel.SetSubHeaderStyle();

			var viewersStack = new StackLayout
			{
				Spacing = 4
			};

			foreach (User u in users)
			{
				var label = new Label
				{
					Text = string.Format("{0} ({1})", u.name, u.pivot.user_type),
				};
                label.SetBodyStyle();
                label.TextColor = ProjectResource.color_red;
				viewersStack.Children.Add(label);
			}

			var notesLabel = new Label
			{
				Text = "Notes on Their Condition",
				Margin = new Thickness(0, 10, 0, 0)
			};
            notesLabel.SetSubHeaderStyle();

			var notes = new Label
			{
				Text = person.notes,
				WidthRequest = Session.ScreenWidth * 0.75,
			};
            notes.SetBodyStyle();
            notes.TextColor = ProjectResource.color_red;

			button = new Button
			{
				Text = "Accept Invitation",
				BackgroundColor = ProjectResource.color_grey,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				IsEnabled = false
			};
            button.SetStyle();
            button.Margin = new Thickness(0, 0, 0, 0);
			button.Clicked += async (sender, e) =>
			{

				var userController = new UserController();
				var r = ProjectResource.relations[relation.SelectedIndex];
				await userController.AcceptInvitation(invitation, r);

				if (new PersonController().GetPerson(invitation.person.id) == null)
				{
					new PersonController().AddPersonLocally(invitation.person);
				}

				SeeInvitations.invitations.Remove(invitation);

				AllProfiles.people.Add(invitation.person);

				Session.activePerson = invitation.person;

				if (isInTutorial)
				{
					Application.Current.MainPage = new AddTicketPrompt();
					isInTutorial = false;
				}
				else
				{
					Application.Current.MainPage = new RootPage();
				}
			};

			rejectButton = new Button
			{
				Text = "Reject Invitation",
				BackgroundColor = ProjectResource.color_red,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
			};
            rejectButton.SetStyle();
            rejectButton.Margin = new Thickness(0, 0, 0, 0);
			rejectButton.Clicked += async (sender, e) =>
			{
				var userController = new UserController();
				var rejected = await userController.RejectInvitation(invitation.person);

				if (rejected)
				{
					SeeInvitations.invitations.Remove(invitation);
					await Navigation.PopAsync();
				}
			};

			var relationLabel = new Label
			{
				Text = "Select a relation to accept the invitation.",
				Margin = new Thickness(0, 10, 0, 0)
			};
            relationLabel.SetSubHeaderStyle();

			relation = new Picker
			{
				Title = "Relation",
				TextColor = ProjectResource.color_red,
				//HorizontalOptions = LayoutOptions.CenterAndExpand,
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