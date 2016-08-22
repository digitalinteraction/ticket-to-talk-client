using System;
using System.Collections.Generic;

namespace TicketToTalk
{
	/// <summary>
	/// Article controller.
	/// </summary>
	public class ArticleController
	{
		ArticleDB articleDB = new ArticleDB();
		NetworkController networkController = new NetworkController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ArticleController"/> class.
		/// </summary>
		public ArticleController()
		{
		}

		/// <summary>
		/// Deletes the article locally.
		/// </summary>
		/// <param name="article">Article.</param>
		public void deleteArticleLocally(Article article)
		{
			articleDB.open();
			articleDB.DeleteArticle(article.id);
			articleDB.close();
		}

		/// <summary>
		/// Deletes the article remotely.
		/// </summary>
		/// <returns><c>true</c>, if article was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="article">Article.</param>
		public bool deleteArticleRemotely(Article article)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = networkController.sendDeleteRequest("articles/destroy", parameters);
			if (jobject != null)
			{
				return false;
			}
			else 
			{
				return true;
			}
		}
	}
}

