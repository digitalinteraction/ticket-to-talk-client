using System;
namespace TicketToTalk
{
	/// <summary>
	/// Inspiration.
	/// </summary>
	public class Inspiration
	{
		public int id { get; set; }
		public string question { get; set; }
		public string prompt { get; set; }
		public string mediaType { get; set;}
		public bool used { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Inspiration"/> class.
		/// </summary>
		public Inspiration()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Inspiration"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="question">Question.</param>
		/// <param name="prompt">Prompt.</param>
		/// <param name="mediaType">Media type.</param>
		/// <param name="used">Used.</param>
		public Inspiration(int id, string question, string prompt, string mediaType, bool used)
		{
			this.id = id;
			this.question = question;
			this.prompt = prompt;
			this.mediaType = mediaType;
			this.used = used;
		}

		public override string ToString()
		{
			return string.Format("[Inspiration: id={0}, question={1}, prompt={2}, mediaType={3}, created_at={4}, updated_at={5}]", id, question, prompt, mediaType, created_at, updated_at);
		}


		/// <summary>
		/// Equals the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			Inspiration i = (Inspiration)obj;
			return (id == i.id) 
				&& (question.Equals(i.question)) 
				&& (prompt.Equals(i.prompt)) 
				&& (mediaType.Equals(i.mediaType))
				&& (used == i.used);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + question.GetHashCode();
			hash = (hash * 7) + prompt.GetHashCode();
			hash = (hash * 7) + mediaType.GetHashCode();

			return hash;
		}
	}
}

