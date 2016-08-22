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
		Inspiration inspiration;
		Label question = new Label();
		Label promptLabel = new Label();
		Button recordMediaButton = new Button();
		Button searchButton;
		string searchLink;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.InspirationView"/> class.
		/// </summary>
		public InspirationView()
		{
			question.HorizontalOptions = LayoutOptions.CenterAndExpand;
			question.TextColor = ProjectResource.color_dark;
			question.FontAttributes = FontAttributes.Bold;

			promptLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
			promptLabel.TextColor = ProjectResource.color_dark;

			Title = "Inspiration";

			searchButton = new Button
			{
				Text = "",
				BackgroundColor = ProjectResource.color_blue,
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				//WidthRequest = Session.ScreenWidth * 0.5,
				IsVisible = false,
				IsEnabled = true,
				Margin = new Thickness(0, 20)
			};
			searchButton.Clicked += SearchButton_Clicked;

			// Check for new inspirations.
			var task = Task.Run(() => this.getIns()).Result;

			foreach (Inspiration ins in task)
			{
				Debug.WriteLine(ins);
			}

			InspirationDB insDB = new InspirationDB();
			inspiration = populateVariables(insDB.getRandomInspiration());
			insDB.close();

			setViewToInspiration();

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

			var img = new Image 
			{
				Source = "drawer_icon_.png",
				WidthRequest = 10,
				HeightRequest = 10,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var drawPull = new StackLayout
			{
				WidthRequest = 10,
				BackgroundColor = ProjectResource.color_blue,
				Children = 
				{
					img
				}
			};

			var content = new StackLayout
			{
				Padding = new Thickness(10,20,20,10),
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
					drawPull, 
					content
				}
			};
		}

		/// <summary>
		/// Gets inspirations from the server.
		/// </summary>
		/// <returns>The ins.</returns>
		public async Task<List<Inspiration>> getIns()
		{
			// Send get request for inspirations
			Debug.WriteLine("Sending get request for inspirations.");
			NetworkController net = new NetworkController();
			var jobject = await net.sendGetRequest("inspiration/get", new Dictionary<string, string>());
			Debug.WriteLine(jobject);

			var jtoken = jobject.GetValue("Inspirations");
			var inspirations = jtoken.ToObject<List<Inspiration>>();
			InspirationDB insDB = new InspirationDB();
			foreach (Inspiration ins in inspirations)
			{
				Debug.WriteLine(ins);
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
		void NextButton_Clicked(object sender, EventArgs e)
		{
			InspirationDB insDB = new InspirationDB();
			var newIns = insDB.getRandomInspiration();

			while (newIns.Equals(inspiration)) 
			{
				newIns = insDB.getRandomInspiration();
			}

			inspiration = populateVariables(newIns);
			insDB.close();

			setViewToInspiration();
		}

		/// <summary>
		/// Sets the view to inspiration.
		/// </summary>
		/// <returns>The view to inspiration.</returns>
		public void setViewToInspiration() 
		{
			question.Text = inspiration.question;
			promptLabel.Text = inspiration.prompt;

			Debug.WriteLine("Inspiration: ins_media_type = " + inspiration.mediaType);

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

			await Navigation.PushAsync(new NewTicket("Picture", file.Path));
		}

		public async void SelectPicture()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				await DisplayAlert("Select Photo", "Photo select not supported", "OK");
				return;
			}

			var file = await CrossMedia.Current.PickPhotoAsync();
			if (file == null) { return; }

			await Navigation.PushAsync(new NewTicket("Picture", file.Path));
		}

		/// <summary>
		/// Populates the variables.
		/// </summary>
		/// <returns>The variables.</returns>
		/// <param name="ins">Ins.</param>
		private Inspiration populateVariables(Inspiration ins) 
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

			var linkIdx = ins.prompt.IndexOf("[link=\"");

			if (linkIdx != -1)
			{
				linkIdx += 7;
				var linkEnd = ins.prompt.IndexOf("\"]");
				var link = ins.prompt.Substring(linkIdx, linkEnd - linkIdx);

				ins.prompt = ins.prompt.Replace("HERE [link=\"" + link + "\"] ", "");
				Debug.WriteLine("Inspirations: link = " + link);

				searchLink = link;

				Debug.WriteLine("Inspirations: searchLink set to - " + searchLink);

				searchButton.Text = "Search Google...";
				searchButton.IsVisible = true;
				searchButton.IsEnabled = true;
			}
			else 
			{
				Debug.WriteLine("Inspirations: setting button as false");
				searchButton.IsVisible = false;
				Debug.WriteLine("Inspirations: set to false");
			}

			return ins;
		}

		// <summary>
		// Searchs the button clicked.
		// </summary>
		// <param name="sender">Sender.</param>
		// <param name="e">E.</param>
		void SearchButton_Clicked(object sender, EventArgs e)
		{
			Console.WriteLine("Clicked");
			Debug.WriteLine("Inspirations: SearchLink = " + searchLink);
			Device.OpenUri(new Uri(searchLink));
		}

		/// <summary>
		/// Records the media button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void RecordMediaButton_Clicked(object sender, EventArgs e)
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
					await Navigation.PushAsync(new AudioRecorder());
					break;
				case ("Video"):
					break;
			}
		}
	}
}