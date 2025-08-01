﻿// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
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
		public SendGridSdkDispatcherException()
		{
		}

		public SendGridSdkDispatcherException(string message) : base(message)
		{
		}

		public SendGridSdkDispatcherException(string message, Exception innerException) : base(message, innerException)
		{
		}

#if NET8_0_OR_GREATER
		[Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
		protected SendGridSdkDispatcherException(SerializationInfo exception, StreamingContext context) : base(exception, context)
		{

		}
	}
}