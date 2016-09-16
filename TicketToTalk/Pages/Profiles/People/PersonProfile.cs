﻿using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Person profile.
	/// </summary>
	public class PersonProfile : ContentPage
	{

		public static Person currentPerson;
		PersonController personController = new PersonController();
		public static PersonPivot pivot;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.PersonProfile"/> class.
		/// </summary>
		/// <param name="person">Person.</param>
		public PersonProfile(Person person)
		{
			currentPerson = person;

			Title = "Profile";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(editPerson)
			});

			person.displayString = personController.getDisplayString(person);

			var users = Task.Run(() => personController.getUsers(person.id)).Result;

			var nameLabel = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};
			nameLabel.SetBinding(Label.TextProperty, "name");
			nameLabel.BindingContext = currentPerson;

			PersonUserDB puDB = new PersonUserDB();
			var personUser = puDB.getRelationByUserAndPersonID(Session.activeUser.id, currentPerson.id);
			puDB.close();

			pivot = new PersonPivot();
			pivot.relation = personUser.relationship;
			currentPerson.personPivot = pivot;

			var relation = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_blue,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};
			relation.SetBinding(Label.TextProperty, "relation");
			relation.BindingContext = pivot;

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
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_blue,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};
			birthYearLabel.SetBinding(Label.TextProperty, "displayString");
			birthYearLabel.BindingContext = currentPerson;

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
				var label = new Label
				{
					Text = string.Format("{0} ({1})", u.name, u.pivot.user_type),
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
			notes.SetBinding(Label.TextProperty, "notes");
			notes.BindingContext = currentPerson;

			var button = new Button
			{
				Text = "Send an Invitation",
				BackgroundColor = ProjectResource.color_red,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
			};
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
		private async void editPerson()
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
						//// TODO: Move to controller.
						//Debug.WriteLine("PersonProfile: Person to be deleted.");
						//Debug.WriteLine("PersonProfile: Deleting person locally.");
						//personController.deletePersonLocally(currentPerson.id);
						//Debug.WriteLine("PersonProfile: Deleting person remotely.");
						//personController.deletePersonRemotely(currentPerson);

						//var ticketController = new TicketController();
						//var mediaController = new MediaController();
						//Debug.WriteLine("PersonProfile: Getting all of the person's tickets.");
						//var tickets = ticketController.getTicketsByPerson(currentPerson.id);
						//Debug.WriteLine("PersonProfile: Deleting ticket files.");
						//foreach (Ticket t in tickets)
						//{
						//	ticketController.deleteTicketLocally(t);
						//	mediaController.deleteFile(t.pathToFile);
						//}

						//var relation = personController.getRelation(currentPerson.id);
						//var personUserDB = new PersonUserDB();
						//personUserDB.DeleteRelation(relation.id);

						//Debug.WriteLine("PersonProfile: Removing person from views.");
						//AllProfiles.people.Remove(currentPerson);

						var deleted = await personController.destroyPerson(currentPerson);

						if (deleted)
						{
							if (Session.activePerson.id == currentPerson.id)
							{
								Session.activePerson = null;
								var navi = new NavigationPage(new SelectActivePerson());
								navi.setNavHeaders();
								//await Navigation.PushAsync(navi);
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

		/// <summary>
		/// Gets the users associated with this person.
		/// </summary>
		/// <returns>The users.</returns>
		//public async Task<List<User>> getUsers()
		//{
		//	IDictionary<string, string> parameters = new Dictionary<string, string>();
		//	parameters["token"] = Session.Token.val;
		//	parameters["person_id"] = currentPerson.id.ToString();
		//	string url = "people/getusers";

		//	// Send request for all users associated with the person
		//	Console.WriteLine("Sending request for all users associated with the person.");
		//	NetworkController net = new NetworkController();
		//	var jobject = await net.sendGetRequest(url, parameters);
		//	Console.WriteLine(jobject);

		//	var jusers = jobject.GetValue("users");
		//	var users = jusers.ToObject<User[]>();
		//	foreach (User u in users)
		//	{
		//		Console.WriteLine(u);
		//	}
		//	return new List<User>(users);
		//}
	}
}


