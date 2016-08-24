using System;
using TicketToTalk;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View an article.
	/// </summary>
	public class ViewArticle : ContentPage
	{
		Article article;

		public ViewArticle(Article article)
		{
			this.Title = "Article";
			this.article = article;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Edit",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(editOptions)
			});

			//var titleLabel = new Label
			//{
			//	Text = article.title,
			//	FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
			//	TextColor = Color.FromHex(ProjectColours.red),
			//	FontAttributes = FontAttributes.Bold
			//};

			var titleStack = new StackLayout
			{
				Padding = new Thickness(20, 10),
				Children =
				{
					new Label
					{
						Text = article.title,
						FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
						TextColor = ProjectResource.color_dark,
						FontAttributes = FontAttributes.Bold,
						HorizontalOptions = LayoutOptions.CenterAndExpand
					}
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

			var viewArticleButton = new Button 
			{
				Text = "View Article",
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_blue,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0,0,0,10),
				BorderRadius = 5
			};
			viewArticleButton.Clicked += launchBrowser;

			var buttonStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.EndAndExpand,
				Spacing = 0,
				Children = {
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
		public async void editOptions()
		{
			var action = await DisplayActionSheet("Article Options", "Cancel", "Delete", "Edit", "Share");

			switch (action)
			{
				case ("Delete"):
					var articleController = new ArticleController();
					await Navigation.PopAsync();
					if (articleController.deleteArticleRemotely(article)) 
					{
						articleController.deleteArticleLocally(article);
					}
					break;
				case ("Edit"):
					var nav = new NavigationPage(new AddArticle(article));
					nav.BarTextColor = ProjectResource.color_white;
					nav.BarBackgroundColor = ProjectResource.color_blue;

					await Navigation.PushModalAsync(nav);
					break;
				case ("Share"):
					break;
			}
		}

		/// <summary>
		/// Launches the browser.
		/// </summary>
		/// <returns>The browser.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		void launchBrowser(Object sender, EventArgs ea)
		{
			Device.OpenUri(new Uri(article.link));
		}
	}
}