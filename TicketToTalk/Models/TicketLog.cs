// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 22/05/2017
//
// TicketLog.cs
using System;
namespace TicketToTalk
{
	public class TicketLog
	{

		private int _ticketId;
		private DateTime _start;
		private DateTime _finish;

		/// <summary>
		/// Gets or sets the ticket identifier.
		/// </summary>
		/// <value>The ticket identifier.</value>
		public int TicketId
		{
			get
			{
				return _ticketId;
			}
			set
			{
				if (_ticketId != value)
				{
					_ticketId = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the start.
		/// </summary>
		/// <value>The start.</value>
		public DateTime Start
		{
			get
			{
				return _start;
			}
			set
			{
				if (_start != value)
				{
					_start = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the finish.
		/// </summary>
		/// <value>The finish.</value>
		public DateTime Finish
		{
			get
			{
				return _finish;
			}
			set
			{
				if (_finish != value)
				{
					_finish= value;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketLog"/> class.
		/// </summary>
		public TicketLog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketLog"/> class.
		/// </summary>
		/// <param name="ticketId">Ticket identifier.</param>
		/// <param name="start">Start.</param>
		/// <param name="finish">Finish.</param>
		public TicketLog(int ticketId, DateTime start, DateTime finish)
		{
			TicketId = ticketId;
			Start = start;
			Finish = finish;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.TicketLog"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.TicketLog"/>.</returns>
		public override string ToString()
		{
			return string.Format("[TicketLog: TicketId={0}, Start={1}, Finish={2}]", TicketId, Start, Finish);
		}
	}
}
