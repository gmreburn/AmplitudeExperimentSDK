# Unofficial Amplitude Experiment SDK for .NET

The unofficial .NET SDK for Amplitude Experiment is a Software Development Kit (SDK) developed to integrate Amplitude's Experimentation
functionality with .NET applications. The SDK provides a set of classes that allow you to easily run A/B tests. With
this SDK, you can quickly integrate Amplitude Experimentation into your applications and test different variations
of your software to identify the most effective changes. The unofficial .NET SDK for Amplitude Experiment is not officially supported by
Amplitude, but it offers an alternative solution for those who work with the .NET framework.

## Install

Install the unofficial .NET SDK for Amplitude Experiment via NuGet by running the following command in the Package Manager Console:

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
