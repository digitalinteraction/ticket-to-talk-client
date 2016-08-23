using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.IO;
using Plugin.Media;
using System.Diagnostics;
using TicketToTalk;
using Plugin.Media.Abstractions;
using ImageCircle.Forms.Plugin.Abstractions;

namespace TicketToTalk
{
	/// <summary>
	/// A page to add a person.
	/// </summary>
	public class AddPerson : ContentPage
	{

		Entry name;
		Image personImage;
		Entry birthPlaceEntry;
		Entry town_city;
		MediaFile file;
		Editor notesEditor;

		string[] relations = 
		{
			"Father",
			"Mother",
			"Grandfather",
			"Grandmother",
			"Uncle",
			"Aunt",
			"Friend",
		};

		Picker yearPicker;
		Picker relationPicker;
		Button savePersonButton;

		/// <summary>
		/// Creates an instance of an add person view.
		/// </summary>
		public AddPerson()
		{
			Title = "New Person";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			personImage = new CircleImage
			{
				BorderColor = ProjectResource.color_red,
				BorderThickness = 2,
				HeightRequest = (Session.ScreenWidth * 0.8),
				WidthRequest = (Session.ScreenWidth * 0.8),
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(20),
			};
			personImage.Source = "person_placeholder.png";
			personImage.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(onPlaceholderTap) });

			var nameLabel = new Label
			{
				Text = "What is their name?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0,10,0,2)
			};

			name = new Entry 
			{
				Placeholder = "Name",
				TextColor = ProjectResource.color_red
			};
			name.TextChanged += Entry_TextChanged;

			var birthPlace = new Label 
			{
				Text = "Where were they born?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			birthPlaceEntry = new Entry
			{
				Placeholder = "Town or City",
				TextColor = ProjectResource.color_red
			};
			birthPlaceEntry.TextChanged += Entry_TextChanged;

			var DOBLabel = new Label 
			{
				Text = "When were they born (approximately)",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			yearPicker = new Picker
			{
				Title = "Year",
				TextColor = ProjectResource.color_red
			};
			yearPicker.SelectedIndexChanged += Entry_TextChanged;
			for (int i = DateTime.Now.Year - 100; i < DateTime.Now.Year; i++)
			{
				yearPicker.Items.Add((i + 1).ToString());
			}

			var relationLabel = new Label 
			{
				Text = "How do you them?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			relationPicker = new Picker
			{
				Title = "Relation",
				TextColor = ProjectResource.color_red
			};
			relationPicker.SelectedIndexChanged += Entry_TextChanged;
			foreach (string r in relations) 
			{
				relationPicker.Items.Add(r);
			}

			Label area = new Label 
			{
				Text = "Where have they lived most of their life?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			town_city = new Entry { 
				Placeholder = "Town or City",
				TextColor = ProjectResource.color_red
			};
			town_city.TextChanged += Entry_TextChanged;

			var notesLabel = new Label 
			{
				Text = "Write some notes about their condition",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			notesEditor = new Editor 
			{
				TextColor = ProjectResource.color_red,
				Text = "Add some notes..."
			};
			notesEditor.TextChanged += Entry_TextChanged;

			var imageStack = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					personImage
				}
			};

			var headerStack = new StackLayout
			{
				Padding = new Thickness(10),
				BackgroundColor = ProjectResource.color_red,
				Children =
				{
					new Label
					{
						Text = "Details",
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						TextColor = ProjectResource.color_white,
					}
				}
			};

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(20, 10, 20, 20),
				Spacing = 0,
				Children =
				{
					nameLabel,
					name,
					DOBLabel,
					yearPicker,
					birthPlace,
					birthPlaceEntry,
					relationLabel,
					relationPicker,
					area,
					town_city,
					notesLabel,
					notesEditor
				}
			};

			savePersonButton = new Button
			{
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = (Session.ScreenWidth * 0.5),
				FontAttributes = FontAttributes.Bold,
				Text = "Save",
				IsEnabled = false,
				Margin = new Thickness(0, 0, 0, 10),
			};
			savePersonButton.Clicked += savePerson;

			var buttonStack = new StackLayout
			{
				Spacing = 0,
				//BackgroundColor = ProjectResource.color_blue,
				Children =
				{
					savePersonButton
				}
			};

			var contentStack = new StackLayout
			{
				Spacing = 0,
				Children = {
					imageStack,
					headerStack,
					detailsStack,
					buttonStack
				}
			};
			Content = new ScrollView
			{
				Content = contentStack
			};
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		void cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// On placeholder tap, select image.
		/// </summary>
		/// <returns>The placeholder tap.</returns>
		async void onPlaceholderTap()
		{
			var action = await DisplayActionSheet("Choose Photo Type", "Cancel", null, "Take a Photo", "Select a Photo From Library");
			MediaFile file = null;
			switch (action)
			{
				case ("Take a Photo"):
					file = await CameraController.TakePicture("temp_profile");
					break;
				case ("Select a Photo From Library"):
					file = await CameraController.SelectPicture();
					break;
			}

			if (file != null) 
			{
				personImage.Source = ImageSource.FromFile(file.Path);
				this.file = file;
			}
		}

		/// <summary>
		/// On Entry text change.
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = (!String.IsNullOrEmpty(name.Text))
				&& (!String.IsNullOrEmpty(birthPlaceEntry.Text))
				&& (!String.IsNullOrEmpty(town_city.Text))
				&& (!String.IsNullOrEmpty(notesEditor.Text))
				&& (relationPicker.SelectedIndex != -1)
				&& (yearPicker.SelectedIndex != -1);

			if (entriesNotNull)
			{
				savePersonButton.BackgroundColor = ProjectResource.color_blue;
				savePersonButton.IsEnabled = true;
			}
			else
			{
				savePersonButton.BackgroundColor = ProjectResource.color_grey;
				savePersonButton.IsEnabled = false;
			}
		}

