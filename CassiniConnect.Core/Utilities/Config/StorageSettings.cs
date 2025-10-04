using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassiniConnect.Core.Utilities.Config
{
    public class StorageSettings
    {
        public string StorageMount { get; set; } = string.Empty;
        public string TeacherImageFolder { get; set; } = string.Empty;
        public string PresenterImageFolder { get; set; } = string.Empty;
    }
}