using System;
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
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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

#if __Android__
			client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler ());
#endif
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

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

			HttpResponseMessage response = null;

			// Get response
			try
			{
				response = await client.GetAsync(uri);
			}
			catch (TaskCanceledException ex)
			{
				Console.WriteLine("Network Timeout");
			}

			Debug.WriteLine(response);

			// Check for success.
			if (response == null)
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
				{
					HandleSessionExpiration();
				}
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

#if __ANDROID__
			client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler ());
#endif

			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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

			// Create json content for parameters.
			string jsonCredentials = JsonConvert.SerializeObject(parameters);
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
			}
			catch (TaskCanceledException ex)
			{
				Console.WriteLine(ex);
			}

			Debug.WriteLine(response);

			// Check for success.
			if (response == null)
			{
				return null;
			}
			else if (response.IsSuccessStatusCode)
			{
				string jsonString = await response.Content.ReadAsStringAsync();
				Debug.WriteLine("Success");
				Debug.WriteLine(jsonString);
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
				{
					HandleSessionExpiration();
				}
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

#if __Android__
			client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler ());
#endif
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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

			// Create json content for parameters.
			string jsonCredentials = JsonConvert.SerializeObject(parameters);
			HttpContent content = new StringContent(jsonCredentials, Encoding.UTF8, "application/json");

			// Get response
			var response = await client.PostAsync(uri, content);
			Debug.WriteLine(response);
			// Check for success.
			if (response.IsSuccessStatusCode)
			{
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{

				if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
				{
					HandleSessionExpiration();
				}
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
#if __Android__
			client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler ());
#endif
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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

			// Get response
			HttpResponseMessage response = null;
			try
			{
				response = await client.DeleteAsync(uri);
			}
			catch (WebException ex)
			{
				Console.WriteLine("Network Timeout");
			}
			catch (TaskCanceledException ex)
			{
			}
			Debug.WriteLine(response);

			if (response == null)
			{
				return null;
			}
			if (response.IsSuccessStatusCode)
			{
				string jsonString = await response.Content.ReadAsStringAsync();
				JObject jobject = JObject.Parse(jsonString);
				return jobject;
			}
			else
			{
				if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
				{
					HandleSessionExpiration();
				}
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
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			client.Timeout = new TimeSpan(0, 0, 100);

			var url = new Uri(Session.baseUrl + "media/get?fileName=" + path + "&token=" + Session.Token.val) + "&api_key=" + Session.activeUser.api_key;

			Console.WriteLine("Beginning Download");
			var returned = await client.GetStreamAsync(url);
			byte[] buffer = new byte[16 * 1024];
			byte[] imageBytes;
			using (MemoryStream ms = new MemoryStream())
			{
				int read = 0;
				while ((read = returned.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				imageBytes = ms.ToArray();
			}

			if (returned != null)
			{
				MediaController.WriteImageToFile(fileName, imageBytes);
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
			Session.activePerson = null;
			Session.activeUser = null;
			Session.Token.val = null;

			var nav = new NavigationPage(new Login());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Application.Current.MainPage = nav;
		}
	}
}

