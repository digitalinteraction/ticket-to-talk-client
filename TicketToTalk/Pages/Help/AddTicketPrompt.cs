// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 12/09/2016
//
// AddTicketPrompt.cs
using System;
using System.Diagnostics;
using Plugin.Media;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Add ticket prompt.
	/// </summary>
	public class AddTicketPrompt : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AddTicketPrompt"/> class.
		/// </summary>
		public AddTicketPrompt()
		{

			AddPerson.isInTutorial = false;
			SeeInvite.isInTutorial = false;

			BackgroundColor = ProjectResource.color_red;

			var icon = new Image
			{
				Source = "ticket_icon.png",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50,
				WidthRequest = 50,
				Aspect = Aspect.AspectFill
			};

			var label = new Label
			{
				Text = "You've added a person! Let's add a photo 'ticket' that we can use in conversation with them.",
			};
			label.SetLabelStyleInversreCenter();

			var button = new Button
			{
				Text = "Add a Ticket"
			};
			button.SetButtonStyle(ProjectResource.color_dark);
			button.Clicked += Button_Clicked;

			var skipTutorialButton = new Button
			{
				Text = "Skip Tutorial"
			};
			skipTutorialButton.SetButtonStyle(ProjectResource.color_red);
			skipTutorialButton.Clicked += SkipTutorialButton_Clicked;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 20,
				Children = {
					icon,
					label,
					button,
					skipTutorialButton
				}
			};
		}

		/// <summary>
		/// Skips the tutorial button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SkipTutorialButton_Clicked(object sender, EventArgs e)
		{
			Application.Current.MainPage = new RootPage();
		}

		/// <summary>
		/// Buttons the clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void Button_Clicked(object sender, EventArgs e)
		{
			var action = await DisplayActionSheet("Choose Photo Type", "Cancel", null, "Take a Photo", "Select a Photo From Library");
			switch (action)
			{
				case ("Take a Photo"):
					TakePicture();
					break;
				case ("Select a Photo From Library"):
					SelectPicture();
					break;
			}
		}

		/// <summary>
		/// Selects the picture.
		/// </summary>
		/// <returns>The picture.</returns>
		private async void TakePicture()
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", "No camera avaialble.", "OK");
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{

				Directory = "TicketToTalk",
				Name = "ticket.jpg"
			});

			// App will not progress to new ticket screen on android without this...
			await DisplayAlert("File Location", "Photo Added!", "OK");

			var page = new NewTicket("Picture", file.Path);

			try
			{
				var nav = new NavigationPage(page);
				nav.BarTextColor = ProjectResource.color_white;
				nav.BarBackgroundColor = ProjectResource.color_blue;
				NewTicketInfo.isInTutorial = true;
				Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(nav));
				Navigation.RemovePage(this);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in taking photo.");
			}
		}

		/// <summary>
		/// Selects a picture from the library.
		/// </summary>
		public async void SelectPicture()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("Select Photo", "Photo select not supported", "OK");
				return;
			}

			var file = await CrossMedia.Current.PickPhotoAsync();
			if (file == null) { return; }

			// App will not progress to new ticket screen on android without this...
			await DisplayAlert("File Location", "Photo Added!", "OK");

			var page = new NewTicket("Picture", file.Path);

			try
			{
				var nav = new NavigationPage(page);
				nav.BarTextColor = ProjectResource.color_white;
				nav.BarBackgroundColor = ProjectResource.color_blue;
				NewTicketInfo.isInTutorial = true;
				Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(nav));
				Navigation.RemovePage(this);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in adding running tutorial.");
			}
		}

	}
}


