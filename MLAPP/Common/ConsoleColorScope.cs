using System;

namespace MLAPP.Common
{
    /// <summary>
  /// Changes the console foreground color and changes it back again.
  /// </summary>
    public class ConsoleColorScope : IDisposable
    {
        private ConsoleColor previousColor;

        /// <summary>
        /// Changes the console foreground color.
        /// </summary>
        /// <param name="color">The new foreground color.</param>
        public ConsoleColorScope(ConsoleColor color)
        {
            this.previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Changes the foreground color to its previous value.
        /// </summary>
        public void Dispose()
        {
            Console.ForegroundColor = previousColor;
        }
    }
}
