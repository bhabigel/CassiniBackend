using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CassiniConnect.Core.Utilities.Config;
using MediatR;

namespace CassiniConnect.Application.Utilities
{
    public class FileService : IFileService
    {
        private readonly StorageSettings storageSettings;
        public FileService(StorageSettings storageSettings)
        {
            this.storageSettings = storageSettings;
        }

        public async Task<Unit> SaveFileAsync(byte[] fileData, string fileName, CancellationToken cancellationToken)
        {
            if (fileData == null || string.IsNullOrEmpty(fileName))
            {
                throw new Exception("Obligatory fields related to file are empty or null!");
            }
            if (string.IsNullOrEmpty(storageSettings.StorageMount) || string.IsNullOrEmpty(storageSettings.TeacherImageFolder))
            {
                throw new Exception("Storage settings are empty or null!");
            }
            var directory = Path.Combine(storageSettings.StorageMount, storageSettings.TeacherImageFolder);
            if (!Directory.Exists(directory))
            {
                throw new Exception("Directory was not found at storage path!");
            }

            var path = Path.Combine(directory, fileName);
            await File.WriteAllBytesAsync(path, fileData, cancellationToken);
            return Unit.Value;
        }

        public void DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("Obligatory fields related to file are empty or null!");
            }
            if (string.IsNullOrEmpty(storageSettings.StorageMount) || string.IsNullOrEmpty(storageSettings.TeacherImageFolder))
            {
                throw new Exception("Storage settings are empty or null!");
            }
            var directory = Path.Combine(storageSettings.StorageMount, storageSettings.TeacherImageFolder);
            if (!Directory.Exists(directory))
            {
                throw new Exception("Directory was not found at storage path!");
            }
            var path = Path.Combine(directory, fileName);
            File.Delete(path);
        }
    }
}