		//void YearPicker_SelectedIndexChanged(object sender, EventArgs e)
		//{

		//}

		/// <summary>
		/// Saves the person to the database.
		/// </summary>
		// TODO check fields are not null.
		public async void savePerson(Object sender, EventArgs ea)
		{
			byte[] image = null;
			if (file != null)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					file.GetStream().CopyTo(ms);
					Debug.WriteLine(ms.ToArray().Length / 1000000 + "MB");
					image = ms.ToArray();
				}
			}

			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["token"] = Session.Token.val;
			parameters["name"] = name.Text;
			parameters["birthYear"] = (DateTime.Now.Year - 99 + yearPicker.SelectedIndex).ToString();
			parameters["birthPlace"] = birthPlaceEntry.Text;
			parameters["townCity"] = town_city.Text;
			parameters["notes"] = notesEditor.Text;
			parameters["relation"] = relations[relationPicker.SelectedIndex];
			parameters["image"] = image;

			NetworkController net = new NetworkController();
			var jobject = await net.sendGenericPostRequest("people/store", parameters);
			Debug.WriteLine(jobject);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("person");
				var stored_person = jtoken.ToObject<Person>();

				var personController = new PersonController();

				//var personDB = new PersonDB();
				var fileName = "p_" + stored_person.id + ".jpg";
				stored_person.pathToPhoto = fileName;
				personController.addPersonLocally(stored_person);
				Session.activePerson = stored_person;

				MediaController.writeImageToFile(fileName, image);

				var personUserDB = new PersonUserDB();
				var pu = new PersonUser
				{
					user_id = Session.activeUser.id,
					person_id = stored_person.id,
					relationship = relations[relationPicker.SelectedIndex],
					user_type = "Admin"
				};
				personUserDB.AddPersonUser(pu);

				personController.addStockPeriods(stored_person);

				await Navigation.PushAsync(new RootPage());
			}
			else
			{
				await DisplayAlert("Add Person", "Person could not be added.", "OK");
			}
		}
	}
}