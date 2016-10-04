using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Money
{
    public class Tests
    {
        // This is the test I would like to write.
        // Annoyingly, it doesn't pass, because I
        // need to implement value-wise equality :(
        [Test]
        public void Two_Times_Five_Dollars_Should_Equal_Ten_Dollars()
        {
            // arrange
            var fiveDollars = new Dollars(5);
            // act
            var actual = fiveDollars.MultiplyBy(2);
            var expected = new Dollars(10);
            // assert
            Assert.That(expected, Is.EqualTo(actual));
        }
    }

    public class Dollars
    {
        private readonly double value;

        public Dollars(double value)
        {
            this.value = value;
        }

        public Dollars MultiplyBy(double multiplier)
        {
            return new Dollars(this.value * multiplier);
        }
    }
}