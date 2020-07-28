using BikeDataProject.FileUpload.Configuration;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace BikeDataProject.FileUpload.Controllers
{
    /// <summary>
    /// Contains the code needed to validata a file
    /// </summary>
    public class FileController : ControllerBase
    {
        /// <summary>
        /// Contains the details from the configuration.
        /// </summary>
        private ConfigurationDetails ConfigurationDetails { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"> class.</see>
        /// </summary>
        /// <param name="configDetails">The configuration details.</param>
        public FileController(ConfigurationDetails configDetails)
        {
            this.ConfigurationDetails = configDetails;
        }

        [HttpGet("/")]
        public IActionResult Test() => this.Ok("{test: 'Hello World'}");

        /// <summary>
        /// Validates a file to check if it's has a good format
        /// </summary>
        /// <returns>Status code that informs on the file validation.</returns>
        [HttpPost("/Upload")]
        public IActionResult ValidateFile()
        {
            var request = Request;
            try
            {
                // To receive file
                if (Request.ContentType == null || !Request.ContentType.Contains("multipart/form-data"))
                {
                    Log.Error($"Content type is invalid: {Request.ContentType}");
                    return this.Problem("Content-Type invalid", statusCode: 400);
                }

                // Checks if the request contains a file
                if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                {
                    Log.Error("File not found in the request");
                    return this.Problem("File not found", statusCode: 400);
                }

                foreach (var file in Request.Form.Files)
                {
                    // Checks if the file is empty
                    if (file.Length == 0)
                    {
                        Log.Error("File is empty");
                        continue;
                    }

                    // Checks if the file is not too big
                    if (file.Length >= this.ConfigurationDetails.SizeLimit)
                    {
                        Log.Error("File too big");
                        continue;
                    }

                    // Checks if the file format is valid
                    var extension = file.FileName.Split(".").Last();
                    if (!this.ConfigurationDetails.Extensions.Contains(extension.ToUpper()))
                    {
                        Log.Error("File extension is invalid");
                        continue;
                    }

                    // Writes it to the said directory
                    var fullPath = Path.Combine(this.ConfigurationDetails.FilePath, $"{Guid.NewGuid()}.{extension}");
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        try
                        {
                            file.CopyTo(stream);
                        }
                        catch (Exception ex)
                        {
                            Log.Fatal($"Error during the copy of the file: {ex.Message}");
                            return this.Problem($"Problem happened when uploading the file: {ex.Message}", statusCode: 500);
                        }
                    }
                }

                return this.Ok("{'status': 'OK', response: 'Files uploaded'}");

            }
            catch (Exception e)
            {
                Log.Fatal($"Unhandled exception: {e.Message}");
                return this.Problem($"Unhandled exception: {e.Message}", statusCode: 500);
            }
        }
    }
}
