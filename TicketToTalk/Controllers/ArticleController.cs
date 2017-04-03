using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TicketToTalk
{
	/// <summary>
	/// Controller for the Article Model.
	/// </summary>
	public class ArticleController
	{
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
			lock(Session.Connection) 
			{
				Session.Connection.Insert(article);
			}
		}

		/// <summary>
		/// Deletes the article locally.
		/// </summary>
		/// <param name="article">Article.</param>
		public void DeleteArticleLocally(Article article)
		{
			lock(Session.Connection) 
			{
				Session.Connection.Delete(article);
			}
		}

		/// <summary>
		/// Deletes the article remotely.
		/// </summary>
		/// <returns><c>true</c>, if article was deleted remotely, <c>false</c> otherwise.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> DeleteArticleRemotely(Article article)
		{
			// Create parameters.
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["article_id"] = article.id.ToString();
			parameters["token"] = Session.Token.val;

			JObject jobject = null;

			// Sends delete request.
			try
			{
				jobject = await networkController.SendDeleteRequest("articles/destroy", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

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
		public async Task<bool> DestoryArticle(Article article)
		{

			bool destroyed = false;

			try
			{
				destroyed = await DeleteArticleRemotely(article);
			}
			catch (NoNetworkException ex) 
			{
				throw ex;
			}

			// If article was destroyed remotely, remove the article locally.
			if (destroyed)
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
				var data = jobject.GetData();
				var jtoken = data["articles"];
				return jtoken.ToObject<List<Article>>();
			}
			return null;
		}

		/// <summary>
		/// Gets all articles.
		/// </summary>
		/// <returns>The all articles.</returns>
		public async Task<ObservableCollection<Article>> GetAllArticles()
		{
			Console.WriteLine("Downloading Articles");

			NetworkController net = new NetworkController();
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var jobject = await net.SendGetRequest("articles/all", parameters);
			var data = jobject.GetData();
			var jarticles = data["articles"];
			var articles = jarticles.ToObject<Article[]>();

			ObservableCollection<Article> list = new ObservableCollection<Article>();
			foreach (Article a in articles)
			{
				a.favicon = GetFaviconURL(a.link);
				list.Add(a);
			}

			return list;
		}

		/// <summary>
		/// Adds an article that was shared with the user.
		/// </summary>
		/// <returns>The shared article.</returns>
		/// <param name="article">Article.</param>
		public async Task<bool> AcceptSharedArticle(Article article)
		{
			// Builds the parameters.
			IDictionary<string, object> parameters = new Dictionary<string, object>();
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
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["article_id"] = article.id.ToString();
			parameters["email"] = email;
			parameters["includeNotes"] = includeNotes.ToString();
			parameters["token"] = Session.Token.val;

			// Send the request.
			JObject jobject = null;
			try
			{
				jobject = await networkController.SendPostRequest("articles/share/send", parameters);
			}
			catch (NoNetworkException ex)
			{
				throw ex;
			}

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
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["title"] = article.title;
			parameters["notes"] = article.notes;
			parameters["link"] = article.link;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			JObject jobject = null;
			try
			{
				jobject = await net.SendPostRequest("articles/store", parameters);
			}
			catch (NoNetworkException ex) 
			{
				throw ex;
			}

			if (jobject != null)
			{
				var data = jobject.GetData();
				var jarticle = data["article"];

				var returned_article = jarticle.ToObject<Article>();
				Console.WriteLine("Saved article");

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
			Console.WriteLine("Updating article");

			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["article_id"] = article.id.ToString();
			parameters["title"] = article.title;
			parameters["notes"] = article.notes;
			parameters["link"] = article.link;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.SendPostRequest("articles/update", parameters);
			if (jobject != null)
			{
				var data = jobject.GetData();
				var jarticle = data["article"];
				var new_article = jarticle.ToObject<Article>();

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
			IDictionary<string, object> parameters = new Dictionary<string, object>();
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
			lock (Session.Connection)
			{
				Session.Connection.Update(article);
			}
		}
	}
}

