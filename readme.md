# Amplitude Experiment SDK for .NET

[![NuGet version (Amplitude.Experiment)](https://img.shields.io/nuget/v/Amplitude.Experiment.svg)](https://www.nuget.org/packages/Amplitude.Experiment)
![Nuget](https://img.shields.io/nuget/dt/Amplitude.Experiment)
![GitHub issues](https://img.shields.io/github/issues/gmreburn/AmplitudeExperimentSDK)
![GitHub last commit](https://img.shields.io/github/last-commit/gmreburn/AmplitudeExperimentSDK)

Streamline your A/B testing with this .NET SDK for Amplitude Experiment that offers seamless integration of Amplitude's
Experimentation functionality with your .NET applications. With a set of easy-to-use classes, you can quickly run A/B
tests and identify the most effective changes in your software. The SDK is built using the official Amplitude Experiment
REST API to provide a reliable and efficient solution for .NET developers. Please note that the .NET bindings are not
maintained by Amplitude; however, it provides an alternative to the REST API for those who work with the .NET framework.

## Install

Install the .NET SDK for Amplitude Experiment via NuGet by running the following command in the Package Manager Console:

```nuget
Install-Package Amplitude.Experiment
```

## Usage

To use the .NET Amplitude Experiment SDK, it is necessary to have a [deployment](https://www.docs.developers.amplitude.com/experiment/general/data-model/#deployments). A deployment is a group of flags or experiments that are used in an application. Each deployment is identified by a randomly generated deployment key that is used by the Experiment to authorize requests to the evaluation servers. If you do not have a deployment, you must create one before using the SDK. You can [create a deployment](https://www.docs.developers.amplitude.com/experiment/guides/getting-started/create-a-deployment/) by following the instructions provided in the Amplitude Experiment documentation on creating a deployment.

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
var variants = await client.VariantAsync("<your-flag-key>");

if(variants.First().Value == "on") {
    // the flag is enabled
} else {
    // the flag is not enabled
}
```

It should be noted that passing the flag key is optional. If no flag key is passed, all flags will be returned by the SDK. This can be useful if you want to cache the state of all flags in your application.

## Usage with additional context

Set context with ExperimentUser when you initialize the SDK, it will be used for every request to retreive variants. The ExperimentUser can impact the value of the variant.

### ExperimentUser

Experiment users map to a user within Amplitude Analytics. Alongside flag configurations, users are an input to [evaluation](https://www.docs.developers.amplitude.com/experiment/general/evaluation/implementation/). Flag and experiment targeting rules can make use of user properties.

Context is an optional JSON of custom properties used during evaluation.

```c#
var user = new ExperimentUser(){ UserId = "test", DeviceId = "UserAgent 1", context = @"{"plan":"premium"}" }
```

You can pass a user when you initialize the SDK or when you retreive variants.

```c#
var client = new ExperimentClient("<your-api-key>", user);
```

You can also supply ExperimentUser during the call to VariantAsync to override the ExperimentUser supplied during initialization.

```c#
var variants = await client.VariantAsync("<your-flag-key>", user);
```
