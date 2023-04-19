# Unofficial Amplitude Experiment SDK for .NET

Streamline your A/B testing with the unofficial .NET SDK for Amplitude Experiment. This Software Development Kit (SDK) offers seamless
integration of Amplitude's Experimentation functionality with your .NET applications. With a set of easy-to-use classes, you can quickly
run A/B tests and identify the most effective changes in your software. The SDK is built using the official Amplitude Experiment REST API
to provide a reliable and efficient solution for .NET developers. Please note that this is an unofficial SDK and is not officially
supported by Amplitude; however, it provides a robust alternative for those who work with the .NET framework.

## Install

Install the .NET SDK for Amplitude Experiment via NuGet by running the following command in the Package Manager Console:

```nuget
Install-Package Amplitude.Experiment
```

## Usage

Once installed, import the `Amplitude.Experiment` namespace into your project.

```c#
using Amplitude.Experiment;
```

Create an instance of the `ExperimentClient` class, passing in your Amplitude Deployment API key:

```c#
var client = new ExperimentClient("<your-api-key>");
```

Use the client object to get variants:

```c#
var client = new ExperimentClient("<your-api-key>");
var variants = await client.VariantAsync("<your-flag-key>");

if(variants.First().Value == "on") {
    // the flag is enabled
} else {
    // the flag is not enabled
}
```

## Usage with additional context

Set context with ExperimentUser when you initialize the SDK, it will be used for every request to retreive variants. The ExperimentUser can impact the value of the variant.

### ExperimentUser

Experiment users map to a user within Amplitude Analytics. Alongside flag configurations, users are an input to [evaluation](https://www.docs.developers.amplitude.com/experiment/general/evaluation/implementation/). Flag and experiment targeting rules can make use of user properties.

Context is an optional JSON of custom properties used when evaluating the user during evaluation.

```c#
var user = new ExperimentUser(){ UserId = "test", DeviceId = "UserAgent 1", context = @"{"plan":"premium"}" }
```

You can pass the user when you initialize the SDK or when you retreive variants.

```c#
var client = new ExperimentClient("<your-api-key>", user);
```

You can also supply ExperimentUser during the call to VariantAsync to override the ExperimentUser supplied during initialization.

```c#
var variants = await client.VariantAsync("<your-flag-key>", user);
```
