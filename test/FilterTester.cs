using System;
using NUnit.Framework;


namespace LarchConsole.Test {
    [TestFixture]
    public class FilterTester {
        [Test]
        public void FilterTest() {
            const string value = "test.www";

            Console.WriteLine("## Normal");
            var filter = new Filter("test.www", CampareType.WildCard, CompareMode.CaseIgnore);
            Test(filter, value);


            Console.WriteLine("## WildCard");
            filter = new Filter("test.???", CampareType.WildCard, CompareMode.CaseIgnore);
            Test(filter, value);


            Console.WriteLine("## Regex");
            filter = new Filter("test.[w]{3}", CampareType.Regex, CompareMode.CaseIgnore);
            Test(filter, value);
        }


        private void Test(Filter filter, string value) {
            var match = filter.GetMatch(value, s => s);
            var table = new Table();
            table.Create(1, 1, "matches", match.Matches);
            Console.WriteLine($"{value}|=> Count: {value.Length}");
            new ConsoleWriter().Write(match).Flush();
            Console.WriteLine();
        }
    }
}