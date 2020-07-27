using BikeDataProject.FileUpload.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;

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
        private ConfigurationDetails ConfigurationDetails {get;set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="FileController"> class.</see>
        /// </summary>
        /// <param name="configDetails">The configuration details.</param>
        public FileController(ConfigurationDetails configDetails)
        {
            this.ConfigurationDetails = configDetails;
        }

        /// <summary>
        /// Validates a file to check if it's has a good format
        /// </summary>
        /// <returns>Status code that informs on the file validation.</returns>
        [HttpPost("/File/Upload")]
        public IActionResult ValidateFile()
        {
            try
            {
                if (Request.Form.Files == null || Request.Form.Files.Count == 0)
                {
                    return this.Problem("File not found", statusCode: 400);
                }
            }
            catch (Exception e)
            {
                return this.Problem($"Unhandled exception: {e.Message}", statusCode: 500);
            }

            return this.Problem("Unhandled situation", statusCode: 500);
        }
    }
}