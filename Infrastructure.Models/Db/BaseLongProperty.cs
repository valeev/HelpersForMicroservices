namespace Infrastructure.Models.Db
{
    /// <summary>
    /// Base properties for models
    /// </summary>
    public abstract class BaseLongProperty
    {
        #region Properties

        /// <summary>
        /// Unique id
        /// </summary>
        public long Id { get; set; }

        #endregion
    }
}