using System;
using System.Collections.Generic;
using System.Text;

namespace Services.BlobStorageService
{
    public class BlobStorageOptions
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
