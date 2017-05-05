using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace LarchConsole {
    public class Watch : IDisposable {
        private readonly string _name;
        private readonly Stopwatch _watch;

        private static readonly List<Task> Tasks = new List<Task>();

        public Watch(string name) {
            _name = name;
            _watch = Stopwatch.StartNew();
        }

        public void Dispose() {
            _watch.Stop();
            var msec = _watch.ElapsedMilliseconds;
            var sec = (int) (msec/1000);
            msec = msec - sec*1000;
            var min = (int) (sec/60);
            sec = sec - min*60;

            var sb = new StringBuilder();
            if (min > 0) {
                sb.Append($"{min}min ");
            }
            if (sec > 0) {
                sb.Append($"{sec}s ");
            }
            if (min == 0) {
                sb.Append($"{msec}ms");
            }

            Tasks.Add(new Task() {
                Name = _name,
                Time = sb.ToString()
            });
        }


        internal class Task {
            public string Name;
            public string Time;
        }


        public static void PrintTasks() {
            var table = new Table();
            table.Create(1, 1, "Performance", Tasks);
        }
    }
}