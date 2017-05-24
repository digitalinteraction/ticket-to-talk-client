using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Select active person.
	/// </summary>
	public class SelectActivePerson : TrackedContentPage
	{
		private PersonController personController = new PersonController();
		private ObservableCollection<Person> people = new ObservableCollection<Person>();
		public static bool promptShown = false;
		private ProgressSpinner indicator;
		StackLayout compRegLayout;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectActivePerson"/> class.
		/// </summary>
		public SelectActivePerson()
		{

            TrackedName = "Select Active Person";

			indicator = new ProgressSpinner(this, ProjectResource.color_grey_transparent);

			Padding = new Thickness(20);

			Title = "Select a Person";

			var label = new Label
			{
				Text = "Choose who you want to talk to.",
				TextColor = ProjectResource.color_dark,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var peopleListView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(PersonCell)),
				SeparatorColor = ProjectResource.color_grey,
				HasUnevenRows = true,
				RowHeight = 90
			};
			peopleListView.SetBinding(ListView.ItemsSourceProperty, ".");
			peopleListView.BindingContext = people;
			peopleListView.ItemSelected += PeopleListView_ItemSelected;

			// Layout if the user does not contribute to any people.
			var incompleteRegistration = new Label
			{
				Text = "You have not yet completed the registration process. Select next to continue",
				TextColor = ProjectResource.color_dark,
				FontSize = 16,
				HorizontalTextAlignment = TextAlignment.Center,
			};

			var continueButton = new Button
			{
				Text = "Next",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_red,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				VerticalOptions = LayoutOptions.End
			};
			continueButton.Clicked += (sender, e) =>
			{

				var t = new AddNewPersonPrompt(false);
				Application.Current.MainPage = t;
				AllProfiles.promptShown = true;
			};

			compRegLayout = new StackLayout
			{
				Spacing = 10,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(20),
				Children =
				{
					incompleteRegistration,
					continueButton
				}
			};

			Content = new StackLayout
			{
				Children = {
					indicator,
					label,
					peopleListView
				}
			};
		}

		/// <summary>
		/// Peoples the list view item selected.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void PeopleListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var p = (Person)e.SelectedItem;
			Session.activePerson = p;

			if (Session.activeUser.firstLogin && !AllProfiles.promptShown)
			{
				var canSkip = true;
				if (people.Count == 0)
				{
					canSkip = false;
				}
				var t = new AddNewPersonPrompt(canSkip);
				Application.Current.MainPage = t;
				AllProfiles.promptShown = true;
			}
			else 
			{
				Application.Current.MainPage = new RootPage();
			}
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();


		}

		public async Task<bool> SetUpSelectActivePerson() 
		{
			// Try and get people from the server
			try
			{
				//var task = Task.Run(() => personController.GetPeopleFromServer());
				var returned_people = new ObservableCollection<Person>();

				try
				{
					//returned_people = task.Result;
					returned_people = await Task.Run(() => personController.GetPeopleFromServer());
				}
				catch (Exception ex)
				{
					throw ex;
				}
				foreach (Person p in returned_people)
				{
					personController.AddPersonLocally(p);
					p.imageSource = await Task.Run(() => personController.GetPersonProfilePicture(p));
					p.relation = personController.GetRelationship(p.id);
					people.Add(p);
				}
			}

			// If network not available, use local records.
			catch (Exception ex)
			{
				var stored_people = new ObservableCollection<Person>(personController.GetPeople());

				foreach (Person p in stored_people)
				{
					p.imageSource = await Task.Run(() => personController.GetPersonProfilePicture(p));
					p.relation = personController.GetRelationship(p.id);
					people.Add(p);
				}

				await DisplayAlert("No Network", ex.Message, "Dismiss");
			}

			if (people.Count == 0)
			{
				Content = compRegLayout;
			}

			return true;
		}
	}
}


