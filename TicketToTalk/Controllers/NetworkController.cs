using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Controller to handle network requests.
	/// </summary>
	public class NetworkController
	{
		HttpClient client;
		string URLBase = Session.baseUrl;

		private readonly string api_key = "a82ae536fc32c8c185920f3a440b0984bb51b9077517a6c8ce4880e41737438d";

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NetworkController"/> class.
		/// </summary>
		public NetworkController()
		{
			client = new HttpClient();
			client.DefaultRequestHeaders.Host = "danielwelsh.uk";
			client.Timeout = new TimeSpan(0, 0, 100);
		}

		/// <summary>
		/// Sends the get request.
		/// </summary>
		/// <returns>The get request.</returns>
		/// <param name="URL">URL.</param>
		/// <param name="parameters">Parameters.</param>
		public async Task<JObject> SendGetRequest(string URL, IDictionary<string, string> parameters)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			client.Timeout = new TimeSpan(0, 0, 100);

			parameters["api_key"] = Session.activeUser.api_key;

			URL += "?";
			foreach (KeyValuePair<string, string> entry in parameters)
			{
				URL += Uri.EscapeUriString(entry.Key);
				URL += "=";
				URL += Uri.EscapeUriString(entry.Value);
				URL += "&";
			}
			URL = URL.Substring(0, URL.Length - 1);
			var uri = new Uri(URLBase + URL);
			Console.WriteLine("Sending request to: " + uri);

			HttpResponseMessage response = null;

			// Get response
			try
			{
				response = await client.GetAsync(uri);
				Debug.WriteLine(response);
			}
			catch (TaskCanceledException ex)
			{
				Debug.WriteLine("NetworkController: Network Timeout");
				Debug.WriteLine(ex);
			}

			// Check for success.
			if (response == null)
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("NetworkController: Response - " + response.StatusCode);
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				Console.WriteLine("Response:" + response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Sends the post request.
		/// </summary>
		/// <returns>The post request.</returns>
		/// <param name="URL">URL.</param>
		/// <param name="parameters">Parameters.</param>
		public async Task<JObject> SendPostRequest(string URL, IDictionary<string, string> parameters)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			client.Timeout = new TimeSpan(0, 0, 100);

			if (Session.activeUser == null)
			{
				parameters["api_key"] = api_key;
			}
			else 
			{
				parameters["api_key"] = Session.activeUser.api_key;	
			}

			var uri = new Uri(URLBase + URL);
			Debug.WriteLine(uri);

			// Create json content for parameters.
			string jsonCredentials = JsonConvert.SerializeObject(parameters);
			Debug.WriteLine("NetworkController: " + jsonCredentials);
			HttpContent content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

			//var response = null;
			HttpResponseMessage response = null;
			try
			{
				// Get response
				response = await client.PostAsync(uri, content);
				Debug.WriteLine(response.ToString());
			}
			catch (WebException ex)
			{
				Debug.WriteLine("Network Timeout");
				Debug.WriteLine(ex);
			}
			catch (TaskCanceledException ex)
			{
				Debug.WriteLine("Network Timeout");
				Debug.WriteLine(ex);
			}

			// Check for success.
			if (response == null)
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Request:" + response.StatusCode);
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				Console.WriteLine("Request:" + response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Sends a post request with generic parameters.
		/// </summary>
		/// <returns>The generic post request.</returns>
		/// <param name="URL">URL.</param>
		/// <param name="parameters">Parameters.</param>
		public async Task<JObject> SendGenericPostRequest(string URL, IDictionary<string, object> parameters)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			client.Timeout = new TimeSpan(0, 0, 100);

			// Use default key, this method is called when registering with a photo.
			if (Session.activeUser == null)
			{
				parameters["api_key"] = api_key;
			}
			else
			{
				parameters["api_key"] = Session.activeUser.api_key;
			}

			var uri = new Uri(URLBase + URL);
			Debug.WriteLine("NetworkController: " + uri);

			// Create json content for parameters.
			string jsonCredentials = JsonConvert.SerializeObject(parameters);
			//Console.WriteLine(jsonCredentials);
			HttpContent content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

			// Get response
			var response = await client.PostAsync(uri, content);

			// Check for success.
			if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("Request:" + response.StatusCode);
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				Debug.WriteLine("Request:" + response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Sends the delete request.
		/// </summary>
		/// <returns>The delete request.</returns>
		/// <param name="URL">URL.</param>
		/// <param name="parameters">Parameters.</param>
		public async Task<JObject> SendDeleteRequest(string URL, IDictionary<string, string> parameters)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			client.Timeout = new TimeSpan(0, 0, 100);

			parameters["api_key"] = Session.activeUser.api_key;

			URL += "?";
			foreach (KeyValuePair<string, string> entry in parameters)
			{
				URL += Uri.EscapeUriString(entry.Key);
				URL += "=";
				URL += Uri.EscapeUriString(entry.Value);
				URL += "&";
			}
			URL = URL.Substring(0, URL.Length - 1);
			var uri = URLBase + URL;

			Debug.WriteLine("NetworkController: Sending delete request to: " + uri);

			// Get response
			HttpResponseMessage response = null;
			try
			{
				response = await client.DeleteAsync(uri);
			}
			catch (WebException ex)
			{
				Debug.WriteLine("NetworkController: Network Timeout");
				Debug.WriteLine("NetworkController:" + ex);
			}
			catch (TaskCanceledException ex)
			{
				Debug.WriteLine("NetworkController: Network Timeout");
				Debug.WriteLine("NetworkController:" + ex);
			}

			if (response == null)
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				Debug.WriteLine("NewtorkController: Request = " + response.StatusCode);
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				Debug.WriteLine("NewtorkController: Request = " + response.StatusCode);
				return null;
			}
		}

		/// <summary>
		/// Downloads the file.
		/// </summary>
		/// <returns>The file.</returns>
		/// <param name="path">Path.</param>
		/// <param name="fileName">File name.</param>
		public async Task<bool> DownloadFile(string path, string fileName)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Host = "tickettotalk.openlab.ncl.ac.uk";
			client.Timeout = new TimeSpan(0, 0, 100);

			Debug.WriteLine("NetworkController: Beginning Download");
			var webClient = new WebClient();

			var url = new Uri(Session.baseUrl + "media/get?fileName=" + path + "&token=" + Session.Token.val) + "&api_key=" + Session.activeUser.api_key;
			Debug.WriteLine(url);

			var returned = await webClient.DownloadDataTaskAsync(url);
			if (returned != null)
			{
				Debug.WriteLine("NetworkController: Downloaded image - " + returned.HashArray());
				MediaController.WriteImageToFile(fileName, returned);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Handles the session expiration.
		/// </summary>
		public void HandleSessionExpiration()
		{
			Application.Current.MainPage = new Login();
			Session.activePerson = null;
			Session.activeUser = null;
			Session.Token.val = null;
		}
	}
}

