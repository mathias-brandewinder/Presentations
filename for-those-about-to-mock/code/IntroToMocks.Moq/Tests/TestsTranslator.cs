using log4net;
using Moq;
using NUnit.Framework;

namespace IntroToMocks.Moq.Tests
{
   [TestFixture]
   public class TestsTranslator
   {
      [Test]
      public void Translation_Should_Return_Result_From_Client()
      {
         var log = new Mock<ILog>();
         var client = new Mock<ITranslationClient>();

         var translator = new Translator(log.Object, client.Object);

         var original = "Hello";
         var expected = "Bonjour";

         client.Setup(me => me.EnglishToFrench(original))
            .Returns(expected);

         Assert.That(expected, Is.EqualTo(translator.EnglishToFrench(original)));
      }

      [Test]
      public void Translation_Should_Log_Message()
      {
         var log = new Mock<ILog>();
         var client = new Mock<ITranslationClient>();

         var translator = new Translator(log.Object, client.Object);

         translator.EnglishToFrench("Hello");

         log.Verify(me => me.Debug("Translating"));
      }
   }
}
