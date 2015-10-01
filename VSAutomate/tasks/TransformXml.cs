// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformXml.cs" company="wwwlicious">
//   Copyright (c) All Rights Reserved
// </copyright>
// <summary>
//   Defines the TransformXml type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VSAutomate
{
    using System;
    using System.Globalization;
    using System.Xml;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.Web.XmlTransform;

    /// <summary>
    /// The transform xml.
    /// </summary>
    public class TransformXml : Task
    {
        /// <summary>
        /// The _source root path.
        /// </summary>
        private string _sourceRootPath = string.Empty;

        /// <summary>
        /// The _transform root path.
        /// </summary>
        private string _transformRootPath = string.Empty;

        /// <summary>
        /// The _source file.
        /// </summary>
        private string _sourceFile;

        /// <summary>
        /// The _transform file.
        /// </summary>
        private string _transformFile;

        /// <summary>
        /// The _destination file.
        /// </summary>
        private string _destinationFile;

        /// <summary>
        /// The stack trace.
        /// </summary>
        private bool stackTrace;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [Required]
        public string Source
        {
            get
            {
                return Utility.GetFilePathResolution(this._sourceFile, this.SourceRootPath);
            }

            set
            {
                this._sourceFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the source root path.
        /// </summary>
        public string SourceRootPath
        {
            get
            {
                return this._sourceRootPath;
            }

            set
            {
                this._sourceRootPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        [Required]
        public string Transform
        {
            get
            {
                return Utility.GetFilePathResolution(this._transformFile, this.TransformRootPath);
            }

            set
            {
                this._transformFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the transform root path.
        /// </summary>
        public string TransformRootPath
        {
            get
            {
                if (string.IsNullOrEmpty(this._transformRootPath))
                    return this.SourceRootPath;
                return this._transformRootPath;
            }

            set
            {
                this._transformRootPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        [Required]
        public string Destination
        {
            get
            {
                return this._destinationFile;
            }

            set
            {
                this._destinationFile = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether stack trace.
        /// </summary>
        public bool StackTrace
        {
            get
            {
                return this.stackTrace;
            }

            set
            {
                this.stackTrace = value;
            }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Execute()
        {
            var flag = true;
            var logger = (IXmlTransformationLogger)new TaskTransformationLogger(this.Log, this.StackTrace);
            var xmlTransformation = (XmlTransformation)null;
            var document = (XmlTransformableDocument)null;
            try
            {
                logger.StartSection(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Starting xdt transformation {0}", new[] { (object)this.Source }));
                document = this.OpenSourceFile(this.Source);
                logger.LogMessage(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Applying transformation {0}", new[] { (object)this.Transform }));
                xmlTransformation = this.OpenTransformFile(this.Transform, logger);
                flag = xmlTransformation.Apply((XmlDocument)document);
                if (flag)
                {
                    logger.LogMessage(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Transforming output {0}", new[] { (object)this.Destination }));
                    this.SaveTransformedFile(document, this.Destination);
                }
            }
            catch (XmlException ex)
            {
                var file = this.Source;
                if (!string.IsNullOrEmpty(ex.SourceUri))
                    file = new Uri(ex.SourceUri).LocalPath;
                logger.LogError(file, ex.LineNumber, ex.LinePosition, ex.Message);
                flag = false;
            }
            catch (Exception ex)
            {
                logger.LogErrorFromException(ex);
                flag = false;
            }
            finally
            {
                logger.EndSection(string.Format((IFormatProvider)CultureInfo.CurrentCulture, flag ? "The xdt transformation succeeded {0}" : "The xdt transformation failed {0}", new object[0]));
                if (xmlTransformation != null)
                    xmlTransformation.Dispose();
                if (document != null)
                    document.Dispose();
            }

            return flag;
        }

        /// <summary>
        /// The save transformed file.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="destinationFile">
        /// The destination file.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        private void SaveTransformedFile(XmlTransformableDocument document, string destinationFile)
        {
            try
            {
                document.Save(destinationFile);
            }
            catch (XmlException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Writing to destination failed {0}", new[] { (object)ex.Message }), ex);
            }
        }

        /// <summary>
        /// The open source file.
        /// </summary>
        /// <param name="sourceFile">
        /// The source file.
        /// </param>
        /// <returns>
        /// The <see cref="XmlTransformableDocument"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        private XmlTransformableDocument OpenSourceFile(string sourceFile)
        {
            try
            {
                var transformableDocument = new XmlTransformableDocument();
                transformableDocument.PreserveWhitespace = true;
                transformableDocument.Load(sourceFile);
                return transformableDocument;
            }
            catch (XmlException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Loading transformation source failed {0}", new[] { (object)ex.Message }), ex);
            }
        }

        /// <summary>
        /// The open transform file.
        /// </summary>
        /// <param name="transformFile">
        /// The transform file.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="XmlTransformation"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        private XmlTransformation OpenTransformFile(string transformFile, IXmlTransformationLogger logger)
        {
            try
            {
                return new XmlTransformation(transformFile, logger);
            }
            catch (XmlException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Loading transform failed {0}", new[] { (object)ex.Message }), ex);
            }
        }
    }
}