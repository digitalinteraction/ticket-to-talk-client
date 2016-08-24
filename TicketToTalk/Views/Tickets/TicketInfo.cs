﻿using System;

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
				//Margin = new Thickness(0,10,0,0),
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

			var areaController = new AreaController();
			var area = areaController.getArea(ticket.area_id);
			var location = new Label 
			{
				FontSize = 14,
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 0),
				HorizontalTextAlignment = TextAlignment.Center
			};
			switch (ticket.mediaType) 
			{
				case ("Picture"):
					location.Text = String.Format("Taken in {0}, {1}", area.townCity, ticket.year);
					break;
				default:
					location.Text = String.Format("From {0}", ticket.year);
					break;
			}

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


