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
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Email
{
	sealed class SendGridEmailWebhookEvent
	{
		[JsonPropertyName("email")]
		public string? Email { get; set; }

		[JsonPropertyName("domain")]
		public string? Domain { get; set; }

		[JsonPropertyName("from")]
		public string? From { get; set; }

		// SendGrid's docs show both "5" and 5.
		[JsonPropertyName("attempt")]
		[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
		public int? Attempt { get; set; }

		[JsonPropertyName("timestamp")]
		[JsonConverter(typeof(UnixToDateTimeOffsetConverter))]
		public DateTimeOffset? Timestamp { get; set; }

		[JsonPropertyName("smtp-id")]
		public string? SmtpId { get; set; }

		[JsonPropertyName("bounce_classification")]
		public string? BounceClassification { get; set; }

		[JsonPropertyName("event")]
		public string? Event { get; set; }

		// A string when the message was sent with a single category, otherwise an array.
		[JsonPropertyName("category")]
		[JsonConverter(typeof(StringOrArrayConverter))]
		public List<string>? Categories { get; set; }

		[JsonPropertyName("sg_event_id")]
		public string? EventId { get; set; }

		[JsonPropertyName("sg_message_id")]
		public string? MessageId { get; set; }

		[JsonPropertyName("reason")]
		public string? Reason { get; set; }

		[JsonPropertyName("status")]
		public string? Status { get; set; }

		[JsonPropertyName("type")]
		public string? Type { get; set; }

		[JsonPropertyName("ip")]
		public string? Ip { get; set; }

		[JsonPropertyName("response")]
		public string? Response { get; set; }

		[JsonPropertyName("useragent")]
		public string? UserAgent { get; set; }

		[JsonPropertyName("url")]
		public string? Url { get; set; }

		[JsonPropertyName("sg_machine_open")]
		public bool? MachineOpen { get; set; }

		[JsonPropertyName("asm_group_id")]
		public int? AsmGroupId { get; set; }
	}
}