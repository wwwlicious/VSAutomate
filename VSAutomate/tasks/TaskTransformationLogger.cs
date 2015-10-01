namespace VSAutomate
{
    using System;
    using System.Text;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using Microsoft.Web.XmlTransform;

    /// <summary>
    /// The task transformation logger. From Microsoft Publishimg Web Assemblies
    /// </summary>
    internal class TaskTransformationLogger
    {
        /// <summary>
        /// The indent string piece.
        /// </summary>
        private readonly string indentStringPiece = "  ";

        /// <summary>
        /// The logging helper.
        /// </summary>
        private TaskLoggingHelper loggingHelper;

        /// <summary>
        /// The indent level.
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// The indent string.
        /// </summary>
        private string indentString;

        /// <summary>
        /// The stack trace.
        /// </summary>
        private bool stackTrace;

        /// <summary>
        /// Gets the indent string.
        /// </summary>
        private string IndentString
        {
            get
            {
                if (this.indentString == null)
                {
                    this.indentString = string.Empty;
                    for (var index = 0; index < this.indentLevel; ++index)
                        this.indentString += this.indentStringPiece;
                }

                return this.indentString;
            }
        }

        /// <summary>
        /// Gets or sets the indent level.
        /// </summary>
        private int IndentLevel
        {
            get
            {
                return this.indentLevel;
            }

            set
            {
                if (this.indentLevel == value)
                    return;
                this.indentLevel = value;
                this.indentString = (string)null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTransformationLogger"/> class.
        /// </summary>
        /// <param name="loggingHelper">
        /// The logging helper.
        /// </param>
        public TaskTransformationLogger(TaskLoggingHelper loggingHelper)
            : this(loggingHelper, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTransformationLogger"/> class.
        /// </summary>
        /// <param name="loggingHelper">
        /// The logging helper.
        /// </param>
        /// <param name="stackTrace">
        /// The stack trace.
        /// </param>
        public TaskTransformationLogger(TaskLoggingHelper loggingHelper, bool stackTrace)
        {
            this.loggingHelper = loggingHelper;
            this.stackTrace = stackTrace;
        }

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogMessage(string message, params object[] messageArgs)
        {
            ((IXmlTransformationLogger)this).LogMessage(MessageType.Normal, message, messageArgs);
        }

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            MessageImportance importance;
            switch (type)
            {
                case MessageType.Normal:
                    importance = MessageImportance.Normal;
                    break;
                case MessageType.Verbose:
                    importance = MessageImportance.Low;
                    break;
                default:
                    importance = MessageImportance.Normal;
                    break;
            }

            this.loggingHelper.LogMessage(importance, this.IndentString + message, messageArgs);
        }

        /// <summary>
        /// The log warning.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogWarning(string message, params object[] messageArgs)
        {
            this.loggingHelper.LogWarning(message, messageArgs);
        }

        /// <summary>
        /// The log warning.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            ((IXmlTransformationLogger)this).LogWarning(file, 0, 0, message, messageArgs);
        }

        /// <summary>
        /// The log warning.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="linePosition">
        /// The line position.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.loggingHelper.LogWarning((string)null, (string)null, (string)null, file, lineNumber, linePosition, 0, 0, this.loggingHelper.FormatString(message, messageArgs));
        }

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogError(string message, params object[] messageArgs)
        {
            this.loggingHelper.LogError(message, messageArgs);
        }

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogError(string file, string message, params object[] messageArgs)
        {
            ((IXmlTransformationLogger)this).LogError(file, 0, 0, message, messageArgs);
        }

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="linePosition">
        /// The line position.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.loggingHelper.LogError((string)null, (string)null, (string)null, file, lineNumber, linePosition, 0, 0, this.loggingHelper.FormatString(message, messageArgs));
        }

        /// <summary>
        /// The log error from exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void LogErrorFromException(Exception ex)
        {
            this.loggingHelper.LogErrorFromException(ex, this.stackTrace, this.stackTrace, (string)null);
        }

        /// <summary>
        /// The log error from exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        public void LogErrorFromException(Exception ex, string file)
        {
            this.loggingHelper.LogErrorFromException(ex, this.stackTrace, this.stackTrace, file);
        }

        /// <summary>
        /// The log error from exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="linePosition">
        /// The line position.
        /// </param>
        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            var message = ex.Message;
            if (this.stackTrace)
            {
                var stringBuilder = new StringBuilder();
                for (var exception = ex; exception != null; exception = exception.InnerException)
                {
                    stringBuilder.AppendFormat("{0} : {1}", (object)exception.GetType().Name, (object)exception.Message);
                    stringBuilder.AppendLine();
                    if (!string.IsNullOrEmpty(exception.StackTrace))
                        stringBuilder.Append(exception.StackTrace);
                }

                message = stringBuilder.ToString();
            }

            ((IXmlTransformationLogger)this).LogError(file, lineNumber, linePosition, message);
        }

        /// <summary>
        /// The start section.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void StartSection(string message, params object[] messageArgs)
        {
            ((IXmlTransformationLogger)this).StartSection(MessageType.Normal, message, messageArgs);
        }

        /// <summary>
        /// The start section.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            ((IXmlTransformationLogger)this).LogMessage(type, message, messageArgs);
            ++this.IndentLevel;
        }

        /// <summary>
        /// The end section.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void EndSection(string message, params object[] messageArgs)
        {
            ((IXmlTransformationLogger)this).EndSection(MessageType.Normal, message, messageArgs);
        }

        /// <summary>
        /// The end section.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="messageArgs">
        /// The message args.
        /// </param>
        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            if (this.IndentLevel > 0)
                --this.IndentLevel;
            ((IXmlTransformationLogger)this).LogMessage(type, message, messageArgs);
        }
    }
}