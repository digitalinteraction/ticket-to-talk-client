// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 05/09/2016
//
// EditProfile.cs
using System;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Edit profile.
	/// </summary>
	public class EditProfile : ContentPage
	{
		Entry confirmPassword;
		Entry email;
		Entry name;
		Entry password;
		Button saveButton;
		byte[] image;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.EditProfile"/> class.
		/// </summary>
		public EditProfile()
		{

			Title = "Edit Profile";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			var profile = new UserProfileImage((Session.ScreenWidth * 0.8), null, ProjectResource.color_red);
			profile.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(onImageTap) });

			var nameLabel = new Label
			{
				Text = "Name",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};

			name = new Entry
			{
				Text = Session.activeUser.name,
				TextColor = ProjectResource.color_red,
			};
			name.TextChanged += EntryChanged;

			var emailLabel = new Label
			{
				Text = "Email",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};

			email = new Entry
			{
				Text = Session.activeUser.email,
				TextColor = ProjectResource.color_red,
			};
			email.TextChanged += EntryChanged;

			var passwordLabel = new Label
			{
				Text = "Password",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};

			password = new Entry
			{
				Text = Session.activeUser.password,
				TextColor = ProjectResource.color_red,
				IsPassword = true
			};
			password.TextChanged += EntryChanged;

			var confirmPasswordLabel = new Label
			{
				Text = "Confirm Password",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0)
			};

			confirmPassword = new Entry
			{
				Text = Session.activeUser.password,
				TextColor = ProjectResource.color_red,
				IsPassword = true
			};
			confirmPassword.TextChanged += EntryChanged;

			saveButton = new Button
			{
				Text = "Save",
				BackgroundColor = ProjectResource.color_grey,
				TextColor = ProjectResource.color_white,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0, 0, 0, 20),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
			};
			saveButton.Clicked += SaveButton_Clicked;

			var meta_content = new StackLayout
			{
				Padding = new Thickness(20),
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					profile,
					nameLabel,
					name,
					emailLabel,
					email,
					passwordLabel,
					password,
					confirmPasswordLabel,
					confirmPassword,
					saveButton
				}
			};

			var content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					profile,
					meta_content
				}
			};

			Content = new ScrollView
			{
				Content = content
			};
		}

		/// <summary>
		/// Ons the image tap.
		/// </summary>
		/// <param name="obj">Object.</param>
		private async void onImageTap(object obj)
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
				var bytes = MediaController.readBytesFromFile(file.Path);
				Session.activeUser.imageSource = ImageSource.FromFile(file.Path);
				image = bytes;
			}
		}

		/// <summary>
		/// Cancel the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		void cancel(object obj)
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Check null fields on entry text change.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void EntryChanged(object sender, EventArgs e)
		{
			var notNull = (!String.IsNullOrEmpty(name.Text))
				&& (!String.IsNullOrEmpty(email.Text))
				&& (!String.IsNullOrEmpty(password.Text))
				&& (!String.IsNullOrEmpty(confirmPassword.Text));

			if (notNull)
			{
				saveButton.BackgroundColor = ProjectResource.color_blue;
				saveButton.IsEnabled = true;
			}
			else
			{
				saveButton.BackgroundColor = ProjectResource.color_grey;
				saveButton.IsEnabled = false;
			}
		}

		/// <summary>
		/// Updates user on button press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void SaveButton_Clicked(object sender, EventArgs e)
		{
			if (!(password.Text.Equals(confirmPassword.Text)))
			{
				await DisplayAlert("Edit Profile", "The passwords do not match", "OK");
				return;
			}

			var userController = new UserController();
			var user = userController.getLocalUserByID(Session.activeUser.id);
			user.name = name.Text;
			user.email = email.Text;
			user.password = password.Text;

			var returned = await userController.updateUserRemotely(user, image);
			if (returned != null)
			{
				Session.activeUser.name = returned.name;
				Session.activeUser.email = returned.email;

				await Navigation.PopModalAsync();
			}
			else
			{
				await DisplayAlert("Edit Profile", "Profile could not be updated.", "OK");
			}
		}
	}
}

