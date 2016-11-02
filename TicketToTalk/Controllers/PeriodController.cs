using System;
using System.Collections.Generic;

namespace TicketToTalk
{
	/// <summary>
	/// Period controller.
	/// </summary>
	public class PeriodController
	{

		private PeriodDB periodDB = new PeriodDB();
		private PersonPeriodDB personPeriodDB = new PersonPeriodDB();
		private TicketDB ticketDB = new TicketDB();
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
			periodDB.Open();
			var period = periodDB.GetPeriod(id);
			periodDB.Close();

			return period;
		}

		/// <summary>
		/// Gets all periods.
		/// </summary>
		/// <returns>The all periods.</returns>
		public List<Period> GetAllLocalPeriods() 
		{
			personPeriodDB.Open();
			var relations = personPeriodDB.GetRelationByPersonID(Session.activePerson.id);

			var periods = new List<Period>();
			foreach (PersonPeriod pp in relations) 
			{
				periods.Add(GetPeriod(pp.period_id));
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
			periodDB.Open();
			periodDB.AddPeriod(p);
			periodDB.Close();
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
			ticketDB.Open();
			var all_tickets = ticketDB.GetTicketsByPeriodID(period_id);
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

			ticketDB.Close();

			return tickets;
		}

		/// <summary>
		/// Gets the period ticket count.
		/// </summary>
		/// <returns>The period ticket count.</returns>
		/// <param name="period_id">Period identifier.</param>
		public int GetPeriodTicketCount(int period_id) 
		{
			ticketDB.Open();
			var tickets = ticketDB.GetTicketsByPeriodID(period_id);
			ticketDB.Close();

			return ticketController.FilterTicketsForUserType(tickets).Count;
		}
	}
}

