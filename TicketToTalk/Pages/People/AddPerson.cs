﻿using System;
using Xamarin.Forms;
using System.IO;
using System.Diagnostics;
using Plugin.Media.Abstractions;
using ImageCircle.Forms.Plugin.Abstractions;

namespace TicketToTalk
{

	/// <summary>
	/// A page to add a person.
	/// </summary>
	public class AddPerson : TrackedContentPage
    {
		private PersonController personController = new PersonController();
		public static bool isInTutorial = false;

		private Entry name;
		private Image personImage;
		private Entry birthPlaceEntry;
		private Entry town_city;
		private MediaFile file;
		private Editor notesEditor;

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

		private Picker yearPicker;
		private Picker relationPicker;
		private Button savePersonButton;
		private Person person;
        private Entry birthYear;

        /// <summary>
        /// Creates an instance of an add person view.
        /// </summary>
        public AddPerson(Person person)
		{

            TrackedName = "Add Person";

			this.person = person;
			Title = "New Person";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
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
			if (person != null)
			{
				byte[] pic = MediaController.ReadBytesFromFile(person.pathToPhoto);
				personImage.Source = ImageSource.FromStream(() => new MemoryStream(pic));
			}
			else
			{
				personImage.Source = "person_placeholder.png";
			}
			personImage.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(OnPlaceholderTap) });

