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

namespace Transmitly.ChannelProvider.SendGrid.Sdk.Email
{
	static class ExtendedEmailDeliveryReportExtension
	{
		public static SendGridEmailDeliveryReport ApplyExtendedProperties(this SendGridEmailDeliveryReport emailDeliveryReport, SendGridEmailWebhookEvent report)
		{
			var email = emailDeliveryReport.SendGrid().Email;
			email.Email = report.Email;
			email.Domain = report.Domain;
			email.From = report.From;
			email.Timestamp = report.Timestamp;
			email.SmtpId = report.SmtpId;
			email.BounceClassification = report.BounceClassification;
			email.Event = report.Event;
			email.Categories = report.Categories;
			email.EventId = report.EventId;
			email.MessageId = report.MessageId;
			email.Reason = report.Reason;
			email.Status = report.Status;
			email.Type = report.Type;
			email.Response = report.Response;
			email.UserAgent = report.UserAgent;
			email.Url = report.Url;
			email.MachineOpen = report.MachineOpen;
			email.AsmGroupId = report.AsmGroupId;
			email.Ip = report.Ip;
			email.Attempt = report.Attempt;
			return emailDeliveryReport;
		}
	}
}