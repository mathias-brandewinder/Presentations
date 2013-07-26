using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpSnippets
{
    [TestFixture]
    public class Functions
    {
        public static IEnumerable<Kitten> GiveMeKittens(IEnumerable<Kitten> source, Func<Kitten, bool> filter)
        {
            return source.Where(filter).ToList();
        }

        [Test]
        public void Demo()
        {
            var kittens = new List<Kitten>()
                {
                    new Kitten() {Name = "Friday"},
                    new Kitten() {Name = "Meow"}
                };

            Func<Kitten, bool> filter = kitten => kitten.Name.StartsWith("F");
            var check = GiveMeKittens(kittens, filter);

            // looks eerilie similar, no?
            Predicate<Kitten> filter2 = kitten => kitten.Name.StartsWith("F");
            // wait how come?
            //var oops = GiveMeKittens(kittens, filter2);
        }
    }
}
