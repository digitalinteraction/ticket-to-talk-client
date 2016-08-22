using System;
namespace TicketToTalk
{
	public class Token
	{
		public string val { get; set; }

		public Token()
		{
		}

		public override string ToString()
		{
			return string.Format("[Token: val={0}]", val);
		}
	}

}

