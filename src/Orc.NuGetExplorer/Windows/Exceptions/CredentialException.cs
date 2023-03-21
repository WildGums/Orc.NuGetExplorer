﻿namespace Orc.NuGetExplorer.Windows;

using System;

/// <summary>
/// The exception that is thrown when an error occurs getting credentials.
/// </summary>
/// <threadsafety instance="false" static="true" />
[Serializable()]
public class CredentialException : System.ComponentModel.Win32Exception
{
    private const string CredentialError = "An error occurred acquiring credentials.";

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialException" /> class.
    /// </summary>
    public CredentialException()
        : base(CredentialError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialException" /> class with the specified error. 
    /// </summary>
    /// <param name="error">The Win32 error code associated with this exception.</param>
    public CredentialException(int error)
        : base(error)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CredentialException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialException" /> class with the specified error and the specified detailed description.
    /// </summary>
    /// <param name="error">The Win32 error code associated with this exception.</param>
    /// <param name="message">A detailed description of the error.</param>
    public CredentialException(int error, string message)
        : base(error, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">A reference to the inner exception that is the cause of the current exception.</param>
    public CredentialException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialException" /> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected CredentialException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
}