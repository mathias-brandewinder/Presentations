using NUnit.Framework;

namespace CSharpSnippets
{
    public enum Boolean { True, False }

    [TestFixture]
    public class Demo
    {
        [Test]
        public void Ooops()
        {
            int number = -42;
            Boolean check = (Boolean)number;
        }
    }

    public class DiscriminatedUnion
    {
        public string Demo(Boolean boolean)
        {
            switch (boolean)
            {
                case Boolean.True:
                    return "true";
                case Boolean.False:
                    return "false";
                //// wait what? What is this mysterious default?
                //default:
                //    return "wat?";
            }

            // what else could it possibly be?
            return "wat?";
        }

        // this is problematic: what happens
        // if you decide to add "Maybe" to the enum?
    }
}
