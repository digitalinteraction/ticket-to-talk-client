using System;
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
	public class InspirationView : ContentPage
	{
		private Inspiration inspiration;
		private Label question;
		private Label promptLabel;
		private Button recordMediaButton = new Button();
		private Button searchButton;
		private string searchLink;

		public static bool tutorialShown;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.InspirationView"/> class.
		/// </summary>
		public InspirationView()
		{
			question = new Label
			{
				HorizontalOptions = LayoutOptions.Start,
				TextColor = ProjectResource.color_dark,
				FontAttributes = FontAttributes.Bold,
			};

			promptLabel = new Label
			{
				HorizontalOptions = LayoutOptions.Start,
				TextColor = ProjectResource.color_dark,
			};

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
				Margin = new Thickness(0, 20)
			};
			searchButton.Clicked += SearchButton_Clicked;

			// Check for new inspirations.
			var task = Task.Run(() => this.GetIns()).Result;

			InspirationDB insDB = new InspirationDB();
			inspiration = PopulateVariables(insDB.GetRandomInspiration());
			insDB.close();

			SetViewToInspiration();

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

			var content = new StackLayout
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
		/// Gets inspirations from the server.
		/// </summary>
		/// <returns>The ins.</returns>
		private async Task<List<Inspiration>> GetIns()
		{
			// Send get request for inspirations
			NetworkController net = new NetworkController();
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var jobject = await net.SendGetRequest("inspiration/get", parameters);

			var data = jobject.GetData();
			var inspirations = data["inspirations"].ToObject<List<Inspiration>>();

			InspirationDB insDB = new InspirationDB();
			foreach (Inspiration ins in inspirations)
			{
				var stored = insDB.GetInspiration(ins.id);
				if (stored == null)
				{
					insDB.AddInspiration(ins);
				}
				else if (stored.GetHashCode() != ins.GetHashCode())
				{
					insDB.DeleteInspiration(ins.id);
					insDB.AddInspiration(ins);
				}
			}
			insDB.close();

			return inspirations;
		}

		/// <summary>
		/// Displays the next prompt on button press.
		/// </summary>
		/// <returns>The button clicked.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void NextButton_Clicked(object sender, EventArgs e)
		{
			InspirationDB insDB = new InspirationDB();
			var newIns = insDB.GetRandomInspiration();

			while (newIns.Equals(inspiration))
			{
				newIns = insDB.GetRandomInspiration();
			}

			inspiration = PopulateVariables(newIns);
			insDB.close();

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

			if (file == null)
				return;

			var nav = new NavigationPage(new NewTicket("Picture", file.Path));
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			await Navigation.PushModalAsync(nav);
		}

		private async void SelectPicture()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("Select Photo", "Photo select not supported", "OK");
				return;
			}

			var file = await CrossMedia.Current.PickPhotoAsync();
			if (file == null) { return; }

			var nav = new NavigationPage(new NewTicket("Picture", file.Path));
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			await Navigation.PushModalAsync(nav);
		}

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
			switch (inspiration.mediaType.Trim())
			{
				case ("Picture"):
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
					break;
				case ("Sound"):
					var nav = new NavigationPage(new AudioRecorder());
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