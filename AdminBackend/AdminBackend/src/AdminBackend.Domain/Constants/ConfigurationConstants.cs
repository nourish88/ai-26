namespace AdminBackend.Domain.Constants
{
    public class ConfigurationConstants
    {
        #region[SearchServiceConfigurationDictionaryKeys]

        public static readonly string SearchServiceUriDictionaryKey = "ServiceUri";
        public static readonly string SearchServiceUserNameDictionaryKey = "ServiceUserName";
        public static readonly string SearchServicePasswordDictionaryKey = "ServicePassword";


        #endregion[END_SearchServiceConfigurationDictionaryKeys]

        #region[EmbeddingServiceConfigurationDictionaryKeys]

        public const string EmbeddingServiceUriDictionaryKey = "EmbeddingServiceUri";
        public const string EmbeddingServiceApiKeyDictionaryKey = "EmbeddingServiceApiKey";
        public const string EmbeddingServiceDefaultApiKeyDictionaryKey = "Default";// Embedding servisi için default api key belirlenmesi için kullanılır
        public const string EmbeddingServiceModelNameDictionaryKey = "EmbeddingServiceModelName";

        #endregion[ENDEmbeddingServiceConfigurationDictionaryKeys]
    }
}
