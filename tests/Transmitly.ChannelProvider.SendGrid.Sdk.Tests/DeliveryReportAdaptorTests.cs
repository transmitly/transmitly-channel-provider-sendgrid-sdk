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

using Transmitly.ChannelProvider.SendGrid.Configuration;
using Transmitly.ChannelProvider.SendGrid.Sdk.Email;
using Transmitly.Delivery;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Tests
{
	/// <summary>
	/// Runs SendGrid's documented Event Webhook payloads through the delivery report adaptor.
	/// </summary>
	[TestClass]
	public sealed class DeliveryReportAdaptorTests
	{
		// Copied from https://www.twilio.com/docs/sendgrid/for-developers/tracking-events/event (2026-11-06); user agents shortened.
		private static readonly Dictionary<string, string> Examples = new()
		{
			["processed"] = """[{"email":"alex@example.com","timestamp":1513299569,"pool":{"name":"new_MY_test","id":210},"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"processed","category":"cat facts","sg_event_id":"rbtnWrG1DVDGGGFHFyun0A==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.000000000000000000000"}]""",
			["deferred"] = """[{"email":"alex@example.com","domain":"example.com","from":"test@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"deferred","category":"cat facts","sg_event_id":"t7LEShmowp86DTdUW8M-GQ==","sg_message_id":" 14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","response":"400 try again later","attempt":"5"}]""",
			["delivered"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"delivered","category":"cat facts","sg_event_id":"rWVYmVk90MjZJ9iohOBa3w==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","response":"250 OK"}]""",
			["bounce"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","bounce_classification":"Invalid Address","event":"bounce","category":"cat facts","sg_event_id":"6g4ZI7SA-xmRDv57GoPIPw==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","reason":"500 unknown recipient","status":"5.0.0","type":"bounce"}]""",
			["blocked"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","bounce_classification":"Invalid Address","event":"bounce","category":"cat facts","sg_event_id":"6g4ZI7SA-xmRDv57GoPIPw==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","reason":"500 unknown recipient","status":"5.0.0","type":"blocked"}]""",
			["dropped"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"dropped","category":"cat facts","sg_event_id":"zmzJhfJgAfUSOW80yEbPyw==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","reason":"Bounced Address","status":"5.0.0"}]""",
			["open"] = """[{"email":"alex@example.com","timestamp":1513299569,"event":"open","sg_machine_open":false,"category":"cat facts","sg_event_id":"FOTFFO0ecsBE-zxFXfs6WA==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","useragent":"Mozilla/4.0","ip":"255.255.255.255"}]""",
			["click"] = """[{"sg_event_id":"sendgrid_internal_event_id","sg_message_id":"sendgrid_internal_message_id","ip":"255.255.255.255","useragent":"Mozilla/5.0","event":"click","email":"email@example.com","timestamp":1249948800,"url":"http://example.com/blog/news.html","url_offset":{"index":0,"type":"html"},"unique_arg_key":"unique_arg_value","category":["category1","category2"],"newsletter":{"newsletter_user_list_id":"10557865","newsletter_id":"1943530","newsletter_send_id":"2308608"},"asm_group_id":1}]""",
			["spamreport"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"spamreport","sg_event_id":"37nvH5QBz858KGVYCM4uOA==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"}]""",
			["unsubscribe"] = """[{"email":"alex@example.com","timestamp":1513299569,"event":"unsubscribe","category":"cat facts","sg_event_id":"zz_BjPgU_5pS-J8vlfB1sg==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0"}]""",
			["group_unsubscribe"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"group_unsubscribe","category":"cat facts","sg_event_id":"ahSCB7xYcXFb-hEaawsPRw==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","useragent":"Mozilla/4.0","ip":"255.255.255.255","url":"http://www.example.com/","asm_group_id":10}]""",
			["group_resubscribe"] = """[{"email":"alex@example.com","timestamp":1513299569,"smtp-id":"<14c5d75ce93.dfd.64b469@ismtpd-555>","event":"group_resubscribe","category":"cat facts","sg_event_id":"w_u0vJhLT-OFfprar5N93g==","sg_message_id":"14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0","useragent":"Mozilla/4.0","ip":"255.255.255.255","url":"http://www.example.com/","asm_group_id":10}]""",
			["account_status_change"] = """[{"event":"account_status_change","sg_event_id":"MjEzNTg5OTcyOC10ZXJtaW5hdGUtMTcwNzg1MTUzMQ","timestamp":1709142428,"type":"compliance_suspend"}]""",
		};

		[TestMethod]
		[DataRow("delivered", "success")]
		[DataRow("processed", "info")]
		[DataRow("deferred", "info")]
		[DataRow("bounce", "failure")]
		[DataRow("blocked", "failure")]
		[DataRow("dropped", "failure")]
		[DataRow("open", "info")]
		[DataRow("click", "info")]
		[DataRow("spamreport", "info")]
		[DataRow("unsubscribe", "info")]
		[DataRow("group_unsubscribe", "info")]
		[DataRow("group_resubscribe", "info")]
		[DataRow("account_status_change", "info")]
		public async Task DocumentedEvent_MapsToExpectedStatus(string example, string expected)
		{
			var report = await AdaptSingleAsync(Examples[example]);

			Assert.AreEqual(expected, KindOf(report.Status), report.Status.ToString());
		}

		[TestMethod]
		public async Task UnknownEvent_IsReportedAsFailure()
		{
			var report = await AdaptSingleAsync("""[{"event":"brand_new_event","sg_event_id":"x","timestamp":1513299569}]""");

			Assert.AreEqual("failure", KindOf(report.Status));
		}

		[TestMethod]
		public async Task Bounce_ExposesEventDetailsOnTheReport()
		{
			var report = await AdaptSingleAsync(Examples["bounce"]);
			var email = Read(report);

			Assert.AreEqual("bounce", email.Event);
			Assert.AreEqual("500 unknown recipient", email.Reason);
			Assert.AreEqual("5.0.0", email.Status);
			Assert.AreEqual("bounce", email.Type);
			Assert.AreEqual("Invalid Address", email.BounceClassification);
			Assert.AreEqual("alex@example.com", email.Email);
			Assert.AreEqual("6g4ZI7SA-xmRDv57GoPIPw==", email.EventId);
			Assert.AreEqual("14c5d75ce93.dfd.64b469.filter0001.16648.5515E0B88.0", email.MessageId);
			Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(1513299569), email.Timestamp);
			CollectionAssert.AreEqual(new[] { "cat facts" }, email.Categories!.ToArray());
		}

		[TestMethod]
		public async Task Click_ExposesArrayCategoriesAndGroupId()
		{
			var email = Read(await AdaptSingleAsync(Examples["click"]));

			CollectionAssert.AreEqual(new[] { "category1", "category2" }, email.Categories!.ToArray());
			Assert.AreEqual(1, email.AsmGroupId);
			Assert.IsNull(email.Attempt);
			Assert.AreEqual("http://example.com/blog/news.html", email.Url);
		}

		[TestMethod]
		// The docs' example sends a string; their property table says integer.
		[DataRow("\"5\"")]
		[DataRow("5")]
		public async Task Deferred_ExposesAttempt(string attempt)
		{
			var email = Read(await AdaptSingleAsync(Examples["deferred"].Replace("\"attempt\":\"5\"", $"\"attempt\":{attempt}")));

			Assert.AreEqual(5, email.Attempt);
			Assert.IsNull(email.AsmGroupId);
		}

		[TestMethod]
		// The docs' examples send a number; their property table says string.
		[DataRow("1513299569")]
		[DataRow("\"1513299569\"")]
		public async Task Timestamp_IsReadAsUnixSeconds(string timestamp)
		{
			var email = Read(await AdaptSingleAsync(Examples["delivered"].Replace("1513299569", timestamp)));

			Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(1513299569), email.Timestamp);
		}

		[TestMethod]
		public async Task Batch_ReturnsOneReportPerEvent()
		{
			var batch = "[" + string.Join(",", Examples.Values.Select(e => e.Substring(1, e.Length - 2))) + "]";

			var reports = await AdaptAsync(batch);

			Assert.AreEqual(Examples.Count, reports!.Count);
		}

		[TestMethod]
		public async Task RequestNotAddressedToSendGrid_IsNotAdapted()
		{
			var reports = await Adaptor().AdaptAsync(new FakeRequest(Examples["delivered"], addressedToSendGrid: false));

			Assert.IsNull(reports);
		}

		private static SendGridChannelProviderDeliveryReportRequestAdaptor Adaptor() =>
			new(new SendGridOptions { ValidateDeliveryReportSignature = false });

		private static Task<IReadOnlyCollection<DeliveryReport>?> AdaptAsync(string body) =>
			Adaptor().AdaptAsync(new FakeRequest(body, addressedToSendGrid: true));

		private static async Task<DeliveryReport> AdaptSingleAsync(string body)
		{
			var reports = await AdaptAsync(body);
			Assert.IsNotNull(reports);
			Assert.AreEqual(1, reports.Count);
			return reports.Single();
		}

		// Nothing is registered in these tests: the values must still be readable.
		private static IEmailExtendedDeliveryReportProperties Read(DeliveryReport report) => report.SendGrid().Email;

		private static string KindOf(CommunicationsStatus status) =>
			status.IsFailure() ? "failure" : status.Code >= CommunicationsStatus.SuccessMin ? "success" : "info";
	}
}
