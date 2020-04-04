namespace Infrastructure.Contracts
{
    /// <summary>
    /// Abstract class with main properties for Get records methods
    /// </summary>
    public abstract class GetListRecordsResponse
    {
        #region Properties

        /// <summary>
        /// Amount of records which was skipped
        /// </summary>
        public int Skipped { get; set; }

        /// <summary>
        /// Amount of records which was taken
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Total amount of records
        /// </summary>
        public int TotalAmount { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetListRecordsResponse(int skipped, int limit, int totalAmount)
        {
            Skipped = skipped;
            Limit = limit;
            TotalAmount = totalAmount;
        }

        #endregion
    }
}