using System;
using System.Runtime.Serialization;

namespace TicketToTalk
{
	[Serializable]
	class APIErrorException : Exception
	{
		public APIErrorException()
		{
		}

		public APIErrorException(string message) : base(message)
		{
		}

		public APIErrorException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected APIErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}