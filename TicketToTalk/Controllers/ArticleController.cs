using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TicketToTalk
{
	/// <summary>
	/// Controller for the Article Model.
	/// </summary>
	public class ArticleController
	{
		private ArticleDB articleDB = new ArticleDB();
		private NetworkController networkController = new NetworkController();

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
		public void AddArticleLocally(Article article)
		{
			articleDB.Open();

			// Checks if the article already exists, if null, the article is added.
			if (articleDB.GetArticle(article.id) == null)
			{
				articleDB.AddArticle(article);
			}

			articleDB.Close();
		}

		/// <summary>
		/// Deletes the article locally.
		/// </summary>
		/// <param name="article">Article.</param>
		public void DeleteArticleLocally(Article article)
		{
			articleDB.Open();
			articleDB.DeleteArticle(article.id);
			articleDB.Close();
		}

		/// <summary>
		/// Deletes the article remotely.
		/// </summary>
		/// <returns><c>true</c>, if article was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="article">Article.</param>
		public bool DeleteArticleRemotely(Article article)
		{
			// Create parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["token"] = Session.Token.val;

			// Sends delete request.
			var jobject = networkController.SendDeleteRequest("articles/destroy", parameters);

			// If null, request failed.
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
		public bool DestoryArticle(Article article)
		{

			// If article was destroyed remotely, remove the article locally.
			if (DeleteArticleRemotely(article))
			{
				DeleteArticleLocally(article);
				AllArticles.ServerArticles.Remove(article);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets articles that have been shared with the user.
		/// </summary>
		/// <returns>The shared articles.</returns>
		public async Task<List<Article>> GetSharedArticles()
		{
			// Build parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			// Sends the request.
			var jobject = await networkController.SendGetRequest("articles/share/get", parameters);

			// If null the request failed.
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("Articles");
				return jtoken.ToObject<List<Article>>();
			}
			return null;
		}

		/// <summary>
		/// Adds an article that was shared with the user.
		/// </summary>
		/// <returns>The shared article.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> AcceptSharedArticle(Article article)
		{
			// Builds the parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;
			parameters["article_id"] = article.id.ToString();

			// Sends the request.
			var jobject = await networkController.SendPostRequest("articles/share/accept", parameters);

			// If null the request failed.
			if (jobject != null)
			{
				AddArticleLocally(article);

				// Add article to the list of displayed articles.
				AllArticles.ServerArticles.Add(article);

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
		public async Task<bool> ShareArticle(Article article, string email, bool includeNotes)
		{
			// TODO: Stop crash if recipient is not registered with the system.
			// Build the paramters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["email"] = email;
			parameters["includeNotes"] = includeNotes.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.SendPostRequest("articles/share/send", parameters);

			// If null, the request failed.
			if (jobject != null)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Adds the article to the remote server
		/// </summary>
		/// <returns><c>true</c>, if article remotely was added, <c>false</c> otherwise.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> AddArticleRemotely(Article article)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["title"] = article.title;
			parameters["notes"] = article.notes;
			parameters["link"] = article.link;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.SendPostRequest("articles/store", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("article");
				var returned_article = jtoken.ToObject<Article>();
				Debug.WriteLine("Saved Article: " + article);

				AddArticleLocally(returned_article);

				article.favicon = GetFaviconURL(article.link);
				AllArticles.ServerArticles.Add(article);

				return true;
			}
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// Updates the article remotely.
		/// </summary>
		/// <returns>The article remotely.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> UpdateArticleRemotely(Article article)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["title"] = article.title;
			parameters["notes"] = article.notes;
			parameters["link"] = article.link;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.SendPostRequest("articles/update", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("article");
				var new_article = jtoken.ToObject<Article>();
				Debug.WriteLine("Saved Article: " + new_article);

				new_article.favicon = GetFaviconURL(new_article.link);
				UpdateArticleLocally(new_article);

				var idx = -1;
				for (int i = 0; i < AllArticles.ServerArticles.Count; i++)
				{
					if (new_article.id == AllArticles.ServerArticles[i].id)
					{
						idx = i;
						break;
					}
				}

				AllArticles.ServerArticles[idx] = new_article;
				ViewArticle.currentArticle.title = new_article.title;
				ViewArticle.currentArticle.link = new_article.link;
				ViewArticle.currentArticle.notes = new_article.notes;

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Rejects the shared article.
		/// </summary>
		/// <returns>The shared.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> RejectShared(Article article)
		{
			// Build the parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			var jobject = await networkController.SendPostRequest("articles/share/reject", parameters);

			// If null, the request failed.
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
		public string GetFaviconURL(string url)
		{
			var idx = GetBaseURLIndex(url);
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
		private int GetBaseURLIndex(string url)
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
		public void UpdateArticleLocally(Article article)
		{
			articleDB.Open();
			articleDB.DeleteArticle(article.id);
			articleDB.AddArticle(article);
			articleDB.Close();
		}
	}
}

