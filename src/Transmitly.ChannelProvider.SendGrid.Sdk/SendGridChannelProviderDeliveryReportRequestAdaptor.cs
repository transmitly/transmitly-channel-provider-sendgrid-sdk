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
using System.Threading.Tasks;
using Transmitly.Delivery;
using System.Linq;
using SendGrid.Helpers.EventWebhook;
using Transmitly.ChannelProvider.SendGrid.Configuration;
using Transmitly.Util;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Email
{
	/// <summary>
	/// Adapts SendGrid webhook payloads into Transmitly delivery reports.
	/// </summary>
	/// <param name="sendGridOptions">SendGrid options.</param>
	public sealed class SendGridChannelProviderDeliveryReportRequestAdaptor(SendGridOptions sendGridOptions) : IChannelProviderDeliveryReportRequestAdaptor
	{
		private readonly SendGridOptions _sendGridOptions = Guard.AgainstNull(sendGridOptions);

		/// <summary>
		/// Adapts the current request into a collection of delivery reports when the request matches the SendGrid webhook format.
		/// </summary>
		/// <param name="adaptorContext">The request adaptor context containing the webhook payload and query values.</param>
		/// <returns>A collection of delivery reports, or <see langword="null"/> when the request should not be adapted.</returns>
		/// <exception cref="DeliveryReportRequestValidationException">The request's signature is invalid.</exception>
		public Task<IReadOnlyCollection<DeliveryReport>?> AdaptAsync(IRequestAdaptorContext adaptorContext)
		{
			if (!ShouldAdapt(adaptorContext))
				return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(null);

			ValidateSignature(adaptorContext);

			var statuses = JsonSerializer.Deserialize<List<SendGridEmailWebhookEvent>>(adaptorContext.Content!);

			if (statuses == null)
				return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(null);
			statuses = [.. statuses.OrderByDescending(o=>o.Timestamp)];
			var ret = new List<DeliveryReport>(statuses.Count);
			foreach (var emailReport in statuses)
			{
				var report = new SendGridEmailDeliveryReport(
						DeliveryReport.Event.StatusChanged(),
						Id.Channel.Email(),
						Id.ChannelProvider.SendGrid(),
						//SG uses a static webhook URL without the ability to add context
						null,
						null,
						emailReport.MessageId,
						Util.ToDispatchStatus(emailReport.Event),
						null,
						null,
						null
					).ApplyExtendedProperties(emailReport);

				ret.Add(report);
			}

			return Task.FromResult<IReadOnlyCollection<DeliveryReport>?>(ret);
		}

		private static bool ShouldAdapt(IRequestAdaptorContext adaptorContext)
		{
			if (string.IsNullOrWhiteSpace(adaptorContext.Content))
				return false;
			return
				(adaptorContext.GetQueryValue(DeliveryUtil.ChannelIdKey)?.Equals(Id.Channel.Email(), StringComparison.InvariantCultureIgnoreCase) ?? false) &&
				(adaptorContext.GetQueryValue(DeliveryUtil.ChannelProviderIdKey)?.StartsWith(Id.ChannelProvider.SendGrid(), StringComparison.InvariantCultureIgnoreCase) ?? false);


		}

		// https://www.twilio.com/docs/sendgrid/for-developers/tracking-events/getting-started-event-webhook-security-features
		private void ValidateSignature(IRequestAdaptorContext adaptorContext)
		{
			if (!_sendGridOptions.ValidateDeliveryReportSignature)
				return;
			if (string.IsNullOrWhiteSpace(_sendGridOptions.EventWebhookVerificationKey))
				throw new DeliveryReportRequestValidationException("SendGrid EventWebhookVerificationKey is required to validate event webhooks.");

			var signature = adaptorContext.GetHeaderValue(RequestValidator.SIGNATURE_HEADER);
			var timestamp = adaptorContext.GetHeaderValue(RequestValidator.TIMESTAMP_HEADER);
			bool valid;
			try
			{
				var validator = new RequestValidator();
				valid = !string.IsNullOrEmpty(signature) && !string.IsNullOrEmpty(timestamp) &&
					validator.VerifySignature(validator.ConvertPublicKeyToECDSA(_sendGridOptions.EventWebhookVerificationKey), adaptorContext.Content, signature, timestamp);
			}
			catch (Exception)
			{
				// Malformed signature or key.
				valid = false;
			}
			if (!valid)
				throw new DeliveryReportRequestValidationException("SendGrid event webhook signature is invalid.");
		}
	}
}
