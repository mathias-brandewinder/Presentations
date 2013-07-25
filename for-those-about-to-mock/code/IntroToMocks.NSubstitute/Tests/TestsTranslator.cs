using log4net;
using NSubstitute;
using NUnit.Framework;

namespace IntroToMocks.NSubstitute.Tests
{
   [TestFixture]
   public class TestsTranslator
   {
      [Test]
      public void Translation_Should_Return_Result_From_Client()
      {
         var log = Substitute.For<ILog>();
         var client = Substitute.For<ITranslationClient>();

         var translator = new Translator(log, client);

         var original = "Hello";
         var expected = "Bonjour";

         client.EnglishToFrench(original).Returns(expected);

         Assert.That(expected, Is.EqualTo(translator.EnglishToFrench(original)));
      }

      [Test]
      public void Translation_Should_Log_Message()
      {
         var log = Substitute.For<ILog>();
         var client = Substitute.For<ITranslationClient>();

         var translator = new Translator(log, client);

         translator.EnglishToFrench("Hello");

         log.Received().Debug("Translating");
      }
   }
}
