namespace MiniServ.Infrastructure;

public static class ConsoleExtensions
{
    extension(Console)
    {
        public static void WriteMarkup(string line)
        {
            var parts = line.Split('[', ']');
            foreach (var part in parts)
            {
                if (part.StartsWith("\\$"))
                {
                    Console.Write(part[1..]);
                    continue;
                }

                if (part is "$")
                {
                    Console.ResetColor();
                    continue;
                }

                if (part.StartsWith("$") && Enum.TryParse<ConsoleColor>(part[1..], ignoreCase: true, out var color))
                {
                    Console.ForegroundColor = color;
                    continue;
                }

                Console.Write(part);
            }

            Console.ResetColor();
        }

        public static void WriteMarkupLine(string line) => Console.WriteMarkup(line + Environment.NewLine);
    }
}
