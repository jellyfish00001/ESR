using Minio.DataModel;
using System.IO;
using System.Threading.Tasks;

namespace ERS.Minio
{
    public interface IMinioRepository
    {
        Task MakeBucketAsync(string bucketName, string area);

        Task<bool> BucketExistsAsync(string bucketName, string area);


        Task RemoveObjectAsync(string objectName, string area);

        MemoryStream GetObjectAsync(string objectName, string area);

        Task PutObjectAsync(string objectName, Stream data, string fileType, string area);
        Task PutObjectAsync(string objectName, string filePath, string contentType, string area);

        Task<ObjectStat> StatObjectAsync(string objectName, string area);

        Task<string> PresignedGetObjectAsync(string objectName, string area, int expiresInt = 7 * 24 * 60 * 60);

        void DeleteFiles(string area, string objectPrefix = null, bool recursive = true);

        string GetMIMEType(string extension);
        Task CopyObjectAsync(string srcObjectName, string destObjectName, string area);
    }
}
