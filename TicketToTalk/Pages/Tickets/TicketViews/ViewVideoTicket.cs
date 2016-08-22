using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// View video ticket.
	/// </summary>
	public class ViewVideoTicket : ContentPage
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ViewVideoTicket"/> class.
		/// </summary>
		public ViewVideoTicket()
		{
			
			Padding = new Thickness(0);
			Title = "Video Ticket";
			var url = String.Format("<!DOCTYPE html><body><iframe src='https://www.youtube.com/embed/qpJHHM9IaJM' " +
			                        "frameborder='0' allowfullscreen></iframe></body></html>");

			var webView = new WebView
			{
				Source = new HtmlWebViewSource
				{
					Html = url
				},
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};


			Content = new StackLayout 
			{
				Spacing = 0,
				Children = 
				{
					webView,
				}
			};
		}
	}
}

