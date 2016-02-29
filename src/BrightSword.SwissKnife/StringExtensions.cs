using System.Collections.Generic;

namespace BrightSword.SwissKnife
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitCamelCase(this string _this)
        {
            return _this.SplitIntoSegments(true, true, false);
        }

        public static IEnumerable<string> SplitCamelCaseAndUnderscore(this string _this)
        {
            return _this.SplitIntoSegments(true, true, true, '_');
        }

        public static IEnumerable<string> SplitDotted(this string _this)
        {
            return _this.SplitIntoSegments(false, false, true, '.');
        }

        public static IEnumerable<string> SplitIntoSegments(
            this string _this,
            bool splitBySpace = true,
            bool splitOnCamelCase = true,
            bool splitOnPunctuation = true,
            params char[] separators)
        {
            if (string.IsNullOrEmpty(_this)) { yield break; }

            var iStart = 0;
            var iEnd = 1;

            while (true)
            {
                bool endOfString;

                var segment = _this.GetNextSegment(
                    ref iStart,
                    ref iEnd,
                    out endOfString,
                    splitBySpace,
                    splitOnCamelCase,
                    splitOnPunctuation,
                    separators);

                if (!(string.IsNullOrWhiteSpace(segment))) { yield return segment; }

                if (endOfString) { break; }

                iStart = iEnd++;
            }
        }

        private static string GetNextSegment(
            this string _this,
            ref int iStart,
            ref int iEnd,
            out bool endOfString,
            bool respectSpace = true,
            bool respectCamelCase = true,
            bool respectPunctuation = true,
            params char[] separators)
        {
            endOfString = iEnd == _this.Length;

            // skip over whitespace
            if (respectSpace)
            {
                while (char.IsWhiteSpace(_this[iStart]))
                {
                    iEnd = ++iStart;

                    if (iStart == _this.Length)
                    {
                        endOfString = true;
                        break;
                    }
                }
            }

            // skip over single punctuation mark ({_|.} etc)
            do
            {
                if (!respectPunctuation) { continue; }

                if (!_this[iStart].IsRecognizedPunctuationMark(separators)) { continue; }

                iStart++;

                if (iStart == _this.Length)
                {
                    endOfString = true;
                    break;
                }

                iEnd = iStart;
            } while (false);

            // munch over next word
            while (!endOfString)
            {
                if (respectSpace) { if (char.IsWhiteSpace(_this[iEnd])) { break; } }

                if (respectPunctuation) { if (_this[iEnd].IsRecognizedPunctuationMark(separators)) { break; } }

                if (respectCamelCase)
                {
                    if (char.IsUpper(_this[iEnd]))
                    {
                        if (char.IsUpper(_this[iEnd - 1]))
                        {
                            var iNext = iEnd + 1;

                            if ((iNext < _this.Length))
                            {
                                if (!char.IsUpper(_this[iNext])
                                    && !char.IsWhiteSpace(_this[iNext])
                                    && !_this[iNext].IsRecognizedPunctuationMark(separators)) {
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                iEnd++;

                endOfString = iEnd == _this.Length;
            }

            return _this.Substring(iStart, iEnd - iStart);
        }

        private static bool IsRecognizedPunctuationMark(this char _this, ICollection<char> separators)
        {
            return separators.Count == 0
                       ? char.IsPunctuation(_this)
                       : separators.Contains(_this);
        }
    }
}