namespace IntroToMocks.HandRolled.Tests
{
   class FakeClient : ITranslationClient
   {
      private string expectedOriginal;
      private string expectedTranslation;

      #region ITranslationClient Members

      public string EnglishToFrench(string original)
      {
         if (original == expectedOriginal)
         {
            return expectedTranslation;
         }

         return null;
      }

      #endregion

      internal void Setup(string original, string expected)
      {
         this.expectedOriginal = original;
         this.expectedTranslation = expected;
      }
   }
}
