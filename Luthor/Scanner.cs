namespace Luthor
{
    public class Scanner
    {
        private readonly string _source;
        private readonly int _maxOffset = 0;
        private int _offset = 0;

        public Scanner(string source)
        {
            _source = source;
            _maxOffset = _source.Length - 1;
            _offset = 0;
        }

        public bool HasMore()
        {
            return _offset <= _maxOffset;
        }

        public bool EndOfSource()
        {
            return !HasMore();
        }

        public char? PeekNext()
        {
            if (EndOfSource())
            {
                return null;
            }

            return _source[_offset];
        }

        public char? GetNext()
        {
            if (EndOfSource())
            {
                return null;
            }

            return _source[_offset++];
        }

        public int GetCurrentPosition()
        {
            return _offset;
        }
    }
}
