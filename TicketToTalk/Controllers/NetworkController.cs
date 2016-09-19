using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TicketToTalk
{
	/// <summary>
	/// Controller to handle network requests.
	/// </summary>
	public class NetworkController
	{
		HttpClient client;
		string URLBase = Session.baseUrl;

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
		public async Task<JObject> sendGetRequest(string URL, IDictionary<string, string> parameters)
		{
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
		public async Task<JObject> sendPostRequest(string URL, IDictionary<string, string> parameters)
		{
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
		public async Task<JObject> sendGenericPostRequest(string URL, IDictionary<string, object> parameters)
		{
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
		public async Task<JObject> sendDeleteRequest(string URL, IDictionary<string, string> parameters)
		{
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

		public async Task<bool> downloadFile(string path, string fileName)
		{
			Debug.WriteLine("NetworkController: Beginning Download");
			var webClient = new WebClient();

			var url = new Uri(Session.baseUrl + "media/get?fileName=" + path + "&token=" + Session.Token.val);
			Debug.WriteLine(url);

			var returned = await webClient.DownloadDataTaskAsync(url);
			if (returned != null)
			{
				Debug.WriteLine("NetworkController: Downloaded image - " + returned.HashArray());
				MediaController.writeImageToFile(fileName, returned);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

