# Amplitude SDK for C#

## Install
Install NuGet package

## Usage
```c#
public async Task DoThing()
{
     var client = new ExperimentClient("INSERT_YOUR_AMPLITUDE_DEPLOYMENT_KEY");
     var variants = await client.VariantAsync("INSERT_YOUR_FEATURE_FLAG_KEY");

     if(variants.First().Value === "on") {
         // the flag is enabled
     } else {
         // the flag is not enabled
     }
}
```

## Usage with extra context
Set context with ExperimentUser when you initialize the SDK, it will be used for every request to retreive variants.

```c#
public async Task DoThing()
{
     var client = new ExperimentClient("INSERT_YOUR_AMPLITUDE_DEPLOYMENT_KEY", new ExperimentUser() { UserId = "test2" });
     var variants = await client.VariantAsync("INSERT_YOUR_FEATURE_FLAG_KEY");

     if(variants.First().Value === "on") {
         // the flag is enabled
     } else {
         // the flag is not enabled
     }
}
```
You can also supply ExperimentUser during the call to VariantAsync to override the ExperimentUser supplied during initialization.
```c#
public async Task DoThing()
{
     var client = new ExperimentClient("INSERT_YOUR_AMPLITUDE_DEPLOYMENT_KEY");
     var variants = await client.VariantAsync("INSERT_YOUR_FEATURE_FLAG_KEY", new ExperimentUser() { UserId = "test2" });

     if(variants.First().Value === "on") {
         // the flag is enabled
     } else {
         // the flag is not enabled
     }
}
```