using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSnippets
{
    public class Kitten
    {
        public string Name { get; set; }
    }

    public class Human
    {
        public Kitten Kitten { get { return null; } }
    }

    public class CurseOfNull
    {
        public void Demo()
        {
            // this is bad
            Kitten badKitten = null;

            // this is better
            var betterKitten = new Kitten();

            // this is still a problem: 
            // whenever you get a property,
            // you have no idea whether it's safe:
            // any class can be null.
            var human = new Human();
            var maybeKitten = human.Kitten;

            // as a result, you need protection everywhere
            // and have code that looks like that:
            if (maybeKitten == null)
            {
                return;
            }

            Console.WriteLine(maybeKitten.Name);


            // this is just not very nice
            var candidate = "42";
            int number;
            var result = Int32.TryParse(candidate, out number);
        }
    }
}
