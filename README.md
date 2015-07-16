# Feature Toggle

A C# library for implementing feature toggles.


## Usage

To add a feature toggle use the `IsFeatureOn` method as shown below:

```csharp
IToggle toggle = new Toggle(new Hasher());
var config = "On";
var identifier = "test-user";

if (toggle.IsFeatureOn(config, identifier))
{
    ShowToggledFeature();
}
```

You must always pass a `config` and an `identifier` into the feature toggle.

### Config

The `config` value is a string which specifies how the feature toggle should behave.
There are several different values possible:

* `off` - The feature code will not be executed.
* `on` - The feature code will always be executed.
* `start=<ISO8601-formatted timestamp>` - The feature code will only be executed
    if the current time (UTC) is greater than or equal to the specified time.
* `end=<ISO8601-formatted timestamp>` - The feature code will only be executed
    if the current time (UTC) is less than the specified time.
* `between=<ISO8601-formatted start timestamp>,<ISO8601-formatted end timestamp>`
    - The feature code will only be executed if the current time (UTC) is between
    the two specified dates.
* `percent=<number in [0, 1]>` - The feature code will only be executed for the
    specified percentage of users based on the `identifier` (see below).
* `percent=<number in [0, 1]>,group=<int>` - The feature code will only be
    executed for the specified percentage of users based on the `identifier`
    and the group value.

Examples:
```
"on"
"off"
"start=2015-07-16T01:40:00Z"
"end=2015-07-16T01:40:00Z"
"between=2015-07-16T01:40:00Z,2015-08-16T02:30:00Z"
"percent=.25"
"percent=.25,group=2"
```

### Identifier

The `identifier` value is a string that represents the current user.
It can be any kind of id value, an ip address, or something else.
It is only used in conjunction with the `percent` config option,
allowing only a percentage of users to access the feature.

When a `percent` config is set, the `identifier` is hashed and the result
is used to determine whether the user falls within the given percentage.
The optional `group=<int>` part of the config will switch which byte of the
hash is used for the comparison; this allows you to change groups so that
the same users aren't always falling in the same percent ranges.

This means that if you set your `percent=.5` any specific user will always
get the same result instead of seeing the feature only 50% of the time.
(It's 50% of the user base, not 50% chance!)

Example calculation:

```
Let the hash for the identifier "test-user" start with "8005AE".
If the percent is set to .5 and the group is not set (default to 0), then
    hash[0] is 0x80,
    0x80 / 0xFF is 0.50196,
    0.50196 is greater than .5 so the user does not see the feature.

If the percent is set to .5 and the group is set to 1, then
    hash[1] is 0x05,
    0x05 / 0xFF is 0.01960,
    0.01960 is less than .5 so the user does see the feature.
```
