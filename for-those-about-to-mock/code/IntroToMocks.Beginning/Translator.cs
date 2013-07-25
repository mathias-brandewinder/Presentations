using Google.API.Translate;
using log4net;

namespace IntroToMocks.Beginning
{
   public class Translator
   {
      private readonly ILog log;
      private readonly TranslateClient client;

      public Translator()
      {
         this.log = LogManager.GetLogger("Translator");
         this.client = new TranslateClient("http://www.mysite.com");
      }

      public string EnglishToFrench(string original)
      {
         this.log.Debug("Translating");
         var result = client.Translate(
            original, 
            Language.English, 
            Language.French);
         return result;
      }
   }
}
