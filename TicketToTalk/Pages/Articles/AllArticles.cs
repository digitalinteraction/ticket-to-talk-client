using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View a list of all articles.
	/// </summary>
	public class AllArticles : ContentPage
	{
		public static ObservableCollection<Article> serverArticles = new ObservableCollection<Article>();
		private ArticleController articleController = new ArticleController();
		List<Article> sharedArticles;

		/// <summary>
		/// Creates an instance of all articles view.
		/// </summary>
		public AllArticles()
		{
			serverArticles.Clear();
			Title = "Articles";

			serverArticles = Task.Run(() => this.checkForNewArticles()).Result;
			sharedArticles = Task.Run(() => articleController.getSharedArticles()).Result;

			foreach (Article a in serverArticles) 
			{
				Console.WriteLine(a);
			}

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Icon = "add_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(launchAddArticleView)
			});

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "View Shared Articles",
				Order = ToolbarItemOrder.Secondary,
				Command = new Command(viewShared),
			});

			// Format image cell
			var cell = new DataTemplate(typeof(ArticleCell));
			cell.SetBinding(TextCell.TextProperty, new Binding("title"));
			cell.SetBinding(TextCell.DetailProperty, new Binding("link"));

			ListView articleList = new ListView();
			articleList.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			articleList.BindingContext = serverArticles;
			articleList.ItemTemplate = cell;
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
		void viewShared()
		{
			if (sharedArticles != null && sharedArticles.Count > 0)
			{
				Navigation.PushAsync(new ViewSharedArticles(sharedArticles));
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
		public void launchAddArticleView() 
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
		public async Task<ObservableCollection<Article>> checkForNewArticles()
		{
			NetworkController net = new NetworkController();
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var jobject = await net.sendGetRequest("articles/all", parameters);
			Console.WriteLine(jobject);
			var jarticles = jobject.GetValue("articles");
			Console.WriteLine(jarticles);
			var articles = jarticles.ToObject<Article[]>();

			ObservableCollection<Article> list = new ObservableCollection<Article>();
			foreach (Article a in articles)
			{
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
		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			Article a = (Article) e.SelectedItem;

			Navigation.PushAsync(new ViewArticle(a));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}
	}
}


