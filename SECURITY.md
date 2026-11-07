# Security

## Deserialization caveat

TotalSerializer is designed to capture and replay the complete object state of .NET instances. To do that, serialized JSON contains type names, and deserialization resolves and instantiates the types named in the payload — including private fields, delegates and arbitrary object graphs.

This is the same class of risk as `BinaryFormatter` and Newtonsoft.Json's `TypeNameHandling.All`: **a crafted payload can instantiate arbitrary types.** Never feed TotalSerializer with JSON from an untrusted or unauthenticated source.

## Guidance

* Only deserialize JSON you produced yourself or received over an authenticated, integrity-protected channel.
* Do not use TotalSerializer for public APIs, HTTP endpoints, or any input a remote party can control — see the "Anti-use cases" in [README.md](README.md).
* Use `Config.IgnoreType` and recreate-type factories (`Config.RegisterRecreateTypeFactory`) to exclude types that must never be reconstructed from a payload.
* The serialized format binds to internal field names and assembly-qualified type names; it is not a stable interchange format.

## Reporting a vulnerability

Please report security issues privately via [GitHub Security Advisories](https://github.com/astropiano/totalserializer/security/advisories/new) instead of opening a public issue.
