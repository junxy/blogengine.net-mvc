namespace BlogEngine.Core.Json
{
    /// <summary>
    /// The json response.
    /// </summary>
    public class JsonResponse
    {
        #region Properties

        /// <summary>
        ///     Gets or sets Data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        ///     Gets or sets Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Success.
        /// </summary>
        public bool Success { get; set; }

        #endregion
    }
}