namespace Juga.Abstractions.Caching.CacheManagement
{
    public class ValueArgumentSuffixSelector : ICacheKeySuffixSelector
    {
        public int ArgumentIndex { get; set; }

        public ValueArgumentSuffixSelector(int argumentIndex)
        {
            if (argumentIndex < 0)
            {
                throw new ArgumentException($"{nameof(argumentIndex)} must greater than 0.");
            }
            ArgumentIndex = argumentIndex;
        }

        public string GetSuffix(object[] arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            if (arguments.Length < ArgumentIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(ArgumentIndex));
            }

            if (arguments[ArgumentIndex] == null)
            {
                throw new ArgumentNullException($"Value in index {ArgumentIndex} is null.");
            }
            return arguments[ArgumentIndex].ToString();
        }
    }
}