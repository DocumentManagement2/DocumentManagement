using System;
using System.IO;

namespace DocumentManagementCommon
{
    public class DocumentBlobInfo
    {
        public Uri BlobUri { get; set; }

        public string BlobName
        {
            get
            {
                return BlobUri.Segments[BlobUri.Segments.Length - 1];
            }
        }
        public string BlobNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(BlobName);
            }
        }
        public string FileExtension
        {
            get
            {
                return Path.GetExtension(BlobName);
            }
        }
        public string DocumentId { get; set; }
    }
}
