// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 24/08/2016
//
// ShareArticle.cs
using System;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Share article.
	/// </summary>
	public class ShareArticle : TrackedContentPage
	{
		private Entry email;
		private Button sendButton;
		private Switch toggle;
		private bool includeNotes = false;
		private Article article;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ShareArticle"/> class.
		/// </summary>
		public ShareArticle(Article article)
		{
            TrackedName = "Share Article";

			Title = "Share Article";
			this.article = article;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			var descriptionLabel = new Label
			{
				Text = "Enter the email address of who you want to send this article to.",
			};
            descriptionLabel.SetSubHeaderStyle();
            descriptionLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            descriptionLabel.HorizontalTextAlignment = TextAlignment.Center;

			email = new Entry
			{
				Placeholder = "Recipient's email",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_red,
				WidthRequest = Session.ScreenWidth * 0.8
			};
            email.SetStyle();
			email.TextChanged += InputChanged;

			var notesLabel = new Label
			{
				Text = "Include your notes",
			};
            notesLabel.SetSubHeaderStyle();
            notesLabel.HorizontalTextAlignment = TextAlignment.Center;

			toggle = new Switch
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.Center,
			};
			toggle.Toggled += Toggle_Toggled;

			var notesStack = new StackLayout
			{
				Padding = new Thickness(20),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				//VerticalOptions = LayoutOptions.StartAndExpand,
				Children =
				{
					notesLabel,
					toggle
				}
			};

			sendButton = new Button
			{
				Text = "Share",
				BorderRadius = 5,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				IsEnabled = false,
				WidthRequest = Session.ScreenWidth * 0.5
			};
            sendButton.SetStyle();
			sendButton.Clicked += SendButton_Clicked;

			var buttonStack = new StackLayout
			{
				Padding = new Thickness(20),
				VerticalOptions = LayoutOptions.EndAndExpand,
				Spacing = 0,
				Children =
				{
					sendButton
				}
			};

			var content = new StackLayout
			{
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(20),
				Spacing = 10,
				Children =
				{
					descriptionLabel,
					email
				}
			};

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Children =
				{
					content,
					notesStack,
					buttonStack
				}
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
		/// Shares the article on button press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void SendButton_Clicked(object sender, EventArgs e)
		{

			sendButton.IsEnabled = false;

			ArticleController articleController = new ArticleController();

			bool shared = false;
			try 
			{
				shared = await articleController.ShareArticle(article, email.Text, includeNotes);

				if (shared)
				{
					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Share Articles", "The article could not be shared", "OK");
					sendButton.IsEnabled = true;
				}
			}
			catch (NoNetworkException ex) 
			{
				await DisplayAlert("No Network", ex.Message, "Dismiss");
				sendButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// Inputs the changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void InputChanged(object sender, EventArgs e)
		{
			var v = (!string.IsNullOrEmpty(email.Text));

			if (v)
			{
				sendButton.BackgroundColor = ProjectResource.color_red;
				sendButton.IsEnabled = true;
			}
			else
			{
				sendButton.BackgroundColor = ProjectResource.color_grey;
				sendButton.IsEnabled = false;
			}
		}

		/// <summary>
		/// Toggles the toggled.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Toggle_Toggled(object sender, ToggledEventArgs e)
		{
			if (includeNotes)
			{
				includeNotes = false;
			}
			else
			{
				includeNotes = true;
			}
		}
	}
}


