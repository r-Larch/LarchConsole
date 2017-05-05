using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LarchConsole {
    public class Table {
        private readonly StringBuilder sb = new StringBuilder();

        public void Create<T>(int left, int top, string title, IEnumerable<T> data) {
            var d = data as T[] ?? data.ToArray();
            if (d.Length == 0) {
                Console.WriteLine($"{title}: List is empty");
                return;
            }

            var type = typeof(T);
            var props = type.GetProperties();
            var fields = type.GetFields();

            var cols = new List<Tuple<string, int>>(); // = list von (spalten name, spalten breite)   [col]
            var table = new List<List<string>>(); // = list von spalten   [col][row]
            foreach (var field in fields) {
                var column = d.Select(rowdata => field.GetValue(rowdata)?.ToString() ?? string.Empty).ToList();
                table.Add(column);

                cols.Add(new Tuple<string, int>(field.Name, Math.Max(column.Max(x => x.Length), field.Name.Length) + 1));
            }
            foreach (var prop in props) {
                var column = d.Select(rowdata => prop.GetValue(rowdata)?.ToString() ?? string.Empty).ToList();
                table.Add(column);

                cols.Add(new Tuple<string, int>(prop.Name, Math.Max(column.Max(x => x.Length), prop.Name.Length) + 1));
            }

            //var width = cols.Sum(x => x.Item2 + 1) + 1;

            Repeat("\r\n", top);
            Repeat(' ', left + 1);
            sb.AppendLine(title);

            Repeat(' ', left);
            TopBorder(cols);
            Repeat(' ', left);
            Header(cols);
            Repeat(' ', left);
            Border(cols);
            Repeat(' ', left);
            Rows(cols, table, left);
            BottomBorder(cols);

            Console.WriteLine(sb);
        }

        private void Header(List<Tuple<string, int>> cols) {
            foreach (var col in cols) {
                sb.Append("│");
                sb.Append(col.Item1);
                Repeat(' ', col.Item2 - col.Item1.Length);
            }

            sb.AppendLine("│");
        }

        private void Border(List<Tuple<string, int>> cols) {
            var isFirst = true;
            foreach (var col in cols) {
                sb.Append(isFirst ? "├" : "┼");
                isFirst = false;
                Repeat('─', col.Item2);
            }

            sb.AppendLine("┤");
        }

        private void Rows(List<Tuple<string, int>> cols, List<List<string>> table, int left) {
            for (var rowIndex = 0; rowIndex < table.First().Count; rowIndex++) {
                for (var col = 0; col < cols.Count; col++) {
                    sb.Append("│");
                    var value = table[col][rowIndex];
                    sb.Append(value);
                    Repeat(' ', cols[col].Item2 - value.Length);
                }

                sb.AppendLine("│");
                Repeat(' ', left);
            }
        }

        private void Repeat(char sign, int num) {
            for (var i = 0; i < num; i++) {
                sb.Append(sign);
            }
        }

        private void Repeat(string sign, int num) {
            for (var i = 0; i < num; i++) {
                sb.Append(sign);
            }
        }

        private void TopBorder(List<Tuple<string, int>> cols) {
            var isFirst = true;
            foreach (var col in cols) {
                sb.Append(isFirst ? "┌" : "┬");
                isFirst = false;
                Repeat('─', col.Item2);
            }

            sb.AppendLine("┐");
        }

        private void BottomBorder(List<Tuple<string, int>> cols) {
            var isFirst = true;
            foreach (var col in cols) {
                sb.Append(isFirst ? "└" : "┴");
                isFirst = false;
                Repeat('─', col.Item2);
            }

            sb.AppendLine("┘");
        }
    }
}