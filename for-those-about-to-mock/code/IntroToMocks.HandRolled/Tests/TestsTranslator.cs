using NUnit.Framework;

namespace IntroToMocks.HandRolled.Tests
{
   [TestFixture]
   public class TestsTranslator
   {
      [Test]
      public void When_Translating_Original_Should_Be_Passed_To_Client_And_Return_Result()
      {
         var log = new FakeLog();
         var client = new FakeClient();

         var translator = new Translator(log, client);

         var original = "Hello";
         var expected = "Bonjour";
         client.Setup(original, expected);

         Assert.That(expected, Is.EqualTo(translator.EnglishToFrench(original)));
      }

      [Test]
      public void When_Translating_Message_Should_Be_Passed_To_Log()
      {
         var log = new FakeLog();
         var client = new FakeClient();

         var translator = new Translator(log, client);

         translator.EnglishToFrench("Hello");

         Assert.IsTrue(log.LastCallWasDebug);
         Assert.That("Translating", Is.EqualTo(log.LastMessage));
      }
   }
}
