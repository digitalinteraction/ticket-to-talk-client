using System;
using Plugin.GoogleAnalytics;
using TicketToTalk;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View an article.
	/// </summary>
	public class ViewArticle : TrackedContentPage
	{
		public static Article currentArticle;
		public static bool tutorialShown;

		public ViewArticle(Article article)
		{
            TrackedName = "View Article";

			currentArticle = article;

			this.SetBinding(TitleProperty, "title");
			this.BindingContext = currentArticle;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Options",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(EditOptions)
			});

			var articleTitle = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				TextColor = ProjectResource.color_dark,
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};
			articleTitle.SetBinding(Label.TextProperty, "title");
			articleTitle.BindingContext = currentArticle;

			var titleStack = new StackLayout
			{
				Padding = new Thickness(20, 10),
				Children =
				{
					articleTitle
				}
			};

			var notesLabel = new Label
			{
				Text = "Notes",
				TextColor = ProjectResource.color_blue
			};

			var notesContent = new Label
			{
				Text = article.notes,
				TextColor = ProjectResource.color_dark
			};
			notesContent.SetBinding(Label.TextProperty, "notes");
			notesContent.BindingContext = currentArticle;

			var linkLabel = new Label
			{
				Text = "Link",
				TextColor = ProjectResource.color_blue
			};

			var linkContent = new Label
			{
				Text = article.link,
				TextColor = ProjectResource.color_dark
			};
			linkContent.SetBinding(Label.TextProperty, "link");
			linkContent.BindingContext = currentArticle;

			var viewArticleButton = new Button
			{
				Text = "View Article",
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_blue,
				WidthRequest = Session.ScreenWidth * 0.4,
				Margin = new Thickness(0, 0, 0, 10),
				BorderRadius = 5
			};
			viewArticleButton.Clicked += LaunchBrowser;

			var shareArticleButton = new Button
			{
				Text = "Share Article",
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_dark,
				WidthRequest = Session.ScreenWidth * 0.4,
				Margin = new Thickness(0, 0, 0, 10),
				BorderRadius = 5
			};
			shareArticleButton.Clicked += ShareArticleButton_Clicked;

			var buttonStack = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Spacing = 5,
				Children = {
					shareArticleButton,
					viewArticleButton
				}
			};

			var contentStack = new StackLayout
			{
				Padding = new Thickness(20),
				Children = {
					linkLabel,
					linkContent,
					notesLabel,
					notesContent,
				}
			};

			Content = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.Fill,
				Children = {
					titleStack,
					contentStack,
					buttonStack
				}
			};
		}

		/// <summary>
		/// Displays edit options
		/// </summary>
		private async void EditOptions()
		{
			var action = await DisplayActionSheet("Article Options", "Cancel", "Delete", "Edit");

			switch (action)
			{
				case ("Delete"):
					var articleController = new ArticleController();

					bool deleted = false;
					try
					{
						deleted = await articleController.DestoryArticle(currentArticle);
						if (deleted) 
						{
							await Navigation.PopAsync();
							break;
						}
					}
					catch (NoNetworkException ex) 
					{
						
						await DisplayAlert("No Network", ex.Message, "Dismiss");
					}
					break;
					
				case ("Edit"):
					var nav = new NavigationPage(new AddArticle(currentArticle));
					nav.BarTextColor = ProjectResource.color_white;
					nav.BarBackgroundColor = ProjectResource.color_blue;

					await Navigation.PushModalAsync(nav);
					break;
			}
		}

		/// <summary>
		/// Launches the browser.
		/// </summary>
		/// <returns>The browser.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		private void LaunchBrowser(Object sender, EventArgs ea)
		{
			Device.OpenUri(new Uri(currentArticle.link));
		}

		/// <summary>
		/// Shares the article button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void ShareArticleButton_Clicked(object sender, EventArgs e)
		{
			var nav = new NavigationPage(new ShareArticle(currentArticle));

			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			await Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Session.activeUser.firstLogin && !tutorialShown)
			{

				var text = "View your article, or, you can share articles by pressing the 'Options' button.";

				Navigation.PushModalAsync(new HelpPopup(text, "file_white_icon.png"));
				tutorialShown = true;
			}
		}
	}
}