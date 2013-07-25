using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace IntroToMocks.Rhino.Tests
{
   [TestFixture]
   public class TestsTranslator
   {
      [Test]
      public void Translation_Should_Return_Result_From_Client()
      {
         var log = MockRepository.GenerateStub<ILog>();
         var client = MockRepository.GenerateStub<ITranslationClient>();

         var translator = new Translator(log, client);

         var original = "Hello";
         var expected = "Bonjour";

         client.Stub(me => me.EnglishToFrench(original))
            .Return(expected);

         Assert.That(expected, Is.EqualTo(translator.EnglishToFrench(original)));
      }

      [Test]
      public void Translation_Should_Log_Message()
      {
         var log = MockRepository.GenerateMock<ILog>();
         var client = MockRepository.GenerateStub<ITranslationClient>();

         var translator = new Translator(log, client);

         translator.EnglishToFrench("Hello");

         log.AssertWasCalled(me => me.Debug("Translating"));
      }
   }
}
