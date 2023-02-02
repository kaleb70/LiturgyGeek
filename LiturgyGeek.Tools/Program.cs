namespace LiturgyGeek.Tools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            args[0] = args[0].ToLower();

            switch (args[0])
            {
                case "uc":
                case "uploadcalendar":
                    new UploadCalendar.UploadCalendar().Run(args.Skip(1).ToArray());
                    break;

                case "uo":
                case "uploadoccasions":
                    new UploadOccasions.UploadOccasions().Run(args.Skip(1).ToArray());
                    break;
            }
        }
    }
}
