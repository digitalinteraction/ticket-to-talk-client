using System;
using System.ComponentModel;
namespace TicketToTalk
{
	/// <summary>
	/// Ticket view model.
	/// </summary>
	public class TicketViewModel : INotifyPropertyChanged
	{

		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string MediaType { get; set; }
		public DateTime DateAdded { get; set; }
		public string Year { get; set; }
		public string PathToFile { get; set; }
		public string DisplayName { get; set; }
		public string DisplayIcon { get; set; }

		/// <summary>
		/// Initializes a new instance of the ticket view model.
		/// </summary>
		public TicketViewModel()
		{
		}

		/// <summary>
		/// Occurs when property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	}
}

