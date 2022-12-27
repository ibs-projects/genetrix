using System.Drawing;

namespace genetrix.Models
{
    public class TypeNotification
    {
        public int Id { get; set; }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private short type;

        public short Type
        {
            get { return type; }
            set { type = value; }
        }

        private Color color;

        public Color Couleur
        {
            get { return color; }
            set { color = value; }
        }
    }

}