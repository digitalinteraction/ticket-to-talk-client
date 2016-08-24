using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
		/// Adds the article locally.
		/// </summary>
		/// <param name="article">Article.</param>
		public void addArticleLocally(Article article) 
		{
			articleDB.open();

			if(articleDB.GetArticle(article.id) == null) 
			{
				articleDB.AddArticle(article);
			}

			articleDB.close();
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
				return true;
			}
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// Destory the article.
		/// </summary>
		/// <returns><c>true</c>, if article was destoried, <c>false</c> otherwise.</returns>
		/// <param name="article">Article.</param>
		public bool destoryArticle(Article article) 
		{
			if (deleteArticleRemotely(article))
			{
				deleteArticleLocally(article);
				AllArticles.serverArticles.Remove(article);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the shared articles.
		/// </summary>
		/// <returns>The shared articles.</returns>
		public async System.Threading.Tasks.Task<List<Article>> getSharedArticles()
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.sendGetRequest("articles/share/get", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("Articles");
				return jtoken.ToObject<List<Article>>();
			}

			return null;
		}

		/// <summary>
		/// Adds the shared.
		/// </summary>
		/// <returns>The shared.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> addShared(Article article)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;
			parameters["article_id"] = article.id.ToString();

			var jobject = await networkController.sendPostRequest("articles/share/accept", parameters);
			if (jobject != null) 
			{
				Debug.WriteLine("ArticleController: " + article);
				addArticleLocally(article);
				AllArticles.serverArticles.Add(article);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Rejects the shared article.
		/// </summary>
		/// <returns>The shared.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> rejectShared(Article article) 
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.sendPostRequest("articles/share/reject", parameters);
			if (jobject != null) 
			{
				return true;
			}

			return false;
		}
	}
}

