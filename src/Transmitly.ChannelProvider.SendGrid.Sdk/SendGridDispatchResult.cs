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
using Transmitly.ChannelProvider.SendGrid.Configuration;

namespace Transmitly.ChannelProvider.SendGrid.Sdk
{
	/// <inheritdoc cref="IDispatchResult"/>
	sealed class SendGridDispatchResult() : IDispatchResult
	{
		public string? ResourceId { get; internal set; }

		public CommunicationsStatus Status { get; internal set; } = CommunicationsStatus.ClientError(SendGridConstant.Id, "Unknown");

		public string? ChannelProviderId { get; }

		public string? ChannelId { get; }

		public string? PipelineId { get; }

		public string? PipelineIntent { get; }

		public Exception? Exception { get; internal set; }
	}
}