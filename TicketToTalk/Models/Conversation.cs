using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;

namespace TicketToTalk
{
	/// <summary>
	/// Conversation.
	/// </summary>
	public class Conversation : INotifyPropertyChanged
	{
		private string _notes;
		private string _date;
		private string _ticket_id_string;
		private string _displayDate = String.Empty;
		private int _ticketCount = 0;

		[PrimaryKey]
		public int id { get; set; }

		public string notes
		{
			get
			{
				return this._notes;
			}
			set
			{
				if (value != this._notes)
				{
					this._notes = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string date
		{
			get
			{
				return this._date;
			}
			set
			{
				if (value != this._date)
				{
					this._date = value;
					NotifyPropertyChanged();
				}
			}
		}

		public int person_id { get; set; }

		public string ticket_id_string
		{
			get
			{
				return this._ticket_id_string;
			}
			set
			{
				if (value != this._ticket_id_string)
				{
					this._ticket_id_string = value;
					NotifyPropertyChanged();
				}
			}
		}

		[Ignore]
		public string displayDate
		{
			get
			{
				return this._displayDate;
			}
			set
			{
				if (value != _displayDate)
				{
					this._displayDate = value;
					NotifyPropertyChanged();
				}
			}
		}

		[Ignore]
		public int ticketCount
		{
			get
			{
				return this._ticketCount;
			}
			set
			{
				if (value != _ticketCount)
				{
					this._ticketCount = value;
					NotifyPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Conversation"/> class.
		/// </summary>
		public Conversation()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Conversation"/> class.
		/// </summary>
		/// <param name="tickets">Tickets.</param>
		/// <param name="notes">Notes.</param>
		/// <param name="date">Date.</param>
		public Conversation(string notes, string date)
		{
			this.notes = notes;
			this.date = date;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Conversation"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Conversation"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Conversation: id={0}, notes={1}, date={2}, person_id={3}, ticket_id_string={4}, displayDate={5}, ticketCount={6}]", id, notes, date, person_id, ticket_id_string, displayDate, ticketCount);
		}
	}
}

