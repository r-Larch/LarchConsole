using System;
using System.Linq;
using NUnit.Framework;


namespace LarchConsole.Test {
    [TestFixture]
    public class ConsolWriterTester {
        [Test]
        public void NormalTest() {
            var writer = new ConsoleWriter();

            writer.WriteLine("test-1").WriteLine("test-2");

            AssertLineEndings(writer);
        }

        [Test]
        public void AppendTest() {
            var writer = new ConsoleWriter();
            writer.Append(ConsoleWriter.CreateLine("test-1"));
            writer.Append(ConsoleWriter.CreateLine("test-2.1").WriteLine("test-2.2").WriteLine("test-2.3"));
            writer.Append(ConsoleWriter.Create("test-3 - "));
            writer.Append(ConsoleWriter.CreateLine("test-4.1").Write("test-4.2 - ").WriteLine("test-4.3"));

            writer.Write("Hallo - ");
            writer.Write("Test ");
            writer.WriteLine("test {0}", "hoi");

            AssertLineEndings(writer);
        }

        [Test]
        public void FlushTest() {
            var writer = new ConsoleWriter();

            for (var i = 0; i < 30; i++) {
                writer.Write("line ").WriteLine("number: {0}", i + 1);
            }

            Console.WriteLine("test 1");
            Console.WriteLine("==================================");
            writer.Flush(0, 5);
            writer.Flush(5, 5);
            Console.WriteLine("==================================");

            Console.WriteLine();
            Console.WriteLine("test 2");
            Console.WriteLine("==================================");
            writer.Flush(0, 5);
            Console.WriteLine("----------------------------------");
            writer.Flush(5, 5);
            Console.WriteLine("==================================");
        }

        [Test]
        public void Flush2Test() {
            var writer = new ConsoleWriter();

            for (var i = 0; i < 30; i++) {
                writer.Write("line ").WriteLine("number: {0}", i + 1);
            }

            Console.WriteLine("test 1");
            Console.WriteLine("==================================");
            writer.Flush(0, 0);
            writer.Flush(0, 5);
            writer.Flush(5, 0);
            Console.WriteLine("==================================");

            Console.WriteLine();
            Console.WriteLine("test 2");
            Console.WriteLine("==================================");
            writer.Flush(0, 5);
            Console.WriteLine("----------------------------------");
            writer.Flush(6, 1);
            Console.WriteLine("==================================");
        }

        private void AssertLineEndings(ConsoleWriter writer) {
            foreach (var ending in writer.LineEndings) {
                Console.WriteLine($"first: {ending}");
                Assert.AreEqual(Environment.NewLine, writer.Buffer[ending]);
            }

            for (var i = 0; i < writer.Buffer.Count; i++) {
                if (writer.Buffer[i].ToString() == Environment.NewLine) {
                    Console.WriteLine($"secund: {i}");
                    Assert.IsTrue(writer.LineEndings.Contains(i));
                }
            }

            Console.WriteLine("===============================");
            writer.Flush();
        }
    }
}