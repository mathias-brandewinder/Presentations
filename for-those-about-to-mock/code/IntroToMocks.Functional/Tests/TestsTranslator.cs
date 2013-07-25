namespace IntroToMocks.Functional.Tests
{
   using System;
    using NUnit.Framework;

   [TestFixture]
   public class TestsTranslator
   {
      [Test]
      public void Translation_Should_Return_Result_From_Client()
      {
         var log = new Action<string>(input => {});

         var original = "Hello";
         var expected = "Bonjour";

         var translate = new Func<string, string>(
            (input) =>
               {
                  if (input == original)
                  {
                     return expected;
                  }

                  throw new ArgumentException();
               }
            );

         var translator = new Translator(translate, log);

         Assert.That(expected, Is.EqualTo(translator.EnglishToFrench(original)));
      }

      [Test]
      public void Translation_Should_Log_Message()
      {
         var logged = "Nothing";
         var original = "Original";

         var log = new Action<string>(
            input => 
            { logged = original; });

         var translate = new Func<string, string>(
            (input) => "");

         var translator = new Translator(translate, log);
         translator.EnglishToFrench(original);

         Assert.That(logged, Is.EqualTo(original));
      }
   }
}
