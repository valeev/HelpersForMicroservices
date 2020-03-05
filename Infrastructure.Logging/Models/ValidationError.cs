namespace Infrastructure.Logging
{
    /// <summary>
    /// Validation error
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Error description
        /// </summary>
        public string Description { get; set; }
    }
}
