// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 22/08/2016
//
// YouTubePlayer.cs
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// You tube player.
	/// </summary>
	public class YouTubePlayer : ContentView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.YouTubePlayer"/> class.
		/// </summary>
		public YouTubePlayer(string videoCode)
		{
			Debug.WriteLine("YouTubePlayer: string code = " + videoCode);

			var height = (double)Session.ScreenWidth * 0.5625;

			Padding = new Thickness(0);
			var url = string.Format(
				"<!DOCTYPE html>" +
					"<body>" +
				"<iframe src='https://www.youtube.com/embed/{0}' frameborder='0' width='{1}' height={2} allowfullscreen>" +
						"</iframe>" +
					"</body>" +
				"</html>",
				videoCode, Session.ScreenWidth, height);

			Debug.WriteLine("YouTubePlayer: Embeded URL = " + url);

			var webView = new WebView
			{
				Source = new HtmlWebViewSource
				{
					Html = url
				},
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = Session.ScreenWidth / 1.6
			};

			var stack = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					webView
				}
			};

			Content = stack;
		}
	}
}


