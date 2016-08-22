using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Article cell.
	/// </summary>
	public class ArticleCell : TextCell
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

			articleController.deleteArticleLocally(article);
			articleController.deleteArticleRemotely(article);

			AllArticles.serverArticles.Remove(article);
		}
	}
}

