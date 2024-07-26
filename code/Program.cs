using FolkerKinzel.VCards;
using System.Net.Http.Headers;
using System.Text;

namespace VcfCardConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var lookupdir = args?.FirstOrDefault();

            // Collection of all contacts.
            var contacts = new List<VCard>();

            // Current app executing dir.
            var dir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

            // Search for all VCF cards in dir.
            var files = string.IsNullOrWhiteSpace(lookupdir) ? Directory.GetFiles(dir, "*.vcf") : Directory.GetFiles(lookupdir, "*.vcf");

            // Iterate through and load them up.
            foreach (var file in files)
            {
                var cards = FolkerKinzel.VCards.Vcf.Load(file, Encoding.GetEncoding(1252));
                cards.ToList().ForEach(card  => PrintContact(card));
                contacts.AddRange(cards);
            }

            // Join them up and export.
            var filename = $"{Guid.NewGuid()}.vcf";
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                Vcf.Serialize(contacts, stream);
            }

            Console.WriteLine($"Stored into {filename}.");
        }

        static void PrintContact(VCard contact)
        {
            var name = contact.NameViews?.First()?.Value;

            if(name == null)
            {
                Console.WriteLine($"Couldn't parse contact {contact.ID}.");
                return;
            }

            Console.WriteLine($"{name.ToDisplayName()}");
        }
    }
}
