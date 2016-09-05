using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Ticket info.
	/// </summary>
	public class TicketInfo : ContentView
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketInfo"/> class.
		/// </summary>
		public TicketInfo(Ticket ticket)
		{
			Padding = new Thickness(10);

			var title = new Label
			{
				Text = ticket.title,
				FontSize = 18,
				FontAttributes = FontAttributes.Bold,
				TextColor = ProjectResource.color_dark,
				HorizontalTextAlignment = TextAlignment.Center,
			};

			var description = new Label 
			{
				Text = ticket.description,
				FontSize = 14,
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0),
				HorizontalTextAlignment = TextAlignment.Center
			};

			var location = new Label 
			{
				FontSize = 14,
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0),
				HorizontalTextAlignment = TextAlignment.Center
			};
			location.SetBinding(Label.TextProperty, "displayString");
			location.BindingContext = ticket;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					title, 
					description, 
					location
				}
			};
		}
	}
}


