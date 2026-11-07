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

using EllipticCurve;
using Transmitly.ChannelProvider.SendGrid.Configuration;
using Transmitly.ChannelProvider.SendGrid.Sdk.Email;
using Transmitly.Delivery;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Tests
{
	/// <summary>
	/// Signed Event Webhook checks. SendGrid signs timestamp + raw body with ECDSA P-256 and sends the
	/// base64 DER signature and the timestamp in headers; the dashboard shows the verification key as base64 DER.
	/// https://www.twilio.com/docs/sendgrid/for-developers/tracking-events/getting-started-event-webhook-security-features
	/// </summary>
	[TestClass]
	public sealed class DeliveryReportSignatureTests
	{
		private const string SignatureHeader = "X-Twilio-Email-Event-Webhook-Signature";
		private const string TimestampHeader = "X-Twilio-Email-Event-Webhook-Timestamp";
		private const string Timestamp = "1600112502";
		private const string Body = """[{"email":"alex@example.com","timestamp":1513299569,"event":"delivered","sg_event_id":"rWVYmVk90MjZJ9iohOBa3w==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","response":"250 OK"}]""";

		private static readonly PrivateKey SigningKey = new("prime256v1");
		private static readonly string VerificationKey = Convert.ToBase64String(SigningKey.publicKey().toDer());

		[TestMethod]
		public async Task SignedRequest_IsAccepted()
		{
			var reports = await Adaptor().AdaptAsync(Request(Body, Sign(Timestamp + Body), Timestamp));

			Assert.AreEqual(1, reports!.Count);
		}

		[TestMethod]
		public async Task TamperedBody_IsRejected()
		{
			var request = Request(Body.Replace("delivered", "bounce"), Sign(Timestamp + Body), Timestamp);

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Adaptor().AdaptAsync(request));
		}

		[TestMethod]
		public async Task ChangedTimestamp_IsRejected()
		{
			var request = Request(Body, Sign(Timestamp + Body), "1600112503");

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Adaptor().AdaptAsync(request));
		}

		[TestMethod]
		public async Task SignatureFromAnotherKey_IsRejected()
		{
			var otherKey = new PrivateKey("prime256v1");
			var request = Request(Body, Ecdsa.sign(Timestamp + Body, otherKey).toBase64(), Timestamp);

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Adaptor().AdaptAsync(request));
		}

		[TestMethod]
		[DataRow(true, false)]
		[DataRow(false, true)]
		[DataRow(false, false)]
		public async Task MissingSignatureHeaders_AreRejected(bool sendSignature, bool sendTimestamp)
		{
			var request = Request(Body, sendSignature ? Sign(Timestamp + Body) : null, sendTimestamp ? Timestamp : null);

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Adaptor().AdaptAsync(request));
		}

		[TestMethod]
		public async Task MalformedSignature_IsRejected()
		{
			var request = Request(Body, "not a signature", Timestamp);

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => Adaptor().AdaptAsync(request));
		}

		[TestMethod]
		public async Task MissingVerificationKey_IsRejected()
		{
			var adaptor = new SendGridChannelProviderDeliveryReportRequestAdaptor(new SendGridOptions { ValidateDeliveryReportSignature = true });

			await Assert.ThrowsExactlyAsync<DeliveryReportRequestValidationException>(() => adaptor.AdaptAsync(Request(Body, Sign(Timestamp + Body), Timestamp)));
		}

		[TestMethod]
		public async Task UnsignedRequest_IsAccepted_WhenValidationIsOff()
		{
			var adaptor = new SendGridChannelProviderDeliveryReportRequestAdaptor(new SendGridOptions { ValidateDeliveryReportSignature = false });

			var reports = await adaptor.AdaptAsync(new FakeRequest(Body));

			Assert.AreEqual(1, reports!.Count);
		}

		[TestMethod]
		public async Task RequestForAnotherProvider_IsIgnoredNotRejected()
		{
			// Another adaptor may own this request; it must come back as "not mine" (null), not as a failed signature.
			var reports = await Adaptor().AdaptAsync(new FakeRequest(Body, addressedToSendGrid: false));

			Assert.IsNull(reports);
		}

		private static SendGridChannelProviderDeliveryReportRequestAdaptor Adaptor() =>
			new(new SendGridOptions { ValidateDeliveryReportSignature = true, EventWebhookVerificationKey = VerificationKey });

		private static string Sign(string message) => Ecdsa.sign(message, SigningKey).toBase64();

		private static FakeRequest Request(string body, string? signature, string? timestamp)
		{
			var headers = new Dictionary<string, string?>();
			if (signature != null)
				headers[SignatureHeader] = signature;
			if (timestamp != null)
				headers[TimestampHeader] = timestamp;
			return new FakeRequest(body, headers: headers);
		}
	}
}
