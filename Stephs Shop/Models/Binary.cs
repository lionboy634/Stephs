namespace Stephs_Shop.Models
{
    public class Binary
    {
        public byte[] filebytes { get; set; }
        public string filename { get; set; }

        public Binary(byte[] filebytes, string filename)
        {
            this.filebytes = filebytes;
            this.filename = filename;
        }
    }
}
