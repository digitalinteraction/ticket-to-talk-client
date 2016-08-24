﻿// Author: Daniel Welsh - d.welsh@ncl.ac.uk
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
	}
}


