// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 24/08/2016
//
// ViewArticles.cs
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View articles.
	/// </summary>
	public class ViewSharedArticles : ContentPage
	{

		public static ObservableCollection<Article> articles = new ObservableCollection<Article>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ViewArticles"/> class.
		/// </summary>
		public ViewSharedArticles(List<Article> shared)
		{
			articles.Clear();
			Title = "Shared Articles";

			foreach (Article a in shared)
			{
				articles.Add(a);
			}

			var cell = new DataTemplate(typeof(ArticleCell));
			cell.SetBinding(TextCell.TextProperty, new Binding("title"));
			cell.SetBinding(TextCell.DetailProperty, new Binding("link"));

			ListView articleList = new ListView();
			articleList.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			articleList.BindingContext = articles;
			articleList.ItemTemplate = cell;
			articleList.SeparatorColor = Color.Transparent;
			articleList.ItemSelected += OnSelection;

			Content = new StackLayout 
			{
				Spacing = 0,
				Children = 
				{
					articleList
				}
			};
		}

		/// <summary>
		/// Ons the selection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			Article a = (Article)e.SelectedItem;

			var nav = new NavigationPage(new ViewSharedArticle(a));
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}
	}
}


