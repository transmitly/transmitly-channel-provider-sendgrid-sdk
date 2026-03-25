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
using Transmitly.Channel.Configuration.Email;
using Transmitly.ChannelProvider.SendGrid.Configuration;
using Transmitly.Delivery;
using Transmitly.Util;

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Email
{
	/// <summary>
	/// Provides SendGrid-specific extended properties for an email delivery report.
	/// </summary>
	public sealed class ExtendedEmailDeliveryReportProperties : IEmailExtendedDeliveryReportProperties
	{
		private readonly IExtendedProperties _extendedProperties;
		private const string ProviderKey = SendGridConstant.EmailPropertiesKey;

		internal ExtendedEmailDeliveryReportProperties(DeliveryReport deliveryReport)
		{
			_extendedProperties = Guard.AgainstNull(deliveryReport).ExtendedProperties;
		}

		internal ExtendedEmailDeliveryReportProperties(IExtendedProperties properties)
		{
			_extendedProperties = Guard.AgainstNull(properties);
		}

		internal void Apply(SendGridEmailWebhookEvent report)
		{
			Email = report.Email;
			Domain = report.Domain;
			From = report.From;
			Timestamp = report.Timestamp;
			SmtpId = report.SmtpId;
			BounceClassification = report.BounceClassification;
			Event = report.Event;
			Categories = report.Categories;
			EventId = report.EventId;
			MessageId = report.MessageId;
			Reason = report.Reason;
			Status = report.Status;
			Type = report.Type;
			Response = report.Response;
			UserAgent = report.UserAgent;
			Url = report.Url;
			MachineOpen = report.MachineOpen;
			AsmGroupId = report.AsmGroupId;
			Ip = report.Ip;
			if (int.TryParse(report.Attempt, out var attemptInt))
				Attempt = attemptInt;
		}

		/// <summary>
		/// Creates an email channel property adapter for the specified email channel configuration.
		/// </summary>
		/// <param name="email">The email channel configuration to adapt.</param>
		/// <returns>The adapted extended channel properties.</returns>
		public IEmailExtendedChannelProperties Adapt(IEmailChannelConfiguration email)
		{
			return new ExtendedEmailChannelProperties(email);
		}

		/// <summary>
		/// Gets or sets the email address associated with the webhook event.
		/// </summary>
		public string? Email
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Email));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Email), value);
		}

		/// <summary>
		/// Gets or sets the sending domain associated with the webhook event.
		/// </summary>
		public string? Domain
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Domain));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Domain), value);
		}

		/// <summary>
		/// Gets or sets the sender address reported by SendGrid.
		/// </summary>
		public string? From
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(From));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(From), value);
		}

		/// <summary>
		/// Gets or sets the event timestamp reported by SendGrid.
		/// </summary>
		public DateTimeOffset? Timestamp
		{
			get => _extendedProperties.GetValue<DateTimeOffset?>(ProviderKey, nameof(Timestamp));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Timestamp), value);
		}

		/// <summary>
		/// Gets or sets the SMTP identifier associated with the message.
		/// </summary>
		public string? SmtpId
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(SmtpId));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(SmtpId), value);
		}

		/// <summary>
		/// Gets or sets the bounce classification reported by SendGrid.
		/// </summary>
		public string? BounceClassification
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(BounceClassification));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(BounceClassification), value);
		}

		/// <summary>
		/// Gets or sets the SendGrid event name.
		/// </summary>
		public string? Event
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Event));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Event), value);
		}

		/// <summary>
		/// Gets or sets the categories associated with the message.
		/// </summary>
		public IReadOnlyCollection<string>? Categories
		{
			get => _extendedProperties.GetValue<List<string>?>(ProviderKey, nameof(Categories));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Categories), value);
		}

		/// <summary>
		/// Gets or sets the SendGrid event identifier.
		/// </summary>
		public string? EventId
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(EventId));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(EventId), value);
		}

		/// <summary>
		/// Gets or sets the SendGrid message identifier.
		/// </summary>
		public string? MessageId
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(MessageId));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(MessageId), value);
		}

		/// <summary>
		/// Gets or sets the reason provided for the webhook event.
		/// </summary>
		public string? Reason
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Reason));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Reason), value);
		}

		/// <summary>
		/// Gets or sets the delivery status reported by SendGrid.
		/// </summary>
		public string? Status
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Status));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Status), value);
		}

		/// <summary>
		/// Gets or sets the type reported by SendGrid for the event.
		/// </summary>
		public string? Type
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Type));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Type), value);
		}

		/// <summary>
		/// Gets or sets the response text returned by the receiving server.
		/// </summary>
		public string? Response
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Response));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Response), value);
		}

		/// <summary>
		/// Gets or sets the user agent associated with the event.
		/// </summary>
		public string? UserAgent
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(UserAgent));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(UserAgent), value);
		}

		/// <summary>
		/// Gets or sets the URL associated with the event.
		/// </summary>
		public string? Url
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Url));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Url), value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the open was identified as machine generated.
		/// </summary>
		public bool? MachineOpen
		{
			get => _extendedProperties.GetValue<bool?>(ProviderKey, nameof(MachineOpen));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(MachineOpen), value);
		}

		/// <summary>
		/// Gets or sets the unsubscribe group identifier associated with the event.
		/// </summary>
		public int? AsmGroupId
		{
			get => _extendedProperties.GetValue<int?>(ProviderKey, nameof(AsmGroupId));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(AsmGroupId), value);
		}

		/// <summary>
		/// Gets or sets the delivery attempt count reported by SendGrid.
		/// </summary>
		public int? Attempt
		{
			get => _extendedProperties.GetValue<int?>(ProviderKey, nameof(AsmGroupId));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(AsmGroupId), value);
		}

		/// <summary>
		/// Gets or sets the IP address associated with the event.
		/// </summary>
		public string? Ip
		{
			get => _extendedProperties.GetValue<string?>(ProviderKey, nameof(Ip));
			set => _extendedProperties.AddOrUpdate(ProviderKey, nameof(Ip), value);
		}
	}
}
