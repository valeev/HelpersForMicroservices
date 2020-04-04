namespace Infrastructure.Models.Db
{
    /// <summary>
    /// Base properties for models
    /// </summary>
    public abstract class BaseProperty
    {
        #region Properties

        /// <summary>
        /// Unique id
        /// </summary>
        public int Id { get; set; }

        #endregion
    }
}