using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace CassiniConnect.Application.Utilities
{
    public interface IFileService
    {
        Task<Unit> SaveFileAsync(byte[] fileData, string fileName, CancellationToken cancellationTokens);
        void DeleteFileAsync(string fileName);
    }
}