using System;
using System.Runtime.Serialization;

namespace TicketToTalk
{
	[Serializable]
	class APIUnauthorisedForResourceException : Exception
	{
		public APIUnauthorisedForResourceException()
		{
		}

		public APIUnauthorisedForResourceException(string message) : base(message)
		{
		}

		public APIUnauthorisedForResourceException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected APIUnauthorisedForResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}