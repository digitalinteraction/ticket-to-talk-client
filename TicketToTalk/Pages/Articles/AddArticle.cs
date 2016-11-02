using System;
using System.Collections.Generic;
using System.Diagnostics;
using TicketToTalk;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Add article.
	/// </summary>
	public class AddArticle : ContentPage
	{
		private Entry title;
		private Editor notes;
		private Entry link;

		private Article article = null;
		private Button saveButton;

		ArticleController articleController = new ArticleController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.AddArticle"/> class.
		/// </summary>
		public AddArticle(Article article)
		{

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			if (article != null)
			{
				this.article = article;
			}

			Title = "New Article";

			var titleLabel = new Label
			{
				Text = "Title",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			title = new Entry
			{
				Placeholder = "Title",
				TextColor = ProjectResource.color_red,
				Margin = new Thickness(0, 0, 0, 2)
			};
			title.TextChanged += Entry_TextChanged;

			var notesLabel = new Label
			{
				Text = "Notes",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			notes = new Editor
			{
				Text = "Add some notes about the article!",
				TextColor = ProjectResource.color_red,
				Margin = new Thickness(0, 0, 0, 2)
			};
			notes.TextChanged += Entry_TextChanged;

			var linkLabel = new Label
			{
				Text = "Link",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			link = new Entry
			{
				Placeholder = "Add a link to the article.",
				TextColor = ProjectResource.color_red,
				Margin = new Thickness(0, 0, 0, 2)
			};
			link.TextChanged += Entry_TextChanged;

			saveButton = new Button()
			{
				Text = "Save",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				BorderRadius = 5,
				IsEnabled = false,
				WidthRequest = (Session.ScreenWidth * 0.5),
				Margin = new Thickness(0, 0, 0, 10)
			};
			if (article == null)
			{
				saveButton.Clicked += SaveArticle;
			}
			else
			{
				saveButton.Text = "Update";
				saveButton.Clicked += UpdateArticle;
			}

			if (article != null)
			{
				title.Text = article.title;
				link.Text = article.link;
				notes.Text = article.notes;
				Title = article.title;
			}

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(20, 10, 20, 20),
				Spacing = 0,
				Children =
				{
					titleLabel,
					title,
					linkLabel,
					link,
					notesLabel,
					notes,
				}
			};

			var buttonStack = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Children =
				{
					saveButton
				}
			};

			Content = new StackLayout
			{
				Spacing = 0,
				Children = {
					detailsStack,
					buttonStack
				}
			};
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		void cancel()
		{
			Navigation.PopModalAsync();

		}

		/// <summary>
		/// Entries the text changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = (!string.IsNullOrEmpty(title.Text))
				&& (!string.IsNullOrEmpty(notes.Text))
				&& (!string.IsNullOrEmpty(link.Text));

			if (entriesNotNull)
			{
				saveButton.BackgroundColor = ProjectResource.color_blue;
				saveButton.IsEnabled = true;
			}
			else
			{
				saveButton.BackgroundColor = ProjectResource.color_grey;
				saveButton.IsEnabled = false;
			}
		}

		/// <summary>
		/// Saves the article.
		/// </summary>
		/// <returns>The article.</returns>
		public async void SaveArticle(object sender, EventArgs e)
		{
			saveButton.IsEnabled = false;

			var post_link = link.Text.ToLower();
			if (!(post_link.StartsWith("http://", StringComparison.Ordinal)) && !(post_link.StartsWith("https://", StringComparison.Ordinal)))
			{
				post_link = "http://" + post_link;
			}

			var article = new Article
			{
				title = title.Text,
				notes = notes.Text,
				link = post_link
			};

			var added =  await articleController.AddArticleRemotely(article);

			if (added)
			{
				await Navigation.PopModalAsync();
			}
			else
			{ 
				await DisplayAlert("Articles", "Article could not be saved.", "OK");
				saveButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// Updates the article.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void UpdateArticle(object sender, EventArgs e)
		{
			var post_link = link.Text.ToLower();
			if (!(post_link.StartsWith("http://", StringComparison.Ordinal)))
			{
				post_link = "http://" + post_link;
			}

			var new_article = new Article
			{
				id = article.id,
				title = title.Text,
				notes = notes.Text,
				link = post_link
			};

			bool added = await articleController.UpdateArticleRemotely(new_article);

			if (added)
			{
				await Navigation.PopModalAsync();
			}
			else 
			{
				await DisplayAlert("Articles", "Article could not updated.", "OK");
				saveButton.IsEnabled = true;
			}
		}
	}
}