using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Article cell.
	/// </summary>
	public class ArticleCell : ViewCell
	{
		ArticleController articleController = new ArticleController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ArticleCell"/> class.
		/// </summary>
		public ArticleCell()
		{
			var deleteCell = new MenuItem
			{
				Text = "Delete",
				IsDestructive = true
			};
			deleteCell.Clicked += DeleteCell_Clicked;

			ContextActions.Add(deleteCell);

			var faviconImage = new Image
			{
				HeightRequest = 25,
				WidthRequest = 25,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			faviconImage.SetBinding(Image.SourceProperty, "favicon");

			var title = new Label
			{
				FontSize = 14,
				TextColor = ProjectResource.color_dark,
			};
			title.SetBinding(Label.TextProperty, "title");

			var link = new Label
			{
				FontSize = 12,
				TextColor = ProjectResource.color_blue,
			};
			link.SetBinding(Label.TextProperty, "link");

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(10, 0, 0, 0),
				Spacing = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					title,
					link
				}
			};

			var cellLayout = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(10, 5, 10, 5),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					faviconImage,
					detailsStack
				}
			};

			this.View = cellLayout;
		}

		/// <summary>
		/// Deletes the cell clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void DeleteCell_Clicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
			var article = (Article)mi.BindingContext;

			Debug.WriteLine("ArticleCell: Binding context - " + article);

			articleController.DeleteArticleLocally(article);
			articleController.DeleteArticleRemotely(article);

			AllArticles.ServerArticles.Remove(article);
		}
	}
}

