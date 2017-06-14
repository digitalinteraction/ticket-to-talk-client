// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 13/06/2017
//
// TicketPeriodCell.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
    public class TicketPeriodCell : ViewCell
    {
        private Label period;
        private Label ticketCount;

        public TicketPeriodCell()
        {
			period = new Label
			{
			};
			period.SetSubHeaderStyle();
			period.VerticalOptions = LayoutOptions.Start;
			period.SetBinding(Label.TextProperty, "text");

			ticketCount = new Label
			{
                Margin = new Thickness(0,0,0,5)
			};
			ticketCount.SetBodyStyle();
			ticketCount.TextColor = ProjectResource.color_red;
			ticketCount.VerticalOptions = LayoutOptions.Start;
			ticketCount.SetBinding(Label.TextProperty, "ticketCount");

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(10, 0, 0, 0),
				Spacing = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					period,
					ticketCount
				}
			};

            this.View = detailsStack;
        }
    }
}

