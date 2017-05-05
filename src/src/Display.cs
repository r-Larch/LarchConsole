using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LarchConsole {
    public class Display : IDisposable {
        private readonly ConsoleWriter _writer;

        public Display(ConsoleWriter writer) {
            _writer = writer;

            Console.Title = "Viewer";
        }

        private void Save() {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;
            var buffersize = width*height;

            var buffer = new char[buffersize];

            Console.SetCursorPosition(0, 0);
            Console.In.Read(buffer, 0, buffersize);
        }

        public void Dispose() {
        }
    }
}