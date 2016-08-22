namespace TicketToTalk
{
	/// <summary>
	/// Area controller.
	/// </summary>
	public class AreaController
	{
		
		AreaDB areaDB = new AreaDB();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AreaController"/> class.
		/// </summary>
		public AreaController()
		{
		}

		/// <summary>
		/// Gets the area.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="id">Identifier.</param>
		public Area getArea(int id) 
		{
			areaDB.open();
			var area = areaDB.GetArea(id);
			areaDB.close();
			return area;
		}

		/// <summary>
		/// Adds the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="area">Area.</param>
		public void addAreaLocally(Area area) 
		{
			areaDB.open();
			areaDB.AddArea(area);
			areaDB.close();
		}

		/// <summary>
		/// Deletes the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="id">Identifier.</param>
		public void deleteAreaLocally(int id) 
		{
			areaDB.open();
			areaDB.DeleteArea(id);
			areaDB.close();
		}

		/// <summary>
		/// Updates the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="area">Area.</param>
		public void updateAreaLocally(Area area) 
		{
			deleteAreaLocally(area.id);
			addAreaLocally(area);
		}
	}
}

