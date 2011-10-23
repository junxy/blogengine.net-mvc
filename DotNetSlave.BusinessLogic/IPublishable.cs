namespace BlogEngine.Core
{
    using System;

    /// <summary>
    /// An interface implemented by the classed that can be published.
    ///     <remarks>
    /// To implemnet this interface means that the class can be searched
    ///         from the search page and that it can be syndicated in RSS and ATOM.
    ///     </remarks>
    /// </summary>
    public interface IPublishable
    {
        #region Properties

        /// <summary>
        ///     Gets the absolute link.
        /// </summary>
        /// <value>The absolute link.</value>
        Uri AbsoluteLink { get; }

        /// <summary>
        ///     Gets the author.
        /// </summary>
        /// <value>The author.</value>
        string Author { get; }

        /// <summary>
        ///     Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        StateList<Category> Categories { get; }

        /// <summary>
        ///     Gets the content.
        /// </summary>
        /// <value>The content.</value>
        string Content { get; }

        /// <summary>
        ///     Gets the date created.
        /// </summary>
        /// <value>The date created.</value>
        DateTime DateCreated { get; }

        /// <summary>
        ///     Gets the date modified.
        /// </summary>
        /// <value>The date modified.</value>
        DateTime DateModified { get; }

        /// <summary>
        ///     Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        ///     Gets the id.
        /// </summary>
        /// <value>The published item id.</value>
        Guid Id { get; }

        /// <summary>
        ///     Gets a value indicating whether or not this item is published.
        /// </summary>
        bool IsPublished { get; }

        /// <summary>
        ///     Gets the relative link.
        /// </summary>
        /// <value>The relative link.</value>
        string RelativeLink { get; }

        /// <summary>
        ///     Gets the title of the object
        /// </summary>
        string Title { get; }

        /// <summary>
        ///     Gets a value indicating whether or not this item should be shown.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        ///     Gets a value indicating whether or not this item is visible to the public.
        /// </summary>
        bool IsVisibleToPublic { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the <see cref="E:Serving"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="BlogEngine.Core.ServingEventArgs"/> instance containing the event data.</param>
        void OnServing(ServingEventArgs eventArgs);

        #endregion
    }
}