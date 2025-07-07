
namespace CodingTracker.SheheryarRaza
{
    public class CodingSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }

        public void CalculateDuration()
        {
            if (EndTime < StartTime)
            {
                Console.WriteLine("Warning: End time is before start time. Duration set to 0.");
                Duration = TimeSpan.Zero;
            }
            else
            {
                Duration = EndTime - StartTime;
            }
        }

        public override string ToString()
        {
            return $"ID: {Id}, Start: {StartTime:yyyy-MM-dd HH:mm}, End: {EndTime:yyyy-MM-dd HH:mm}, Duration: {Duration}";
        }
    }
}
