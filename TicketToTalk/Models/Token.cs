using System;
namespace TicketToTalk
{

	/// <summary>
	/// Token.
	/// </summary>
	public class Token
	{
		public string val { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Token"/> class.
		/// </summary>
		public Token()
		{
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Token"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Token"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Token: val={0}]", val);
		}
	}

}

