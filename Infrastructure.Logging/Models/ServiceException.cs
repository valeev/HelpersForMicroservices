using System;

namespace Infrastructure.Logging
{
    /// <summary>
    /// Custom service exception
    /// </summary>
    public class ServiceException : Exception
    {
        #region Properties

        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; }

        #endregion

        #region Constructors

        protected ServiceException()
        {
        }

        public ServiceException(string code)
        {
            Code = code;
        }

        public ServiceException(string message, params object[] args)
            : this(string.Empty, message, args)
        {
        }

        public ServiceException(string code, string message, params object[] args)
            : this(null, code, message, args)
        {
        }

        public ServiceException(Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        public ServiceException(Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }

        #endregion
    }
}
