using System.Globalization;

namespace Domain.Models.Common
{
    public class Name
    {
        public string Kk { get; set; }
        public string Ru { get; set; }
        public string En { get; set; }

        public string GetName() =>
            this.GetType()
                .GetProperty(
                    char.ToUpper(CultureInfo.CurrentCulture.TwoLetterISOLanguageName[0])
                    + CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Substring(1)
                )
                .GetValue(this, null)
                .ToString();

        public string GetName(string locale) =>
            this.GetType()
                .GetProperty(
                    char.ToUpper(locale[0])
                    + locale.Substring(1)
                )
                .GetValue(this, null)
                .ToString();
    }
}
