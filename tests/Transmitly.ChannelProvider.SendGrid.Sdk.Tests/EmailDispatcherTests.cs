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

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Moq;
using Transmitly.Channel.Configuration.Email;
using Transmitly.ChannelProvider.SendGrid.Configuration;
using Transmitly.ChannelProvider.SendGrid.Sdk.Email;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Tests
{
	/// <summary>
	/// Checks the v3 mail/send body the dispatcher hands to SendGrid.
	/// </summary>
	[TestClass]
	public sealed class EmailDispatcherTests
	{
		private static readonly byte[] Pdf = Encoding.UTF8.GetBytes("%PDF-1.4 statement");
		private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

		[TestMethod]
		[DataRow(false)]
		[DataRow(true)]
		public async Task AttachmentAndInlineImage_AreSent(bool useTemplate)
		{
			var email = Email(useTemplate, Attachment("statement.pdf", "application/pdf", Pdf));
			var context = Context(new LinkedResource("cid:logo", new MemoryStream(Png)) { ContentType = "image/png" });

			// Sent twice: each recipient's email reuses the same resource streams.
			await SendAsync(email, context);
			var body = await SendAsync(email, context);

			var attachments = body.GetProperty("attachments").EnumerateArray().ToArray();
			Assert.AreEqual(2, attachments.Length);

			Assert.AreEqual("statement.pdf", attachments[0].GetProperty("filename").GetString());
			Assert.AreEqual("application/pdf", attachments[0].GetProperty("type").GetString());
			Assert.AreEqual(Convert.ToBase64String(Pdf), attachments[0].GetProperty("content").GetString());
			Assert.IsFalse(attachments[0].TryGetProperty("content_id", out _));

			// HTML refers to it as cid:logo, so SendGrid needs content_id "logo" and an inline disposition.
			Assert.AreEqual("logo", attachments[1].GetProperty("filename").GetString());
			Assert.AreEqual("image/png", attachments[1].GetProperty("type").GetString());
			Assert.AreEqual(Convert.ToBase64String(Png), attachments[1].GetProperty("content").GetString());
			Assert.AreEqual("inline", attachments[1].GetProperty("disposition").GetString());
			Assert.AreEqual("logo", attachments[1].GetProperty("content_id").GetString());

			if (useTemplate)
				Assert.AreEqual("d-123", body.GetProperty("template_id").GetString());
		}

		[TestMethod]
		public async Task EmailWithoutAttachments_SendsNoAttachmentsField()
		{
			var body = await SendAsync(Email(useTemplate: false), Context());

			Assert.IsFalse(body.TryGetProperty("attachments", out _));
		}

		private static async Task<JsonElement> SendAsync(IEmail email, IDispatchCommunicationContext context)
		{
			var handler = new CapturingHandler();
			var dispatcher = new EmailChannelProviderDispatcher(new SendGridOptions { ApiKey = "SG.test" }, new HttpClient(handler));

			await dispatcher.DispatchAsync(email, context, CancellationToken.None);

			return JsonDocument.Parse(handler.Body!).RootElement;
		}

		private static IEmail Email(bool useTemplate, params IEmailAttachment[] attachments)
		{
			// The pipeline sets this on the email channel; core hands the channel's properties to the email it dispatches.
			var channel = Mock.Of<IEmailChannelConfiguration>(c => c.ExtendedProperties == new ExtendedProperties());
			if (useTemplate)
				channel.SendGrid().TemplateId = "d-123";

			var email = new Mock<IEmail>();
			email.SetupGet(e => e.From).Returns("sender@example.com".AsIdentityAddress());
			email.SetupGet(e => e.To).Returns(["rider@example.com".AsIdentityAddress()]);
			email.SetupGet(e => e.Subject).Returns("Your statement");
			email.SetupGet(e => e.HtmlBody).Returns("<img src=\"cid:logo\"> Your statement is attached.");
			email.SetupGet(e => e.TextBody).Returns("Your statement is attached.");
			email.SetupGet(e => e.Attachments).Returns(attachments);
			email.SetupGet(e => e.ExtendedProperties).Returns(channel.ExtendedProperties);
			return email.Object;
		}

		private static IEmailAttachment Attachment(string name, string contentType, byte[] content) =>
			Mock.Of<IEmailAttachment>(a => a.Name == name && a.ContentType == contentType && a.ContentStream == new MemoryStream(content));

		private static IDispatchCommunicationContext Context(params LinkedResource[] linkedResources)
		{
			var context = new Mock<IDispatchCommunicationContext> { DefaultValue = DefaultValue.Mock };
			context.SetupGet(c => c.ContentModel).Returns(Mock.Of<IContentModel>(m => m.LinkedResources == linkedResources && m.Model == new { Name = "Ada" }));
			return context.Object;
		}

		private sealed class CapturingHandler : HttpMessageHandler
		{
			public string? Body { get; private set; }

			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				Body = await request.Content!.ReadAsStringAsync();
				var response = new HttpResponseMessage(HttpStatusCode.Accepted) { Content = new StringContent(string.Empty) };
				response.Headers.Add("X-Message-Id", "test-message-id");
				return response;
			}
		}
	}
}
