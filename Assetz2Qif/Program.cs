namespace Assetz2Qif
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Program
    {
        public static void Main(string[] args)
        {
            // can't continue if there's no path passed as an argument!
            if (args.Length != 1)
            {
                Console.WriteLine("No path provided!");
                Console.ReadKey(true);

                return;
            }

            new Program(args[0]);
        }

        public Program(string path)
        {
            var builder = new StringBuilder();

            builder.AppendLine("!Type:Bank");

            var stream = File.OpenRead(path);
            var reader = new StreamReader(stream);

            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var row = reader.ReadLine().Split(',');

                var date = DateTime.Parse(row[1].Replace("\"", "")).ToShortDateString();
                var type = row[3].Replace("\"", "").Split(' ').First();
                var value = Convert.ToDecimal(row[4]);

                var note = string.Empty;

                var valid = false;

                switch (type)
                {
                    case "Interest":
                        note = "Interest";
                        valid = true;
                        break;
                    case "Deposit:":
                        note = "Deposit";
                        valid = true;
                        break;
                    case "Withdrawal":
                        note = "Withdrawal";
                        valid = true;
                        break;
                }

                if (valid)
                {
                    builder.AppendFormat("D{0}\r\n", date);
                    builder.AppendFormat("T{0}\r\n", value);
                    builder.AppendFormat("P{0}\r\n", note);
                    builder.AppendLine("^");
                }
            }

            reader.Dispose();
            stream.Dispose();
            reader = null;
            stream = null;

            var result = builder.ToString();

            File.WriteAllText("out.qif", result);
        }
    }
}
