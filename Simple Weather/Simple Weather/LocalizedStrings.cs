namespace Simple_Weather
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static Simple_Weather.AppResources localizedResources = new Simple_Weather.AppResources();

        public Simple_Weather.AppResources LocalizedResources { get { return localizedResources; } }
    }
}
