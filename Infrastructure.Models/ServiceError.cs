using Newtonsoft.Json;

namespace Infrastructure.Models
{
    /// <summary>
    /// Common service error
    /// </summary>
    public class ServiceError
    {
        #region Properties

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return JSON string of current class object
        /// </summary>
        /// <returns>Stringified json</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}