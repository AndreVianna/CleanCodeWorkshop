namespace XPenC.BusinessLogic.Validation
{
    public class ValidationError
    {
        public ValidationError()
        {
        }

        public ValidationError(string source, string message) : this()
        {
            Source = source;
            Message = message;
        }

        public string Source { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return $"{Source} => {Message}";
        }
    }
}