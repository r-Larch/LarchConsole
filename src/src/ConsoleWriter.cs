using System;
using System.Collections.Generic;
using System.Linq;


namespace LarchConsole {
    public class ConsoleWriter {
        private readonly List<IEnumerable<char>> _buffer;
        private readonly List<int> _lineEndings;

        private AvailableColors _colors;
        private AvailableColors Colors => _colors ?? (_colors = new AvailableColors());

        public int LinesCount => _lineEndings.Count;
        public IReadOnlyList<IEnumerable<char>> Buffer => _buffer;
        public IReadOnlyList<int> LineEndings => _lineEndings;

        public ConsoleWriter() {
            _lineEndings = new List<int>() {};
            _buffer = new List<IEnumerable<char>>();
        }

        public ConsoleWriter Write(string str, params object[] args) {
            Add(string.Format(str, args));
            return this;
        }

        public ConsoleWriter WriteLine(string str, params object[] args) {
            Add(string.Format(str, args), newLine: true);
            return this;
        }

        public ConsoleWriter WriteLine() {
            Add(null, newLine: true);
            return this;
        }

        public ConsoleWriter Write(IMatch match, int formatLeng) {
            var leng = formatLeng - match.Value.Length;
            Write(match);
            if (leng > 0) {
                Add(Enumerable.Repeat(' ', leng));
            }
            return this;
        }

        public ConsoleWriter WriteLine(IMatch match, int formatLeng) {
            var leng = formatLeng - match.Value.Length;
            Write(match);
            if (leng > 0) {
                Add(Enumerable.Repeat(' ', leng), newLine: true);
            }
            return this;
        }

        public ConsoleWriter Write(IMatch match) {
            Add(RenderFilter(match));
            return this;
        }

        public ConsoleWriter WriteLine(IMatch match) {
            Add(RenderFilter(match), newLine: true);
            return this;
        }

        public ConsoleWriter Format(string format, Func<Parms, Parms> parms) {
            var dic = parms(new Parms());
            Add(GetFormated(format, dic.Parameters));
            return this;
        }

        public ConsoleWriter FormatLine(string format, Func<Parms, Parms> parms) {
            var dic = parms(new Parms());
            Add(GetFormated(format, dic.Parameters), newLine: true);
            return this;
        }

        public ConsoleWriter FormatLines<T>(string format, List<Match<T>> datas, Func<Match<T>, Parms, Parms> parms) {
            foreach (var match in datas) {
                var dic = parms(match, new Parms());
                Add(GetFormated(format, dic.Parameters), newLine: true);
            }

            return this;
        }

        public static ConsoleWriter Create(string str) {
            return new ConsoleWriter().Write(str);
        }

        public static ConsoleWriter CreateLine(string line) {
            return new ConsoleWriter().WriteLine(line);
        }

        public void Flush() {
            foreach (var sign in _buffer.SelectMany(x => x)) {
                Console.Write(sign);
            }
        }

        public void Flush(int startLineIndex, int count) {
            if (startLineIndex == 0 && count == 0) {
                return;
            }

            if (startLineIndex < 0 || startLineIndex >= _lineEndings.Count) {
                throw new IndexOutOfRangeException(nameof(startLineIndex));
            }

            startLineIndex -= 1;
            var endIndex = startLineIndex + count;

            if (endIndex < 0 || endIndex >= _lineEndings.Count) {
                throw new IndexOutOfRangeException(nameof(count));
            }

            var start = 0;
            if (startLineIndex != -1) {
                start = _lineEndings[startLineIndex] + 1;
            }

            var end = _lineEndings[endIndex] + 1;

            foreach (var sign in _buffer.GetRange(start, end - start).SelectMany(x => x)) {
                Console.Write(sign);
            }
        }

        public void Append(ConsoleWriter writer) {
            if (writer == null) {
                return;
            }

            var count = _buffer.Count;
            _buffer.AddRange(writer.Buffer);
            foreach (var lineEnding in writer.LineEndings) {
                _lineEndings.Add(lineEnding + count);
            }
        }

        private void Add(IEnumerable<char> chars) {
            _buffer.Add(chars);
        }

        private void Add(IEnumerable<char> chars, bool newLine) {
            if (chars != null) {
                _buffer.Add(chars);
            }
            if (newLine) {
                _buffer.Add(Environment.NewLine);
                _lineEndings.Add(_buffer.Count - 1);
            }
        }

        private IEnumerable<char> RenderFilter(IMatch match) {
            var chars = match.Value.ToCharArray();
            var c = Console.ForegroundColor;
            for (var i = 0; i < chars.Length; i++) {
                var part = match.Matches.LastOrDefault(x => x.IsMatch && i >= x.Start && i < x.Stop);
                Console.ForegroundColor = part != null ? Colors.GetColor(part.Num) : c;
                yield return chars[i];
            }

            Console.ForegroundColor = c;
        }

        private IEnumerable<char> GetFormated(string format, Dictionary<string, object> parms) {
            var key = "";
            var formStr = "";
            var fullformStr = "";
            var isParm = false;
            var isFormat = false;
            foreach (var sign in format.ToCharArray()) {
                switch (sign) {
                    case '{':
                        isParm = true;
                        fullformStr = "";
                        continue;
                    case '}':
                        isParm = false;
                        isFormat = false;

                        int minCount, count = 0;
                        int.TryParse(formStr, out minCount);
                        formStr = "";

                        object value;
                        if (!parms.TryGetValue(key, out value)) {
                            var str = $"{{{key}}}";
                            if (minCount > 0) {
                                var spaces = minCount - str.Length;
                                for (var i = 0; i < spaces; i++) yield return ' ';
                            }

                            foreach (var s in str) {
                                count++;
                                yield return s;
                            }

                            if (minCount < 0) for (var i = 0; i < Math.Abs(minCount) - count; i++) yield return ' ';

                            key = "";
                            continue;
                        }

                        key = "";

                        var match = value as IMatch;
                        if (match != null) {
                            if (minCount > 0) {
                                var spaces = minCount - match.Value.Length;
                                for (var i = 0; i < spaces; i++) yield return ' ';
                            }

                            foreach (var s in RenderFilter(match)) {
                                count++;
                                yield return s;
                            }

                            if (minCount < 0) for (var i = 0; i < Math.Abs(minCount) - count; i++) yield return ' ';

                            continue;
                        }

                        foreach (var s in string.Format($"{{0{fullformStr}}}", value)) yield return s;

                        continue;
                    case ',':
                        if (isParm) {
                            isFormat = true;
                            fullformStr += sign;
                            break;
                        }

                        yield return sign;

                        break;
                    case ':':
                        if (isParm) {
                            isFormat = true;
                            fullformStr += sign;
                            break;
                        }

                        yield return sign;

                        break;
                    default: {
                        if (isParm) {
                            if (isFormat) {
                                formStr += sign;
                                fullformStr += sign;
                            } else {
                                key += sign;
                            }
                        } else {
                            yield return sign;
                        }

                        break;
                    }
                }
            }
        }
    }


    public class Parms {
        public Dictionary<string, object> Parameters { get; set; }

        public Parms() {
            Parameters = new Dictionary<string, object>();
        }

        public Parms Add(string name, object value) {
            Parameters.Add(name, value);
            return this;
        }

        public Parms Add(string name, IMatch value, bool highlight = false) {
            Parameters.Add(name, highlight ? value : (object) value.Value);
            return this;
        }
    }
}