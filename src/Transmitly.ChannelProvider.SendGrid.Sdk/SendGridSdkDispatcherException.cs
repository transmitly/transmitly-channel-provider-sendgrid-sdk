// Copyright (c) Code Impressions, LLC. All Rights Reserved.
//  
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Runtime.Serialization;

namespace Transmitly.ChannelProvider.SendGrid.Sdk
{
	///<inheritdoc cref="Exception"/>
	[Serializable]
	public class SendGridSdkDispatcherException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SendGridSdkDispatcherException"/> class.
		/// </summary>
		public SendGridSdkDispatcherException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SendGridSdkDispatcherException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public SendGridSdkDispatcherException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SendGridSdkDispatcherException"/> class with a specified error message and a reference to the inner exception.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that caused the current exception.</param>
		public SendGridSdkDispatcherException(string message, Exception innerException) : base(message, innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridSdkDispatcherException"/> class with serialized data.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="context"></param>
#if NET8_0_OR_GREATER
		[Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
        protected SendGridSdkDispatcherException(SerializationInfo exception, StreamingContext context) : base(exception, context)
		{

		}
	}
}
