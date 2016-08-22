using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
	/// Controller for network operations.
	/// </summary>
	public class NetworkController
	{
		HttpClient client;
		string URLBase = Session.baseUrl;

		public NetworkController()
		{
			client = new HttpClient();
			client.DefaultRequestHeaders.Host = "danielwelsh.uk";
			client.Timeout = new TimeSpan(0, 0, 100);
		}

		/// <summary>
		/// Gets authentication token from the server.
		/// </summary>
		/// <returns>The token</returns>
		// TODO: Encrypt password.
		public async Task<Token> getToken()
		{
			string URL = Session.baseUrl + "/api/auth/login";
			var uri = new Uri(URL);
			Debug.WriteLine(uri);

			IDictionary<string, string> credentials = new Dictionary<string, string>();
			//credentials["email"] = email.Text;
			//credentials["password"] = password.Text;

			string paramaters = JsonConvert.SerializeObject(credentials);
			Console.WriteLine(paramaters);
			HttpContent content = new StringContent(paramaters, Encoding.UTF8, "application/json");

			var response = await client.PostAsync(uri, content);
			string jsonString = await response.Content.ReadAsStringAsync();
			Console.WriteLine(jsonString);
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Success: Token Generated");
				var tokenVal = JsonConvert.DeserializeObject<string>(jsonString);
				return new Token { val = tokenVal };
			}
			else
			{
				string error = JsonConvert.DeserializeObject<string>(jsonString);
				Console.WriteLine(error);
				return null;
			}
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
				Console.WriteLine("Network Timeout");
				Debug.WriteLine(ex);
			}

			// Check for success.
			if (response == null) 
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Response:" + response.StatusCode);
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
			Console.WriteLine(jsonCredentials);
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
				Console.WriteLine("Network Timeout");
				Debug.WriteLine(ex);
			}
			catch (TaskCanceledException ex) 
			{
				Console.WriteLine("Network Timeout");
				Debug.WriteLine(ex);
			}

			// Check for success.
			if (response == null) 
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Request:" + response.StatusCode);
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
		/// Sends the generic post request.
		/// </summary>
		/// <returns>The generic post request.</returns>
		/// <param name="URL">URL.</param>
		/// <param name="parameters">Parameters.</param>
		public async Task<JObject> sendGenericPostRequest(string URL, IDictionary<string, Object> parameters)
		{
			var uri = new Uri(URLBase + URL);

			// Create json content for parameters.
			string jsonCredentials = JsonConvert.SerializeObject(parameters);
			//Console.WriteLine(jsonCredentials);
			HttpContent content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

			// Get response
			var response = await client.PostAsync(uri, content);

			// Check for success.
			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Request:" + response.StatusCode);
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

			Console.WriteLine("NetworkController: Sending delete request to: " + uri);

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

		public bool downloadImage(string path, string fileName) 
		{
			Debug.WriteLine("Beginning Download");
			var webClient = new WebClient();
			var success = true;
			//webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
			webClient.DownloadDataCompleted += (s, e) =>
			{
				try
				{
					var bytes = e.Result; // get the downloaded data
					string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					string localPath = Path.Combine(documentsPath, fileName);
					Debug.WriteLine(localPath);
					File.WriteAllBytes(localPath, bytes); // writes to local storage

					var finished = true;
					MessagingCenter.Send<NetworkController, bool>(this, "download_image", finished);
				}
				catch (Exception ex)
				{
					Debug.WriteLine("NetworkController: Image not downloaded.");
					Debug.WriteLine(String.Format("NetworkController: {0}", ex));
					success = false;
				}
			};

			var url = new Uri(Session.baseUrl + "media/get?fileName=" + path + "&token=" + Session.Token.val);
			Debug.WriteLine(url);
			//await Task.Run(() => webClient.DownloadData(url));
			webClient.DownloadDataAsync(url);

			return success;
		}
	}
}