			var nameLabel = new Label
			{
				Text = "What is their name?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};
            nameLabel.SetSubHeaderStyle();

			name = new Entry
			{
				Placeholder = "Name"
			};
            name.SetStyle();
            name.TextColor = ProjectResource.color_red;
			name.TextChanged += Entry_TextChanged;

			var birthPlace = new Label
			{
				Text = "Where were they born?",
				Margin = new Thickness(0, 10, 0, 2)
			};
            birthPlace.SetSubHeaderStyle();

			birthPlaceEntry = new Entry
			{
				Placeholder = "Town or City",
				TextColor = ProjectResource.color_red
			};
            birthPlaceEntry.SetStyle();
			birthPlaceEntry.TextChanged += Entry_TextChanged;

			var DOBLabel = new Label
			{
				Text = "When were they born (approximately)",
				Margin = new Thickness(0, 10, 0, 2)
			};
            DOBLabel.SetSubHeaderStyle();

            birthYear = new Entry 
            {
				Placeholder = "1920",
                Keyboard = Keyboard.Numeric
            };
            birthYear.SetStyle();
            birthYear.TextColor = ProjectResource.color_red;
            birthYear.TextChanged += Entry_TextChanged;

			var relationLabel = new Label
			{
				Text = "How do you know them?",
				Margin = new Thickness(0, 10, 0, 2)
			};
            relationLabel.SetSubHeaderStyle();

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

			var area = new Label
			{
				Text = "Where have they lived most of their life?",
				Margin = new Thickness(0, 10, 0, 2)
			};
            area.SetSubHeaderStyle();

			town_city = new Entry
			{
				Placeholder = "Town or City"
			};
            town_city.SetStyle();
            town_city.TextColor = ProjectResource.color_red;
			town_city.TextChanged += Entry_TextChanged;

			var notesLabel = new Label
			{
				Text = "Write some notes about their condition",
				Margin = new Thickness(0, 10, 0, 2)
			};
            notesLabel.SetSubHeaderStyle();

			notesEditor = new Editor
			{
				Text = "Add some notes..."
			};
            notesEditor.SetStyle();
            notesEditor.TextColor = ProjectResource.color_red;
			notesEditor.TextChanged += Entry_TextChanged;
            notesEditor.HeightRequest = 100;

			var imageStack = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					personImage
				}
			};

            var header = new Label
            {
				Text = "Details"
            };
            header.SetHeaderStyle();

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(20, 10, 20, 20),
				Spacing = 0,
				Children =
				{
					nameLabel,
					name,
					DOBLabel,
                    birthYear,
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
				Text = "Save",
				IsEnabled = false,
			};
            savePersonButton.SetStyle();
			if (person != null)
			{
				savePersonButton.Clicked += SavePersonChanges;
			}
			else
			{
				savePersonButton.Clicked += SavePerson;
			}

			var buttonStack = new StackLayout
			{
				Spacing = 0,
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
					header,
					detailsStack,
					buttonStack
				}
			};

			if (person != null)
			{
				Title = "Edit Person";

				var personUser = personController.GetUserPersonRelation(Session.activeUser.id, person.id);

				var ridx = 0;
				for (int i = 0; i < ProjectResource.relations.Length; i++)
				{
					if (ProjectResource.relations[i].Equals(personUser.relationship))
					{
						ridx = i;
					}
				}

				relationPicker.SelectedIndex = ridx;
				name.Text = person.name;
				birthPlaceEntry.Text = person.birthPlace;
				notesEditor.Text = person.notes;
				town_city.Text = person.area;
				yearPicker.SelectedIndex = int.Parse(person.birthYear) - (DateTime.Now.Year - 99);
				savePersonButton.Text = "Save Changes";
			}

			Content = new ScrollView
			{
				Content = contentStack
			};
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		private void Cancel()
		{
			Navigation.PopModalAsync();


		}

		/// <summary>
		/// On placeholder tap, select image.
		/// </summary>
		/// <returns>The placeholder tap.</returns>
		private async void OnPlaceholderTap()
		{
			var action = await DisplayActionSheet("Choose Photo Type", "Cancel", null, "Take a Photo", "Select a Photo From Library");
			var cameraController = new CameraController();

			cameraController.MediaReady += (file) => 
			{
				if (file != null)
				{
					personImage.Source = ImageSource.FromFile(file.Path);
                	this.file = file;
				}
			};

			switch (action)
			{
				case ("Take a Photo"):
					file = await cameraController.TakePicture("temp_profile");
					break;
				case ("Select a Photo From Library"):
					file = await cameraController.SelectPicture();
					break;
			}
		}

		/// <summary>
		/// On Entry text change.
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = (!string.IsNullOrEmpty(name.Text))
				&& (!string.IsNullOrEmpty(birthPlaceEntry.Text))
				&& (!string.IsNullOrEmpty(town_city.Text))
				&& (!string.IsNullOrEmpty(notesEditor.Text))
				&& (relationPicker.SelectedIndex != -1)
                && (!string.IsNullOrEmpty(birthYear.Text));

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

		/// <summary>
		/// Saves the person to the database.
		/// </summary>
		private async void SavePerson(object sender, EventArgs ea)
		{

			if ((!string.IsNullOrEmpty(birthYear.Text)))
			{
				if (int.Parse(birthYear.Text) > DateTime.Now.Year)
				{
					await DisplayAlert("Add Person", "Birth year cannot be in the future", "OK");
                    return;
				}
			}

			savePersonButton.IsEnabled = false;

			Person tempPerson = new Person
			{
				name = name.Text,
                birthYear = birthYear.Text,
				birthPlace = birthPlaceEntry.Text,
				area = town_city.Text,
				notes = notesEditor.Text,
				pathToPhoto = null,
			};

			byte[] image = null;
			if (file != null)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					file.GetStream().CopyTo(ms);
					image = ms.ToArray();
				}
			}

			bool saved = false;

			try
			{
				saved = await personController.AddPersonRemotely(tempPerson, relations[relationPicker.SelectedIndex], image);

				if (saved)
				{
					if (isInTutorial)
					{
						Application.Current.MainPage = new AddTicketPrompt();
						isInTutorial = false;
					}
					else
					{
						Application.Current.MainPage = new RootPage();
					}
				}
				else
				{
					await DisplayAlert("Add Person", "Person could not be added.", "OK");
					savePersonButton.IsEnabled = true;
				}
			}
			catch (NoNetworkException ex)
			{
				await DisplayAlert("No Network", ex.Message, "OK");
				savePersonButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// Saves the person changes.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void SavePersonChanges(object sender, EventArgs e)
		{

			if ((!string.IsNullOrEmpty(birthYear.Text)))
			{
				if (int.Parse(birthYear.Text) > DateTime.Now.Year)
				{
					await DisplayAlert("Add Person", "Birth year cannot be in the future", "OK");
					return;
				}
			}

			savePersonButton.IsEnabled = false;

			var p = personController.GetPerson(person.id);

			p.name = name.Text;
            p.birthYear = birthYear.Text;
			p.birthPlace = birthPlaceEntry.Text;
			p.area = town_city.Text;
			p.notes = notesEditor.Text;

			byte[] image = null;
			if (file != null)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					file.GetStream().CopyTo(ms);
					image = ms.ToArray();
				}
			}

			Person returned = null;

			try
			{
				returned = await personController.UpdatePersonRemotely(p, relations[relationPicker.SelectedIndex], image);

				if (returned != null)
				{
					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Update Person", "Person could not be updated.", "OK");
					savePersonButton.IsEnabled = true;
				}
			}
			catch (NoNetworkException ex)
			{
				await DisplayAlert("No Network", ex.Message, "Dismiss");
				savePersonButton.IsEnabled = true;
			}
		}
	}
}