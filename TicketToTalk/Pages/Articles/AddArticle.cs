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
		Entry title;
		Editor notes;
		Entry link;

		Article article = null;
		Button saveButton;

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
				saveButton.Clicked += saveArticle;
			}
			else 
			{
				saveButton.Text = "Update";
				saveButton.Clicked += updateArticle;
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
				//BackgroundColor = ProjectResource.color_blue,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Children =
				{
					saveButton
				}
			};

			Content = new StackLayout
			{
				Spacing = 0,
				//VerticalOptions = LayoutOptions.FillAndExpand,
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
			var entriesNotNull = (!String.IsNullOrEmpty(title.Text))
				&& (!String.IsNullOrEmpty(notes.Text))
				&& (!String.IsNullOrEmpty(link.Text));

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
		public async void saveArticle(object sender, EventArgs e) 
		{
			saveButton.IsEnabled = false;

			var post_link = link.Text.ToLower();
			if (!(post_link.StartsWith("http://"))) 
			{
				post_link = "http://" + post_link;
			}

			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["title"] = title.Text;
			parameters["notes"] = notes.Text;
			parameters["link"] = post_link;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.sendPostRequest("articles/store", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("article");
				var article = jtoken.ToObject<Article>();
				Debug.WriteLine("Saved Article: " + article);

				ArticleDB aDB = new ArticleDB();
				aDB.open();
				aDB.AddArticle(article);
				aDB.close();

				AllArticles.serverArticles.Add(article);

				await Navigation.PopModalAsync();
			}
			else 
			{
				await DisplayAlert("Articles", "Article could not be saved." ,"OK");
				saveButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// Updates the article.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void updateArticle(object sender, EventArgs e) 
		{
			var post_link = link.Text.ToLower();
			if (!(post_link.StartsWith("http://")))
			{
				post_link = "http://" + post_link;
			}

			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["title"] = title.Text;
			parameters["notes"] = notes.Text;
			parameters["link"] = post_link;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.sendPostRequest("articles/update", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("article");
				var new_article = jtoken.ToObject<Article>();
				Debug.WriteLine("Saved Article: " + new_article);

				ArticleDB aDB = new ArticleDB();
				aDB.open();
				aDB.DeleteArticle(article.id);
				aDB.AddArticle(new_article);
				aDB.close();

				var idx = AllArticles.serverArticles.IndexOf(article);
				AllArticles.serverArticles[idx] = new_article;

				ViewArticle.currentArticle.title = new_article.title;
				ViewArticle.currentArticle.link = new_article.link;
				ViewArticle.currentArticle.notes = new_article.notes;

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