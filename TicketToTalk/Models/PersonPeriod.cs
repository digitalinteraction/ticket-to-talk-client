using System;
using SQLite;

namespace TicketToTalk
{
	/// <summary>
	/// Person period.
	/// </summary>
	public class PersonPeriod
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public int person_id { get; set; }
		public int period_id { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PersonPeriod"/> class.
		/// </summary>
		public PersonPeriod()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PersonPeriod"/> class.
		/// </summary>
		/// <param name="person_id">Person identifier.</param>
		/// <param name="period_id">Period identifier.</param>
		public PersonPeriod(int person_id, int period_id)
		{
			this.person_id = person_id;
			this.period_id = period_id;
		}

		/// <summary>
		/// Tos the string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[PersonPeriod: id={0}, person_id={1}, period_id={2}]", id, person_id, period_id);
		}

		/// <summary>
		/// Equals the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

