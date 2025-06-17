using System.ComponentModel;

namespace Serene.Modules.Documents // Corrected namespace based on previous subtask's DocumentWorkflowPermissionHandler
{
    /// <summary>
    /// Represents the type of a document, influencing its accessibility and handling.
    /// </summary>
    [EnumKey("Documents.DocumentType")]
    public enum DocumentType
    {
        /// <summary>
        /// Publicly accessible documents.
        /// </summary>
        [Description("Public Document")]
        Public = 1,

        /// <summary>
        /// Documents intended for internal use, typically requiring specific roles for access.
        /// </summary>
        [Description("Internal Document")]
        Internal = 2,

        // Consider if 'Casual' and 'Annual' should be preserved with different values,
        // or if they are entirely replaced. For this task, they are replaced as per instructions.
        // If they were to be kept:
        // [Description("Casual Document")]
        // Casual = 3,
        // [Description("Annual Document")]
        // Annual = 4
    }
}
