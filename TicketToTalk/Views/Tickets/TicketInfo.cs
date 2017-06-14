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
		public TicketInfo()
		{
			Padding = new Thickness(10);

			var title = new Label
			{
			};
            title.SetHeaderStyle();
			title.SetBinding(Label.TextProperty, "title");
			title.BindingContext = ViewTicket.displayedTicket;

			var description = new Label 
			{
				Margin = new Thickness(0, 10, 0, 0),
			};
            description.SetBodyStyle();
            description.HorizontalTextAlignment = TextAlignment.Center;
            description.HorizontalOptions = LayoutOptions.CenterAndExpand;
			description.SetBinding(Label.TextProperty, "description");
			description.BindingContext = ViewTicket.displayedTicket;

			var location = new Label 
			{
			};
            location.SetBodyStyle();
            location.HorizontalOptions = LayoutOptions.CenterAndExpand;
            location.HorizontalTextAlignment = TextAlignment.Center;
			location.SetBinding(Label.TextProperty, "displayString");
			location.BindingContext = ViewTicket.displayedTicket;

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


