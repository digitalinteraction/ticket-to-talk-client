using System;
using System.Collections.Generic;

namespace TicketToTalk
{
	/// <summary>
	/// Period controller.
	/// </summary>
	public class PeriodController
	{

		//private PeriodDB periodDB = new PeriodDB();
		//private PersonPeriodDB personPeriodDB = new PersonPeriodDB();
		//private TicketDB ticketDB = new TicketDB();
		private TicketController ticketController = new TicketController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PeriodController"/> class.
		/// </summary>
		public PeriodController()
		{
		}

		/// <summary>
		/// Gets the period.
		/// </summary>
		/// <returns>The period.</returns>
		/// <param name="id">Identifier.</param>
		public Period GetPeriod(int id) 
		{
			Period period;

			lock (Session.Connection)
			{
				period = (from p in Session.Connection.Table<Period>() where p.id == id select p).FirstOrDefault();
			}

			return period;
		}

		/// <summary>
		/// Gets all periods.
		/// </summary>
		/// <returns>The all periods.</returns>
		public List<Period> GetAllLocalPeriods() 
		{

			List<PersonPeriod> relations = new List<PersonPeriod>();

			lock (Session.Connection)
			{
				var q = from p in Session.Connection.Table<PersonPeriod>() where p.person_id == Session.activePerson.id select p;

				foreach (PersonPeriod p in q) 
				{
					relations.Add(p);
				}
			}

			var periods = new List<Period>();

			lock (Session.Connection)
			{
				foreach (PersonPeriod pp in relations)
				{
					var period = (from p in Session.Connection.Table<Period>() where p.id == pp.period_id select p).FirstOrDefault();
					periods.Add(period);
				}
			}

			return periods;
		}

		/// <summary>
		/// Adds the period.
		/// </summary>
		/// <returns>The period.</returns>
		/// <param name="p">P.</param>
		public void AddLocalPeriod(Period p) 
		{
			lock (Session.Connection)
			{
				Session.Connection.Insert(p);
			}
		}

		/// <summary>
		/// Inits the stock periods.
		/// </summary>
		/// <returns>The stock periods.</returns>
		public void InitStockPeriods() 
		{
			string[] texts = 
			{
				"Childhood",
				"Teenager",
				"Adult",
				"Retirement"
			};

			for (int i = 1; i < 5; i++) 
			{
				if (GetPeriod(i) == null) 
				{
					var pp = new Period(i, texts[i - 1]);
					AddLocalPeriod(pp);
				}
			}
		}

		/// <summary>
		/// Gets the tickets in period.
		/// </summary>
		/// <returns>The tickets in period.</returns>
		/// <param name="period_id">Period identifier.</param>
		public List<Ticket> GetTicketsInPeriod(int period_id) 
		{
			//ticketDB.Open();
			//var all_tickets = ticketDB.GetTicketsByPeriodID(period_id);

			List<Ticket> all_tickets = new List<Ticket>();

			lock (Session.Connection)
			{
				var q = from t in Session.Connection.Table<Ticket>() where t.period_id == period_id select t;

				foreach (Ticket t in q) 
				{
					all_tickets.Add(t);
				}
			}

			var tickets = new List<Ticket>();

			if (all_tickets != null) 
			{
				foreach (Ticket t in ticketController.FilterTicketsForUserType(all_tickets))
				{
					if (t.person_id == Session.activePerson.id)
					{
						tickets.Add(t);
					}
				}
			}

			return tickets;
		}

		/// <summary>
		/// Gets the period ticket count.
		/// </summary>
		/// <returns>The period ticket count.</returns>
		/// <param name="period_id">Period identifier.</param>
		public int GetPeriodTicketCount(int period_id) 
		{
			List<Ticket> tickets = ticketController.GetTicketsByPeriodID(period_id);

			return ticketController.FilterTicketsForUserType(tickets).Count;
		}
	}
}

