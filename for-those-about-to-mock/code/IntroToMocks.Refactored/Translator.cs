using log4net;

namespace IntroToMocks.Refactored
{
   public class Translator
   {
      private readonly ILog log;
      private readonly ITranslationClient client;

      public Translator(ILog log, ITranslationClient client)
      {
         this.log = log;
         this.client = client;
      }

      public string EnglishToFrench(string original)
      {
         this.log.Debug("Translating");
         var result = client.EnglishToFrench(original);
         return result;
      }
   }
}
