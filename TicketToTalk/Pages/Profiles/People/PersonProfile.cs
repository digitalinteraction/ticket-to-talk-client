using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Person profile.
	/// </summary>
	public class PersonProfile : TrackedContentPage
	{

		public static Person currentPerson;
		private PersonController personController = new PersonController();
		public static PersonPivot pivot;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.PersonProfile"/> class.
		/// </summary>
		/// <param name="person">Person.</param>
		public PersonProfile(Person person)
		{

            Padding = ProjectResource.Padding;

            TrackedName = "Person Profile";

			currentPerson = person;

			Title = "Profile";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(EditPerson)
			});

			person.displayString = personController.GetDisplayString(person);

			var task = Task.Run(() => personController.GetUsers(person.id));
			List<User> users = null;

			try
			{
				users = task.Result;
			}
			catch (Exception ex)
			{
				users = new List<User>();
				Debug.WriteLine(ex.Message);
			}

			var nameLabel = new Label
			{
			};
            nameLabel.SetHeaderStyle();
			nameLabel.SetBinding(Label.TextProperty, "name");
			nameLabel.BindingContext = currentPerson;

			var personUser = personController.GetUserPersonRelation(Session.activeUser.id, currentPerson.id);

			pivot = new PersonPivot();
			pivot.relation = personUser.relationship;
			currentPerson.personPivot = pivot;

			var relation = new Label
			{
			};
            relation.SetSubHeaderStyle();
            relation.TextColor = ProjectResource.color_red;
            relation.HorizontalTextAlignment = TextAlignment.Center;
            relation.HorizontalOptions = LayoutOptions.CenterAndExpand;
			relation.SetBinding(Label.TextProperty, "relation");
			relation.BindingContext = pivot;

			var detailsHeader = new Label
			{
				Text = "Details",
				Margin = new Thickness(0, 10, 0, 0)
			};
            detailsHeader.SetSubHeaderStyle();

			var birthYearLabel = new Label
			{
			};
            birthYearLabel.SetBodyStyle();
            birthYearLabel.TextColor = ProjectResource.color_red;
            birthYearLabel.SetBinding(Label.TextProperty, "displayString");
			birthYearLabel.BindingContext = currentPerson;

			var associatesLabel = new Label
			{
				Text = "Contributors to This Person:",
				Margin = new Thickness(0, 10, 0, 0)
			};
            associatesLabel.SetSubHeaderStyle();

			var viewersStack = new StackLayout
			{
				Spacing = 4
			};

			if (users.Count == 0) 
			{
                var l = new Label 
                {
					Text = "Contributors Unavailable",
                };
                l.SetBodyStyle();
                l.TextColor = ProjectResource.color_red;
				viewersStack.Children.Add(l);
			}

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
			};
            notes.SetBodyStyle();
            notes.TextColor = ProjectResource.color_red;
			notes.SetBinding(Label.TextProperty, "notes");
			notes.BindingContext = currentPerson;

			var button = new Button
			{
				Text = "Invite Contributor",
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
			};
            button.SetStyle();
            button.Margin = new Thickness(0, 0, 0, 0);
			button.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new SendInvitation(person));
			};

			var profileImage = new PersonProfileImage(currentPerson);
			profileImage.profilePic.SetBinding(Image.SourceProperty, "imageSource");
			profileImage.BindingContext = currentPerson;
			var imageStack = new StackLayout()
			{
				Spacing = 0,
				Children =
				{
					profileImage
				}
			};

			var infStack = new StackLayout()
			{
				Spacing = 4,
				Children =
				{
					nameLabel,
					relation,
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
					button,
					notesLabel,
					notes,
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};
		}

		/// <summary>
		/// Edits the person.
		/// </summary>
		private async void EditPerson()
		{

			// Display the action sheet.
			var action = string.Empty;
			if (currentPerson.admin_id == Session.activeUser.id)
			{
				action = await DisplayActionSheet("Edit Person", "Cancel", "Delete Person", "Edit Person");
			}
			else
			{
				action = await DisplayActionSheet("Edit Person", "Cancel", "Delete Person");
			}

			// Handle the action.
			switch (action)
			{
				case ("Delete Person"):
					var confirm = await DisplayAlert("Delete " + currentPerson.name, "Are you sure you want to delete " + currentPerson.name + "'s profile?", "Yes", "Cancel");
					if (confirm)
					{

						bool deleted = false;

						try
						{
							deleted = await personController.DestroyPerson(currentPerson);
						}
						catch (NoNetworkException ex)
						{
							await DisplayAlert("No Network", ex.Message, "Dismiss");
						}


						if (deleted)
						{
							if (Session.activePerson.id == currentPerson.id)
							{
								Session.activePerson = null;
								var navi = new NavigationPage(new SelectActivePerson());
								navi.SetNavHeaders();
								Application.Current.MainPage = navi;
							}
							else
							{
								await Navigation.PopAsync();
							}
						}
						else
						{
							await DisplayAlert("Delete Person", "Person could not be deleted.", "OK");
						}

					}
					break;
				case ("Edit Person"):
					var nav = new NavigationPage(new AddPerson(currentPerson));
					nav.BarBackgroundColor = ProjectResource.color_blue;
					nav.BarTextColor = ProjectResource.color_white;

					await Navigation.PushModalAsync(nav);
					break;
			}
		}
	}
}


