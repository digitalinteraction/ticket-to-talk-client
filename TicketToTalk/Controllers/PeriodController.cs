using System;
using System.Collections.Generic;

namespace TicketToTalk
{
	/// <summary>
	/// Period controller.
	/// </summary>
	public class PeriodController
	{

		PeriodDB periodDB = new PeriodDB();
		PersonPeriodDB personPeriodDB = new PersonPeriodDB();
		TicketDB ticketDB = new TicketDB();
		TicketController ticketController = new TicketController();

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
		public Period getPeriod(int id) 
		{
			periodDB.open();
			var period = periodDB.GetPeriod(id);
			periodDB.close();

			return period;
		}

		/// <summary>
		/// Gets all periods.
		/// </summary>
		/// <returns>The all periods.</returns>
		public List<Period> getAllLocalPeriods() 
		{
			personPeriodDB.open();
			var relations = personPeriodDB.getRelationByPersonID(Session.activePerson.id);

			var periods = new List<Period>();
			foreach (PersonPeriod pp in relations) 
			{
				periods.Add(getPeriod(pp.period_id));
			}

			return periods;
		}

		/// <summary>
		/// Adds the period.
		/// </summary>
		/// <returns>The period.</returns>
		/// <param name="p">P.</param>
		public void addLocalPeriod(Period p) 
		{
			periodDB.open();
			periodDB.AddPeriod(p);
			periodDB.close();
		}

		/// <summary>
		/// Inits the stock periods.
		/// </summary>
		/// <returns>The stock periods.</returns>
		public void initStockPeriods() 
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
				if (getPeriod(i) == null) 
				{
					var pp = new Period(i, texts[i - 1]);
					addLocalPeriod(pp);
				}
			}
		}

		/// <summary>
		/// Gets the tickets in period.
		/// </summary>
		/// <returns>The tickets in period.</returns>
		/// <param name="period_id">Period identifier.</param>
		public List<Ticket> getTicketsInPeriod(int period_id) 
		{
			ticketDB.open();
			var all_tickets = ticketDB.getTicketsByPeriodID(period_id);
			var tickets = new List<Ticket>();

			if (all_tickets != null) 
			{
				foreach (Ticket t in ticketController.filterTicketsForUserType(all_tickets))
				{
					if (t.person_id == Session.activePerson.id)
					{
						tickets.Add(t);
					}
				}
			}

			ticketDB.close();

			return tickets;
		}

		/// <summary>
		/// Gets the period ticket count.
		/// </summary>
		/// <returns>The period ticket count.</returns>
		/// <param name="period_id">Period identifier.</param>
		public int getPeriodTicketCount(int period_id) 
		{
			ticketDB.open();
			var tickets = ticketDB.getTicketsByPeriodID(period_id);
			ticketDB.close();

			return ticketController.filterTicketsForUserType(tickets).Count;
		}
	}
}

