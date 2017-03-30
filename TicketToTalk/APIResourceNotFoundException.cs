using System;
using System.Runtime.Serialization;

namespace TicketToTalk
{
	[Serializable]
	class APIResourceNotFoundException : Exception
	{
		public APIResourceNotFoundException()
		{
		}

		public APIResourceNotFoundException(string message) : base(message)
		{
		}

		public APIResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected APIResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}