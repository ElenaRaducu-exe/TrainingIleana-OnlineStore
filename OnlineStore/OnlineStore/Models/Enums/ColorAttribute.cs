namespace OnlineStore.Models.Enums
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ColorAttribute : Attribute
    {
        public string HexColor { get; }

        public ColorAttribute(string hexColor)
        {
            HexColor = hexColor;
        }
    }
}