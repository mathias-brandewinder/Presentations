namespace CSharpSnippets
{
    public class UnitsOfMeasure
    {
        /// <summary>
        /// Compute length in miles
        /// </summary>
        public double ComputeMiles()
        {
            return 42.0;
        }

        /// <summary>
        /// Compute temperature in Celcius
        /// </summary>
        public double ComputeTemperature()
        {
            return 42.0;
        }

        public void Demo()
        {
            var miles = ComputeMiles();
            var temperature = ComputeTemperature();

            // now this is a problem...
            var oops = miles + temperature;

            // I can maybe prevent addition 
            // with a Miles and Temperature types

            // Now this is getting bad
            var uh = temperature / miles;
            var oh = miles * miles;
        }
    }
}
