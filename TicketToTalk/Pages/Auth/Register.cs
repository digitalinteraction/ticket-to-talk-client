﻿using System;
using System.Diagnostics;
using System.IO;
using ImageCircle.Forms.Plugin.Abstractions;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Register.
	/// </summary>
	public partial class Register : ContentPage
	{
		Image personImage;
		Entry nameEntry;
		Entry emailEntry;
		Entry passwordEntry;
		Entry confirmPasswordEntry;
		Button savePersonButton;
		MediaFile file;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Register"/> class.
		/// </summary>
		public Register()
		{

			// Set title.
			this.Title = "Register";

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
			personImage.Source = "profile_placeholder.png";
			personImage.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(onPlaceholderTap) });

			// Create form.
			var nameLabel = new Label
			{
				Text = "Name",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0,10,0,0)
			};
			nameEntry = new Entry 
			{	
				Placeholder = "Enter your name",
				TextColor = ProjectResource.color_red
			};
			nameEntry.Focus();
			nameEntry.TextChanged += Entry_TextChanged;

			var emailLabel = new Label { 
				Text = "Email",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};
			emailEntry = new Entry { 
				Placeholder = "Enter your email...",
				TextColor = ProjectResource.color_red
			};
			emailEntry.TextChanged += Entry_TextChanged;

			var passwordLabel = new Label { 
				Text = "Password",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};
			passwordEntry = new Entry { 
				Placeholder = "Enter your password...",
				IsPassword = true,
				TextColor = ProjectResource.color_red,
			};
			passwordEntry.TextChanged += Entry_TextChanged;

			var confirmPasswordLabel = new Label
			{
				Text = "Confirm Password",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};
			confirmPasswordEntry = new Entry
			{
				Placeholder = "Re-enter your password...",
				IsPassword = true,
				TextColor = ProjectResource.color_red
			};
			confirmPasswordEntry.TextChanged += Entry_TextChanged;

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
				Children =
				{
					nameLabel,
					nameEntry,
					emailLabel,
					emailEntry,
					passwordLabel,
					passwordEntry,
					confirmPasswordLabel,
					confirmPasswordEntry,
				}
			};

			savePersonButton = new Button
			{
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				WidthRequest = Session.ScreenWidth * 0.5,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontAttributes = FontAttributes.Bold,
				Text = "Save",
				IsEnabled = false,
				Margin = new Thickness(0,0,0,10)
			};
			savePersonButton.Clicked += register;
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
		/// On entry text change...
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Entry_TextChanged(object sender, TextChangedEventArgs e)
		{
			var entriesNotNull = (!String.IsNullOrEmpty(nameEntry.Text))
				&& (!String.IsNullOrEmpty(emailEntry.Text))
				&& (!String.IsNullOrEmpty(passwordEntry.Text))
				&& (!String.IsNullOrEmpty(confirmPasswordEntry.Text));
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
		/// On image tap
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
		/// Save the user when the save button is clicked.
		/// </summary>
		async void register(Object sender, EventArgs e)
		{
			savePersonButton.IsEnabled = false;

			var registered = false;
			if (string.Compare(passwordEntry.Text, confirmPasswordEntry.Text, StringComparison.Ordinal) != 0)
			{
				await DisplayAlert("Register", "Password and Confirm Password do not match.", "OK");
				return;
			}
			else 
			{
				// Create new user.
				User user = new User();
				user.name = nameEntry.Text;
				user.email = emailEntry.Text;
				user.password = passwordEntry.Text;

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

				var userController = new UserController();
				registered = await userController.registerNewUser(user, image);
			}

			if (registered)
			{
				await Navigation.PushAsync(new AllProfiles());
				Navigation.RemovePage(this);
			}
			else 
			{
				await DisplayAlert("Register", "Your profile could not be registered.", "OK");
				savePersonButton.IsEnabled = true;
			}
		}
	}
}