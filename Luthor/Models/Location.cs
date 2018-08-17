namespace Luthor.Models
{
    public class Location
    {
        public int Offset;
        public int Line;
        public int Column;

        public override string ToString()
        {
            return $"line {Line}, column {Column} (offset {Offset})";
        }
    }
}
