using System;

namespace Grynwald.MdDocs.ApiReference.Model.XmlDocs
{
    /// <summary>
    /// Represents a bold text element in XML documentation comments.
    /// </summary>
    /// <remarks>
    /// For a list of tags in documentation comments, see
    /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments
    /// </remarks>
    public class BoldElement : Element, IEquatable<BoldElement>
    {
        /// <summary>
        /// Gets the bold text elements content
        /// </summary>
        public TextBlock Content { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="BoldElement"/>.
        /// </summary>
        /// <param name="content">The text element's content.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
        public BoldElement(TextBlock content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <inheritdoc />
        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        /// <inheritdoc />
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Content);

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as BoldElement);

        /// <inheritdoc />
        public bool Equals(BoldElement? other) => other != null && StringComparer.Ordinal.Equals(Content, other.Content);
    }
}
