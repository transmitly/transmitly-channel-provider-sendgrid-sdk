# Transmitly.ChannelProvider.SendGrid.Sdk

SendGrid SDK-based email dispatcher implementation for [Transmitly](https://github.com/transmitly/transmitly).

## Recommended Package

Most users should use the convenience package instead:

- [Transmitly.ChannelProvider.SendGrid](https://github.com/transmitly/transmitly-channel-provider-sendgrid)

That package registers this SDK implementation for you.

## What This Package Provides

- `EmailChannelProviderDispatcher` for the `Email` channel.
- `ExtendedEmailChannelProperties` for SendGrid-specific email settings.
- `SendGridChannelProviderDeliveryReportRequestAdaptor` for SendGrid event webhooks.
- Concrete delivery-report property types for SendGrid webhook payload data.

## Using This Package Directly (Advanced)

```csharp
using Transmitly;
using Transmitly.ChannelProvider.SendGrid.Configuration;
using Transmitly.ChannelProvider.SendGrid.Sdk.Email;

var builder = new CommunicationsClientBuilder();

var options = new SendGridOptions
{
	ApiKey = "your-sendgrid-api-key"
};

builder.ChannelProvider
	.Build(Id.ChannelProvider.SendGrid(), options)
	.AddDispatcher<EmailChannelProviderDispatcher, IEmail>(Id.Channel.Email())
	.AddEmailExtendedPropertiesAdaptor<ExtendedEmailChannelProperties>()
	.AddDeliveryReportRequestAdaptor<SendGridChannelProviderDeliveryReportRequestAdaptor>()
	.Register();
```

## Related Packages

- [Transmitly](https://github.com/transmitly/transmitly)
- [Transmitly.ChannelProvider.SendGrid](https://github.com/transmitly/transmitly-channel-provider-sendgrid)
- [Transmitly.ChannelProvider.SendGrid.Configuration](https://github.com/transmitly/transmitly-channel-provider-sendgrid-configuration)

---
_Copyright (c) Code Impressions, LLC. This open-source project is sponsored and maintained by Code Impressions and is licensed under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
