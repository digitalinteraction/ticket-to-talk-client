using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Ticket cell.
	/// </summary>
	public class TicketCell : ImageCell
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketCell"/> class.
		/// </summary>
		public TicketCell()
		{
			var deleteCell = new MenuItem
			{
				Text = "Delete",
				IsDestructive = true
			};
			deleteCell.Clicked += DeleteCell_Clicked;

			ContextActions.Add(deleteCell);
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
				var deleted = await ticketController.DestroyTicket(ticket);
			}
			catch (NoNetworkException ex)
			{
				await Application.Current.MainPage.DisplayAlert("No Network", ex.Message, "Dismiss");
			}
		}
	}
}


