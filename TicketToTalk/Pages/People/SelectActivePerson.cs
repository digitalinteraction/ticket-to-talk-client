using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Select active person.
	/// </summary>
	public class SelectActivePerson : ContentPage
	{
		PersonController personController = new PersonController();
		ObservableCollection<Person> people = new ObservableCollection<Person>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.SelectActivePerson"/> class.
		/// </summary>
		public SelectActivePerson()
		{
			Padding = new Thickness(20);

			Title = "Select a Person";

			var label = new Label
			{
				Text = "Choose who you want to talk to.",
				TextColor = ProjectResource.color_dark,
				HorizontalTextAlignment = TextAlignment.Center
			};

			people = Task.Run(() => personController.getPeopleFromServer()).Result;
			Debug.WriteLine("SelectActivePerson: peopleLength - " + people.Count);
			foreach (Person p in people) 
			{
				//var imgS = personController.getPersonProfilePicture(p);
				p.imageSource = personController.getPersonProfilePicture(p);

				//if (p.pathToPhoto.StartsWith("storage", StringComparison.CurrentCulture)) 
				//{
				//	personController.downloadPersonProfilePicture(p);
				//}
				//Debug.WriteLine(p.pathToPhoto);
				//p.pathToPhoto = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), p.pathToPhoto);

				Debug.WriteLine("SelectActivePerson: Got image");

				p.relation = personController.getRelationship(p.id);
			}
			Debug.WriteLine("PersonController: Finished block");
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
		void PeopleListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			Person p = (Person)e.SelectedItem;
			Session.activePerson = p;
			Debug.WriteLine("Setting active person: "+ p);

			App.Current.MainPage = new RootPage();
		}
	}
}


