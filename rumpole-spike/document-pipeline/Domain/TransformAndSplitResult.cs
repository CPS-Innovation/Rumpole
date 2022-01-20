using System.Collections.Generic;
using System.IO;

namespace Domain
{
  public class TransformAndSplitResult
  {
    public BlobNameAndSasLinkUrl Pdf { get; set; }

    public List<BlobNameAndSasLinkUrl> Pngs { get; set; }
  }
}