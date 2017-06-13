// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 13/06/2017
//
// StyledTicketCell.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
    public class StyledTicketCell : ViewCell
    {
        private Image ticketIcon;
        private Label titleLabel;
        private Label year;

        public StyledTicketCell()
        {
			var deleteCell = new MenuItem
			{
				Text = "Delete",
				IsDestructive = true
			};
			deleteCell.Clicked += DeleteCell_Clicked;

			ContextActions.Add(deleteCell);

			ticketIcon = new Image
			{
				HeightRequest = 50,
				WidthRequest = 50,
				Aspect = Aspect.AspectFill,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
            ticketIcon.SetBinding(Image.SourceProperty, "imageSource");

			titleLabel = new Label
			{
			};
			titleLabel.SetSubHeaderStyle();
			titleLabel.VerticalOptions = LayoutOptions.Start;
			titleLabel.SetBinding(Label.TextProperty, "title");

			year = new Label
			{
			};
			year.SetBodyStyle();
			year.TextColor = ProjectResource.color_red;
			year.VerticalOptions = LayoutOptions.Start;
			year.SetBinding(Label.TextProperty, "year");

			var detailsStack = new StackLayout
			{
				Padding = new Thickness(10, 0, 0, 0),
				Spacing = 2,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					titleLabel,
                    year
				}
			};

			var cellLayout = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(10, 5, 10, 5),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
				{
					ticketIcon,
					detailsStack
				}
			};
			this.View = cellLayout;
        }

		/// <summary>
		/// Deletes the cell clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void DeleteCell_Clicked(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);

			var ticket = (Ticket)mi.BindingContext;

			var ticketController = new TicketController();

			try
			{
				await ticketController.DestroyTicket(ticket);
			}
			catch (NoNetworkException ex)
			{
				await Application.Current.MainPage.DisplayAlert("No Network", ex.Message, "Dismiss");
			}
		}
    }
}

