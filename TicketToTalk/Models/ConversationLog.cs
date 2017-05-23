// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 22/05/2017
//
// ConversationLog.cs

using System;
using System.Collections.Generic;

namespace TicketToTalk
{
	public class ConversationLog
	{

		private DateTime _start;
		private DateTime _finish;
		private int _conversationId;
		private List<TicketLog> _ticketLogs;

		/// <summary>
		/// Gets or sets the ticket logs.
		/// </summary>
		/// <value>The ticket logs.</value>
		public List<TicketLog> TicketLogs
		{
			get
			{
				return _ticketLogs;
			}
			set 
			{
				if (_ticketLogs != value) 
				{
					_ticketLogs = value;
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
				return this._start;
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
					_finish = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the conversation identifier.
		/// </summary>
		/// <value>The conversation identifier.</value>
		public int ConversationId 
		{
			get 
			{
				return _conversationId;
			}
			set 
			{
				if (_conversationId != value) 
				{
					_conversationId = value;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationLog"/> class.
		/// </summary>
		public ConversationLog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationLog"/> class.
		/// </summary>
		/// <param name="ticketLogs">Ticket logs.</param>
		/// <param name="start">Start.</param>
		/// <param name="finish">Finish.</param>
		/// <param name="conversationId">Conversation identifier.</param>
		public ConversationLog(List<TicketLog> ticketLogs, DateTime start, DateTime finish, int conversationId)
		{
			TicketLogs = ticketLogs;
			Start = start;
			Finish = finish;
			ConversationId = conversationId;
		}

		public override string ToString()
		{
			return string.Format("[ConversationLog: TicketLogs={0}, Start={1}, Finish={2}, ConversationId={3}]", TicketLogs.Count, Start, Finish, ConversationId);
		}
	}
}
