using System;
using System.Diagnostics;

namespace MyBrain
{
	/// <summary>
	/// Stop watch to track time elapsed.
	/// </summary>
	public static class StopWatch
	{
		/// <summary>
		/// Start time in ticks.
		/// </summary>
		private static long StartTick { get; set; }

		/// <summary>
		/// Stop time or elapsed time so far in ticks.
		/// </summary>
		private static long StopTick { get; set; }

		/// <summary>
		/// Starts stop watch.
		/// </summary>
		public static void Start()
		{
			StartTick = DateTime.Now.Ticks;
		}

		/// <summary>
		/// Sets stop time, and by default outputs the total elapsed time in milliseconds.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="writeConsole"></param>
		/// <param name="writeDebuggerOutput"></param>
		public static void Elapsed( string label = "", bool writeConsole = true, bool writeDebuggerOutput = true )
		{
			StopTick = DateTime.Now.Ticks;
			if ( writeConsole )
				Console.WriteLine( string.IsNullOrWhiteSpace( label )
					? $"Elapsed: {Convert.ToInt32( new TimeSpan( StopTick - StartTick ).TotalMilliseconds )} ms"
					: $"Elapsed: {Convert.ToInt32( new TimeSpan( StopTick - StartTick ).TotalMilliseconds )} ms - {label}" );

			if ( writeDebuggerOutput && Debugger.IsAttached )
			{
				Debug.WriteLine( string.IsNullOrWhiteSpace( label )
					? $"Elapsed: {Convert.ToInt32( new TimeSpan( StopTick - StartTick ).TotalMilliseconds )} ms"
					: $"Elapsed: {Convert.ToInt32( new TimeSpan( StopTick - StartTick ).TotalMilliseconds )} ms - {label}" );
			}
		}

		/// <summary>
		/// Sets the stop time.
		/// </summary>
		public static void Stop()
		{
			StopTick = DateTime.Now.Ticks;
		}
	}
}