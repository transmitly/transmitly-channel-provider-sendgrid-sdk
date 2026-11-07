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
	/// <summary>
	/// Reads a JSON value that is either a single string or an array of strings.
	/// </summary>
	internal sealed class StringOrArrayConverter : JsonConverter<List<string>?>
	{
		public override List<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.String)
				return [reader.GetString()!];
			return JsonSerializer.Deserialize<List<string>>(ref reader, options);
		}

		public override void Write(Utf8JsonWriter writer, List<string>? value, JsonSerializerOptions options) =>
			JsonSerializer.Serialize(writer, value, options);
	}
}
