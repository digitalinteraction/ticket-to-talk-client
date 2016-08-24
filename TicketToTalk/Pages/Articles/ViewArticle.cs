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
				Command = new Command(launchEditArticleView)
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

			var deleteArticleButton = new Button
			{
				Text = "Delete Article",
				TextColor = Color.Red
			};
			deleteArticleButton.Clicked += DeleteArticleButton_Clicked;

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
		/// Launches the browser.
		/// </summary>
		/// <returns>The browser.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		void launchBrowser(Object sender, EventArgs ea)
		{
			Device.OpenUri(new Uri(article.link));
		}

		/// <summary>
		/// Launches the edit article view.
		/// </summary>
		/// <returns>The edit article view.</returns>
		public void launchEditArticleView() 
		{
			Navigation.PushAsync(new AddArticle(article));
		}

		/// <summary>
		/// Delete article button is clicked.
		/// </summary>
		/// <returns>The article button clicked.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void DeleteArticleButton_Clicked(object sender, EventArgs e)
		{
			DisplayActionSheet("Delete Article", "Cancel", "Delete");
		}
	}
}