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
	public class PersonProfile : ContentPage
	{

		public static Person currentPerson;
		PersonController personController = new PersonController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.PersonProfile"/> class.
		/// </summary>
		/// <param name="person">Person.</param>
		public PersonProfile(Person person)
		{
			currentPerson = person;

			// TODO: Fix name 
			this.SetBinding(TitleProperty, currentPerson.name);
			this.BindingContext = currentPerson;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "?",
				Icon = "info_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(editPerson)
			});

			person.displayString = personController.getDisplayString(person);

			var users = Task.Run(() => getUsers()).Result;

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

			var relation = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_blue,
				FontSize = 14,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};
			relation.SetBinding(Label.TextProperty, "relationship");
			relation.BindingContext = personUser;

			var detailsHeader = new Label
			{
				Text = "Details",
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = ProjectResource.color_dark,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Margin = new Thickness(0,10,0,0)
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

			var imageStack = new StackLayout()
			{
				Spacing = 0,
				Children = 
				{
					new PersonProfileImage(person)
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
				Padding = new Thickness(0,0,0,20),
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
		async void editPerson()
		{
			var action = await DisplayActionSheet("Edit Person", "Cancel", "Delete Person", "Edit Person");
			Debug.WriteLine("PersonProfile: Action sheet selection - " + action);

			switch(action) 
			{
				case ("Delete Person"):
					var confirm = await DisplayAlert("Delete " + currentPerson.name, "Are you sure you want to delete " + currentPerson.name + "'s profile?", "Yes", "Cancel");
					if (confirm) 
					{
						Debug.WriteLine("PersonProfile: Person to be deleted.");
						Debug.WriteLine("PersonProfile: Deleting person locally.");
						personController.deletePersonLocally(currentPerson.id);
						Debug.WriteLine("PersonProfile: Deleting person remotely.");
						personController.deletePersonRemotely(currentPerson);

						var ticketController = new TicketController();
						var mediaController = new MediaController();
						Debug.WriteLine("PersonProfile: Getting all of the person's tickets.");
						var tickets = ticketController.getTicketsByPerson(currentPerson.id);
						Debug.WriteLine("PersonProfile: Deleting ticket files.");
						foreach (Ticket t in tickets) 
						{
							ticketController.deleteTicketLocally(t);
							mediaController.deleteFile(t.pathToFile);
						}

						var relation = personController.getRelation(currentPerson.id);
						var personUserDB = new PersonUserDB();
						personUserDB.DeleteRelation(relation.id);

						Debug.WriteLine("PersonProfile: Removing person from views.");
						AllProfiles.people.Remove(currentPerson);
						if (Session.activePerson.id == currentPerson.id)
						{
							Session.activePerson = null;
							await Navigation.PushAsync(new SelectActivePerson());
						}
						else 
						{
							await Navigation.PopAsync();
						}
					}
					break;
				case ("Edit Person"):
					var nav = new NavigationPage(new AddPerson(currentPerson));
					nav.BarBackgroundColor = ProjectResource.color_blue;
					nav.BarTextColor = ProjectResource.color_white;

					await Navigation.PushModalAsync(nav);
					break;
			};
		}

		/// <summary>
		/// Gets the users associated with this person.
		/// </summary>
		/// <returns>The users.</returns>
		public async Task<List<User>> getUsers() 
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;
			parameters["person_id"] = currentPerson.id.ToString();
			string url = "people/getusers";

			// Send request for all users associated with the person
			Console.WriteLine("Sending request for all users associated with the person.");
			NetworkController net = new NetworkController();
			var jobject = await net.sendGetRequest(url, parameters);
			Console.WriteLine(jobject);

			var jusers = jobject.GetValue("users");
			var users = jusers.ToObject<User[]>();
			foreach (User u in users) 
			{
				Console.WriteLine(u);
			}
			return new List<User>(users);
		}
	}
}


