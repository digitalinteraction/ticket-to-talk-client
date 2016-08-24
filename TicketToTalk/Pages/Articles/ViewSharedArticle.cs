// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 24/08/2016
//
// ViewSharedArticle.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// View shared article.
	/// </summary>
	public class ViewSharedArticle : ContentPage
	{
		Article article;
		ArticleController articleController = new ArticleController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ViewSharedArticle"/> class.
		/// </summary>
		/// <param name="article">Article.</param>
		public ViewSharedArticle(Article article)
		{
			this.Title = "Article";
			this.article = article;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

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
				Margin = new Thickness(0, 0, 0, 10),
				BorderRadius = 5
			};
			viewArticleButton.Clicked += launchBrowser;

			var acceptArticle = new Button
			{
				Text = "Add",
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_red,
				WidthRequest = Session.ScreenWidth * 0.3,
				Margin = new Thickness(0, 0, 0, 10),
			};
			acceptArticle.Clicked += AcceptArticle_Clicked;

			var rejectArticle = new Button
			{
				Text = "Dismiss",
				TextColor = ProjectResource.color_white,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = ProjectResource.color_dark,
				WidthRequest = Session.ScreenWidth * 0.3,
				Margin = new Thickness(0, 0, 0, 10),
			};
			rejectArticle.Clicked += RejectArticle_Clicked;

			var shareControls = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					rejectArticle,
					acceptArticle
				}
			};

			var buttonStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.EndAndExpand,
				Padding = new Thickness(20, 0),
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
					buttonStack,
					shareControls
				}
			};
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
		/// Launchs the browser.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="ea">Ea.</param>
		void launchBrowser(Object sender, EventArgs ea)
		{
			Device.OpenUri(new Uri(article.link));
		}


		/// <summary>
		/// Accepts the article clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void AcceptArticle_Clicked(object sender, EventArgs e)
		{
			var accepted = await articleController.addShared(article);
			if (accepted) 
			{
				ViewSharedArticles.articles.Remove(article);
				await Navigation.PopModalAsync();
			}
		}

		/// <summary>
		/// Rejects the article clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void RejectArticle_Clicked(object sender, EventArgs e)
		{
			var rejected = await articleController.rejectShared(article);
			if (rejected) 
			{
				ViewSharedArticles.articles.Remove(article);
				await Navigation.PopModalAsync();
			}
		}
	}
}


