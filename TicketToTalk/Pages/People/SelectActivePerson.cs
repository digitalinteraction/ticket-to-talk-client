﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Select active person.
	/// </summary>
	public class SelectActivePerson : ContentPage
	{
		private PersonController personController = new PersonController();
		private ObservableCollection<Person> people = new ObservableCollection<Person>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectActivePerson"/> class.
		/// </summary>
		public SelectActivePerson()
		{

			try
			{
				people = Task.Run(() => personController.GetPeopleFromServer()).Result;
			}
			catch (NoNetworkException ex)
			{
				people = new ObservableCollection<Person>(personController.GetPeople());
				DisplayAlert("No Network", ex.Message, "Dismiss");
			}


			foreach (Person p in people)
			{
				personController.AddPersonLocally(p);
				p.imageSource = Task.Run(() => personController.GetPersonProfilePicture(p)).Result;
				p.relation = personController.GetRelationship(p.id);
			}

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
				Navigation.PushAsync(new AllProfiles());
				Navigation.RemovePage(this);
			};

			var compRegLayout = new StackLayout
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

			if (people.Count > 0)
			{
				Content = new StackLayout
				{
					Children = {
					label,
					peopleListView
				}
				};
			}
			else
			{
				Content = compRegLayout;
			}
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

			Application.Current.MainPage = new RootPage();
		}
	}
}


