using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ALISS.Business.Services
{
    public class BlobStorageService
    {
        MemoryStream _imageContent;

        public BlobStorageService()
        {
            _imageContent = new MemoryStream();
        }

        public bool IsValidImage(HttpPostedFileBase fileToUpload, out string validationError, string imageName = "")
        {
            string error = "";
            if (fileToUpload.ContentType == "image/jpeg" || fileToUpload.ContentType == "image/png")
            {
                if (fileToUpload.ContentLength > 2621440)
                {
                    error += "The " + imageName + "image you have uploaded is too large. Please upload a file less than 2.5MB. ";
                }
                fileToUpload.InputStream.CopyTo(_imageContent);
                var img = Image.FromStream(_imageContent, true, true);
                int w = img.Width;
                int h = img.Height;
                if (w < 80 || h < 80)
                {
                    error += "The " + imageName + "image you have uploaded is too small. Please upload an image that is bigger than 80 pixels in both width and height. ";
                }
            }
            else
            {
                error += "The " + imageName + "image you upload must be a jpg or png image. Please upload a file in the correct format.";
            }

            validationError = error;

            return String.IsNullOrEmpty(error);
        }

        public bool IsValidImage(string imageData, out string validationError, string imageName = "")
        {
            string error = "";
            if (imageData.Contains("image/jpeg") || imageData.Contains("image/png"))
            {
                byte[] imageBytes = Convert.FromBase64String(imageData.Substring(imageData.IndexOf(',') + 1));
                if (imageBytes.Length > 2621440)
                {
                    error += "The " + imageName + "image you have uploaded is too large. Please upload a file less than 2.5MB. ";
                }
                _imageContent.Write(imageBytes, 0, imageBytes.Length);
                var img = Image.FromStream(_imageContent, true, true);
                int w = img.Width;
                int h = img.Height;
                if (w < 80 || h < 80)
                {
                    error += "The " + imageName + "image you have uploaded is too small. Please upload an image that is bigger than 80 pixels in both width and height. ";
                }
            }
            else
            {
                error += "The " + imageName + "image you upload must be a jpg or png image. Please upload a file in the correct format.";
            }

            validationError = error;

            return String.IsNullOrEmpty(error);
        }

        public string UploadLogoToBlobStorage(Guid id, HttpPostedFileBase fileToUpload)
        {
            string connectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"].ToString();
            string containerName = "files";
            string suffix = ConfigurationManager.AppSettings["BlobNameSuffix"].ToString();
            string[] fileNameParts = fileToUpload.FileName.Split('.');
            string blobName = $"{id}{suffix}.{fileNameParts[fileNameParts.Length - 1]}";

            BlobClient client = new BlobClient(connectionString, containerName, blobName);
            _imageContent.Position = 0;
            client.Upload(_imageContent, true);
            return client.Uri.ToString();
        }

        public string UploadLogoToBlobStorage(Guid id, string imageName)
        {
            string connectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"].ToString();
            string containerName = "files";
            string suffix = ConfigurationManager.AppSettings["BlobNameSuffix"].ToString();
            string[] fileNameParts = imageName.Split('.');
            string blobName = $"{id}{suffix}.{fileNameParts[fileNameParts.Length - 1]}";

            BlobClient client = new BlobClient(connectionString, containerName, blobName);
            _imageContent.Position = 0;
            client.Upload(_imageContent, true);
            return client.Uri.ToString();
        }

        public void DeleteImageFromBlobStorage(string blobUrl)
        {
            string connectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"].ToString();
            string containerName = "files";
            string blobName = blobUrl.Substring(blobUrl.LastIndexOf("/") + 1);

            BlobClient client = new BlobClient(connectionString, containerName, blobName);
            client.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
