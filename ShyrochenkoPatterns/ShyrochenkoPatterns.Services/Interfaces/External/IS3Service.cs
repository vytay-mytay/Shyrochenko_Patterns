using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.External
{
    public interface IS3Service
    {
        /// <summary>
        /// Uploads file to AWS S3 source
        /// </summary>
        /// <param name="stream">uploading file stream</param>
        /// <param name="key">output file name</param>
        /// <returns></returns>
        Task<string> UploadFile(Stream stream, string key);

        /// <summary>
        /// Remove file from AWS S3
        /// </summary>
        /// <param name="name">File name</param>
        Task DeleteFile(string name);
    }
}
