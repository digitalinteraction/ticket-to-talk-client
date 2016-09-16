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

			if (articleDB.GetArticle(article.id) == null)
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
		/// Gets the articles shared with the user.
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
		/// Adds the shared article.
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
		/// Shares the article.
		/// </summary>
		/// <returns>The article.</returns>
		/// <param name="article">Article.</param>
		/// <param name="email">Email.</param>
		/// <param name="includeNotes">If set to <c>true</c> include notes.</param>
		public async Task<bool> shareArticle(Article article, string email, bool includeNotes)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["email"] = email;
			parameters["includeNotes"] = includeNotes.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.sendPostRequest("articles/share/send", parameters);
			if (jobject != null)
			{
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

		/// <summary>
		/// Gets the favicon URL of an article link.
		/// </summary>
		/// <returns>The favicon URL.</returns>
		/// <param name="url">URL.</param>
		public string getFaviconURL(string url)
		{
			var idx = getBaseURLIndex(url);
			var baseURL = url.Substring(0, idx + 1);
			if (!(baseURL.EndsWith("/", StringComparison.Ordinal)))
			{
				baseURL += "/";
			}

			return baseURL + "/favicon.ico";
		}

		/// <summary>
		/// Gets the base URL Index.
		/// </summary>
		/// <returns>The base URL Index.</returns>
		/// <param name="url">URL.</param>
		private int getBaseURLIndex(string url)
		{
			int count = 0;
			char key = '/';

			// Get third occurance of a '/'
			for (int i = 0; i < url.Length; i++)
			{
				if (url[i] == key)
				{
					count++;
					if (count == 3)
					{
						return i;
					}
				}
				else if (i == (url.Length - 1))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Updates the article locally.
		/// </summary>
		/// <param name="article">Article.</param>
		public void updateArticleLocally(Article article)
		{
			articleDB.open();
			articleDB.DeleteArticle(article.id);
			articleDB.AddArticle(article);
			articleDB.close();
		}
	}
}

