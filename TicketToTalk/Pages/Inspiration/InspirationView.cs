﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Inspiration view.
	/// </summary>
	public class InspirationView : TrackedContentPage
	{
		private Inspiration inspiration;
		private Label question;
		private Label promptLabel;
		private Button recordMediaButton = new Button();
		private Button searchButton;
		private string searchLink;
		private InspirationController inspirationController;

		public static bool tutorialShown;
		bool no_ins = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.InspirationView"/> class.
		/// </summary>
		public InspirationView()
		{

            TrackedName = "Inspiration";

			this.inspirationController = new InspirationController();
			// Check for new inspirations.

			var task = Task.Run(() => inspirationController.GetInspirationsFromServer());

			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			question = new Label
			{
			};
            question.SetSubHeaderStyle();
            question.VerticalOptions = LayoutOptions.Start;

			promptLabel = new Label
			{
				//HorizontalOptions = LayoutOptions.Start,
				//TextColor = ProjectResource.color_dark,
			};
            promptLabel.SetBodyStyle();
            promptLabel.VerticalOptions = LayoutOptions.Start;

			Title = "Inspiration";

			searchButton = new Button
			{
				Text = "",
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = Session.ScreenWidth * 0.5,
				IsVisible = false,
				IsEnabled = true,
			};
            searchButton.SetStyle();
            searchButton.Margin = new Thickness(0, 20);
            searchButton.VerticalOptions = LayoutOptions.Start;
            searchButton.Clicked += SearchButton_Clicked;

			inspiration = inspirationController.GetRandomInspiration();

			if (inspiration != null)
			{
				inspiration = PopulateVariables(inspiration);

				SetViewToInspiration();
			}
			else 
			{
				no_ins = true;
			}

            recordMediaButton.SetStyle();
            recordMediaButton.Margin = new Thickness(0, 0, 0, 0);
			recordMediaButton.HorizontalOptions = LayoutOptions.CenterAndExpand;
			recordMediaButton.WidthRequest = 125;
			recordMediaButton.TextColor = ProjectResource.color_white;
			recordMediaButton.BackgroundColor = ProjectResource.color_red;
			recordMediaButton.Clicked += RecordMediaButton_Clicked;

			var nextButton = new Button()
			{
				Text = "Next",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				WidthRequest = 125,
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_dark
			};
            nextButton.SetStyle();
            nextButton.Margin = new Thickness(0, 0, 0, 0);
			nextButton.Clicked += NextButton_Clicked;

			var buttonStack = new StackLayout
			{
				Spacing = 2,
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Children =
				{
					recordMediaButton,
					nextButton
				}
			};

            var noNetworkLabel = new Label 
            {
				Text = "Please connect to a network to view inspirations.",
            };
            noNetworkLabel.SetSubHeaderStyle();
            noNetworkLabel.VerticalOptions = LayoutOptions.Start;

			StackLayout content = null;

			if (no_ins)
			{
				content = new StackLayout
				{
					Padding = new Thickness(20),
					Spacing = 12,
					Children = {
                        noNetworkLabel
					}
				};
			}
			else 
			{
				content = new StackLayout
					{
						Padding = new Thickness(20),
						Spacing = 12,
						Children = {
						question,
						promptLabel,
						searchButton,
						buttonStack
					}
				};
			}

			Content = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Spacing = 0,
				Children =
				{
					content
				}
			};
		}

		/// <summary>
		/// Displays the next prompt on button press.
		/// </summary>
		/// <returns>The button clicked.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void NextButton_Clicked(object sender, EventArgs e)
		{
			var newIns = inspirationController.GetRandomInspiration();

			while (newIns.Equals(inspiration))
			{
				newIns = inspirationController.GetRandomInspiration();
			}

			inspiration = PopulateVariables(newIns);

			SetViewToInspiration();
		}

		/// <summary>
		/// Sets the view to inspiration.
		/// </summary>
		/// <returns>The view to inspiration.</returns>
		private void SetViewToInspiration()
		{
			question.Text = inspiration.question;
			promptLabel.Text = inspiration.prompt;

			switch (inspiration.mediaType.Trim())
			{
				case ("Picture"):
					recordMediaButton.Text = "Take Picture";
					break;
				case ("Sound"):
					recordMediaButton.Text = "Record Sound";
					break;
				case ("Video"):
					recordMediaButton.Text = "Record Video";
					break;
				case ("YouTube"):
					recordMediaButton.Text = "Add YouTube Clip";
					break;
			}
		}

		///// <summary>
		///// Selects the picture.
		///// </summary>
		///// <returns>The picture.</returns>
		//private async void TakePicture()
		//{
		//	if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
		//	{
		//		await DisplayAlert("No Camera", "No camera avaialble.", "OK");
		//		return;
		//	}

		//	var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
		//	{
		//		Directory = "TicketToTalk",
		//		Name = "ticket.jpg"
		//	});

		//	if (file == null)
		//		return;

		//	var nav = new NavigationPage(new NewTicket("Picture", file.Path));
		//	nav.BarTextColor = ProjectResource.color_white;
		//	nav.BarBackgroundColor = ProjectResource.color_blue;

		//	await Navigation.PushModalAsync(nav);
		//}

		//private async void SelectPicture()
		//{
		//	if (!CrossMedia.Current.IsPickPhotoSupported)
		//	{
		//		await DisplayAlert("Select Photo", "Photo select not supported", "OK");
		//		return;
		//	}

		//	var file = await CrossMedia.Current.PickPhotoAsync();
		//	if (file == null) { return; }

		//	var nav = new NavigationPage(new NewTicket("Picture", file.Path));
		//	nav.BarTextColor = ProjectResource.color_white;
		//	nav.BarBackgroundColor = ProjectResource.color_blue;

		//	await Navigation.PushModalAsync(nav);
		//}

		/// <summary>
		/// Populates the variables.
		/// </summary>
		/// <returns>The variables.</returns>
		/// <param name="ins">Ins.</param>
		private Inspiration PopulateVariables(Inspiration ins)
		{
			var asChars = ins.question.ToCharArray();
			var ageIdx = ins.question.LastIndexOf("[age=");

			if (ageIdx != -1)
			{
				ageIdx += 5;
				var age = "" + asChars[ageIdx] + asChars[ageIdx + 1];

				ins.question = ins.question.Replace("[age=" + age + "]", age);
				ins.prompt = ins.prompt.Replace("[age=" + age + "]", age);

				var year = (Int32.Parse(Session.activePerson.birthYear) + Int32.Parse(age)).ToString();

				ins.question = ins.question.Replace("[year]", year);
				ins.prompt = ins.prompt.Replace("[year]", year);
			}
			ins.question = ins.question.Replace("[name]", Session.activePerson.name);
			ins.prompt = ins.prompt.Replace("[name]", Session.activePerson.name);

			var linkIdx = ins.prompt.IndexOf("[link=\"", StringComparison.Ordinal);

			if (linkIdx != -1)
			{
				linkIdx += 7;
				var linkEnd = ins.prompt.IndexOf("\"]", StringComparison.Ordinal);
				var link = ins.prompt.Substring(linkIdx, linkEnd - linkIdx);

				ins.prompt = ins.prompt.Replace("HERE [link=\"" + link + "\"] ", "");

				searchLink = link;

				searchButton.Text = "Search Google...";
				searchButton.IsVisible = true;
				searchButton.IsEnabled = true;
			}
			else
			{
				searchButton.IsVisible = false;
			}

			return ins;
		}

		// <summary>
		// Searchs the button clicked.
		// </summary>
		// <param name="sender">Sender.</param>
		// <param name="e">E.</param>
		private void SearchButton_Clicked(object sender, EventArgs e)
		{
			Device.OpenUri(new Uri(searchLink));
		}

		/// <summary>
		/// Records the media button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void RecordMediaButton_Clicked(object sender, EventArgs e)
		{
			NavigationPage nav = null;

			switch (inspiration.mediaType.Trim())
			{
				case ("Picture"):

					var cameraController = new CameraController();

					cameraController.MediaReady += async (file) => 
					{
						if (file == null)
							return;

						nav = new NavigationPage(new NewTicket("Picture", file.Path));
						nav.BarTextColor = ProjectResource.color_white;
						nav.BarBackgroundColor = ProjectResource.color_blue;

						await Navigation.PushModalAsync(nav);
					};

					var action = await DisplayActionSheet("Choose Photo Type", "Cancel", null, "Take a Photo", "Select a Photo From Library");
					switch (action)
					{
						case ("Take a Photo"):
							await cameraController.TakePicture("temp_ticket");
							break;
						case ("Select a Photo From Library"):
							await cameraController.SelectPicture();
							break;
					}
					break;
				case ("Sound"):
					nav = new NavigationPage(new AudioRecorder());
					nav.BarTextColor = ProjectResource.color_white;
					nav.BarBackgroundColor = ProjectResource.color_blue;

					await Navigation.PushModalAsync(nav);
					break;
				case ("Video"):
					break;
				case ("YouTube"):
					nav = new NavigationPage(new AddYoutubeLinkView());
					nav.BarTextColor = ProjectResource.color_white;
					nav.BarBackgroundColor = ProjectResource.color_blue;

					await Navigation.PushModalAsync(nav);
					break;
			}
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Session.activeUser.firstLogin && !tutorialShown)
			{

				var text = "Use Inspirations to help you add tickets. Here you'll find prompts to help you build up a collection.";

				Navigation.PushModalAsync(new HelpPopup(text, "light_white_icon.png"));
				tutorialShown = true;
			}
		}
	}
}