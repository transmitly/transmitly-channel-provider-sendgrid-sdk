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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Email
{
    /// <summary>
    /// Converts Unix timestamp values to and from <see cref="DateTimeOffset"/>.
    /// </summary>
    public class UnixToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
	{
		/// <summary>
		/// Reads a Unix timestamp value from JSON.
		/// </summary>
		/// <param name="reader">The JSON reader containing the Unix timestamp.</param>
		/// <param name="typeToConvert">The target type being converted.</param>
		/// <param name="options">The serializer options.</param>
		/// <returns>The parsed <see cref="DateTimeOffset"/> value.</returns>
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			// Assuming the Unix time is in seconds
			long unixTimeSeconds = reader.GetInt64();
			return DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
		}

		/// <summary>
		/// Writes a <see cref="DateTimeOffset"/> value as a Unix timestamp.
		/// </summary>
		/// <param name="writer">The JSON writer to write to.</param>
		/// <param name="value">The value to serialize.</param>
		/// <param name="options">The serializer options.</param>
		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
			// Convert DateTimeOffset to Unix time (seconds) for serialization
			writer.WriteNumberValue(value.ToUnixTimeSeconds());
		}
	}
}
