using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BikeDataProject.FileUpload.Configuration
{
    /// <summary>
    /// Contains the environment variables used throughout the project.
    /// </summary>
    public class ConfigurationDetails
    {
        public string FilePath { get; set; }

        public List<string> Extensions { get; set; }

        public long SizeLimit { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDetails"> class.</see>
        /// </summary>
        /// <param name="filePath">The file path to store the files that we receive.</param>
        protected ConfigurationDetails(string filePath, List<string> extensions, long sizeLimit)
        {
            this.FilePath = filePath;
            this.Extensions = extensions;
            this.SizeLimit = sizeLimit;
        }

        public static ConfigurationDetails FromConfiguration(IConfiguration configuration)
        {
            var filePath = File.ReadAllText(configuration[$"{Program.EnvVarPrefix}FILE_PATH"]);
            var extensions = File.ReadAllText(configuration[$"{Program.EnvVarPrefix}FILE_EXTENSIONS"]);
            var sizeLimit = File.ReadAllText(configuration[$"{Program.EnvVarPrefix}FILE_SIZE_LIMIT"]);
            long size;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Log.Fatal("File Path is not set");
                throw new Exception("Environment variable for the file path is not set");
            }

            if (string.IsNullOrWhiteSpace(extensions))
            {
                Log.Fatal("Extensions not set");
                throw new Exception("Enviromnent variable for extensions not set");
            }

            if (string.IsNullOrWhiteSpace(sizeLimit))
            {
                Log.Fatal("Size Limit not set");
                throw new Exception("Environmnent variable for the size limit is not set");
            }
            else if (!long.TryParse(sizeLimit, out size))
            {
                Log.Fatal("Size limit is not a number");
                throw new Exception("Size limit is not a number");
            }

            return new ConfigurationDetails(filePath, extensions.Split(";").ToList(), size);
        }
    }
}