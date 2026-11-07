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

using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Tests
{
	/// <summary>
	/// A webhook request as the delivery report endpoint hands it to the adaptor.
	/// </summary>
	internal sealed class FakeRequest(string body, bool addressedToSendGrid = true, IReadOnlyDictionary<string, string?>? headers = null) : IRequestAdaptorContext
	{
		public string? GetQueryValue(string key)
		{
			if (!addressedToSendGrid)
				return null;
			if (key == DeliveryUtil.ChannelIdKey)
				return Id.Channel.Email();
			if (key == DeliveryUtil.ChannelProviderIdKey)
				return Id.ChannelProvider.SendGrid();
			return null;
		}

		public string? GetFormValue(string key) => null;

		public string? GetHeaderValue(string key) => headers != null && headers.TryGetValue(key, out var value) ? value : null;

		string? IRequestAdaptorContext.GetValue(string key) => throw new System.NotSupportedException("The ASP.NET hosts throw here; use GetQueryValue, GetFormValue or GetHeaderValue.");

		public string? Content => body;

		public string? RequestUrl => null;

		public string? PipelineIntent => null;

		public string? PipelineId => null;

		public string? ResourceId => null;
	}
}
