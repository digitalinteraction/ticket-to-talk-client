﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View a list of all articles.
	/// </summary>
	public class AllArticles : ContentPage
	{
		public static ObservableCollection<Article> ServerArticles = new ObservableCollection<Article>();
		private ArticleController articleController = new ArticleController();
		public static List<Article> SharedArticles;
		public static bool TutorialShown = false;

		/// <summary>
		/// Creates an instance of all articles view.
		/// </summary>
		public AllArticles()
		{
			ServerArticles.Clear();
			Title = "Articles";

			ServerArticles = Task.Run(() => this.CheckForNewArticles()).Result;
			SharedArticles = Task.Run(() => articleController.GetSharedArticles()).Result;

			foreach (Article a in ServerArticles)
			{
				Console.WriteLine(a);
			}

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Icon = "add_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(LaunchAddArticleView)
			});

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "View Shared Articles",
				Order = ToolbarItemOrder.Secondary,
				Command = new Command(ViewShared),
			});

			// Format image cell
			ListView articleList = new ListView();
			articleList.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			articleList.BindingContext = ServerArticles;
			articleList.ItemTemplate = new DataTemplate(typeof(ArticleCell));
			articleList.SeparatorColor = Color.Transparent;
			articleList.ItemSelected += OnSelection;

			Content = new StackLayout
			{
				Children = {
					articleList
				}
			};
		}

		/// <summary>
		/// View shared articles.
		/// </summary>
		private void ViewShared()
		{
			if (SharedArticles != null && SharedArticles.Count > 0)
			{
				Navigation.PushAsync(new ViewSharedArticles(SharedArticles));
			}
			else
			{
				DisplayAlert("Articles", "You have not been sent any articles", "OK");
			}
		}

		/// <summary>
		/// Launchs the add article view.
		/// </summary>
		/// <returns>The add article view.</returns>
		public void LaunchAddArticleView()
		{
			var nav = new NavigationPage(new AddArticle(null));
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Checks for new articles.
		/// </summary>
		/// <returns>The for new articles.</returns>
		public async Task<ObservableCollection<Article>> CheckForNewArticles()
		{
			NetworkController net = new NetworkController();
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var jobject = await net.SendGetRequest("articles/all", parameters);
			Console.WriteLine(jobject);
			var jarticles = jobject.GetValue("articles");
			Console.WriteLine(jarticles);
			var articles = jarticles.ToObject<Article[]>();

			ObservableCollection<Article> list = new ObservableCollection<Article>();
			foreach (Article a in articles)
			{
				Debug.WriteLine("AllArticles: Parsing link: " + a.link);
				a.favicon = articleController.GetFaviconURL(a.link);
				list.Add(a);
			}

			return list;
		}

		/// <summary>
		/// Launch article view on article selection
		/// </summary>
		/// <returns>The selection.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			Article a = (Article)e.SelectedItem;

			Navigation.PushAsync(new ViewArticle(a));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Session.activeUser.firstLogin && !TutorialShown)
			{

				var text = "Use Articles to store useful information about the person you're collecting tickets for. Click '+' to get started!";

				Navigation.PushModalAsync(new HelpPopup(text, "file_white_icon.png"));
				TutorialShown = true;
			}
		}
	}
}


