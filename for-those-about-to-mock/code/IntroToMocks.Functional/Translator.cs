namespace IntroToMocks.Functional
{
   using System;

   public class Translator
   {
      private readonly Action<string> log;
      private readonly Func<string, string> translate;

      public Translator(Func<string, string> translate, Action<string> log)
      {
         this.log = log;
         this.translate = translate;
      }

      public string EnglishToFrench(string original)
      {
         this.log(original);
         var result = translate(original); 
         return result;
      }
   }
}
