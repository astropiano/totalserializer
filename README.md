# TotalSerializer

![TotalSerializer logo](branding/totalserializer_incircle_small.png)

JSON serializer for .NET which focuses on capturing and replaying object
states. The library serializes all fields of a class (public and private) as
well as event registration and referenced delegates and is able to
deserialize the object such that all event subscriptions and referenced
delegates are still there and fully functional.

> **Warning:** This project is still in alpha state.

TotalSerializer is built on top of the companion `Astropiano.ReflectionTools`
library, which can also be used independently.

## What is this library about

This library is about serializing the deep object state of .NET instances to
JSON. With this the library is able to create deep copies of instances or
replicate the same instance on another machine.

### Use cases

Cases when you should consider using TotalSerializer:

* Transfer the state of .NET objects from one machine to another.
* Creating deep copies of instances.

### Anti-use cases

Cases when you should NOT consider using TotalSerializer:

* You would like to serialize data for an HTML client. Please use a library
  like Newtonsoft.Json. Even if you would be able to use TotalSerializer for
  such a case it is not recommended to do so.
* You would like to deserialize JSON written by an older version of a class
  into a newer version. If fields have been renamed or changed,
  TotalSerializer will not work.

## Documentation

### Basic usage

```csharp
TotalSerializer serializer = new TotalSerializer();
MyObject instance = new MyObject();
string json = serializer.Serialize(instance);
MyObject instanceDeserialized = serializer.Deserialize<MyObject>(json);
```

### Creating copies

```csharp
TotalSerializer serializer = new TotalSerializer();
MyObject instance = new MyObject();
MyObject copyOfInstance = serializer.Copy(instance);
```

### Configuration

It is possible to pass a configuration object to the TotalSerializer instance
with the constructor.

```csharp
Config configuration = new Config();
TotalSerializer serializer = new TotalSerializer(configuration);
```

To reset the configuration to default you can call `SetDefaults()`.

### Loggers

The logger of TotalSerializer can be integrated into the application so that
log messages of TotalSerializer appear in the application log file. To
configure the logger you have to use the static field
`TotalSerializer.LoggerProvider`. You can set the value to a custom
implementation deriving from `TotalSerializerLoggerProvider`.

### Ignored types

You can mark types that should be ignored completely and that should not
appear in the serialized JSON.

```csharp
Config configuration = new Config();
configuration.IgnoreType(typeof(MyType)); // pass by argument
configuration.IgnoreType<MyTypeToo>(); // generic overload
TotalSerializer serializer = new TotalSerializer(configuration);
```

Please mind that the actual type of the field value will be checked and not
the type of the field itself.

### Custom serialization

It is possible to define custom serialization handlers, a so called
total-special-serializer, to control how a specific type is serialized and
deserialized. To do so you need to create a custom class deriving from
`TotalSpecialSerializer<T>`. The class is generic and `T` is a
replacement-instance that is serialized instead of the original object. This
replacement-instance is eventually used. A total-special serializer can
handle many types at once.

A good example to refer to is the `WeakReferenceSerializer` used by
TotalSerializer itself.

In simple cases where you may want to only handle a single type you can use
the simpler `TypedTotalSpecialSerializer<THandled, TReplacement>`.

In order to use these total-special-serializers they need to be registered:

```csharp
Config configuration = new Config();
MyCustomTotalSpecialSerializer specialSerializer = new MyCustomTotalSpecialSerializer();
configuration.RegisterSpecialSerializer(specialSerializer);
```

### Recreate types

There are certain types that are not feasible for serialization and
deserialization, like for example instances of loggers. Instead you might
want to get the instance from a DI container instead. For that reason you can
register factories, deriving from `IFactory`, for a type so that instances of
this type will always be recreated. This can be done like this:

```csharp
Config config = new Config();
MyFactory myFactory = new MyFactory();
config.RegisterRecreateTypeFactory<MyType>(myFactory);
TotalSerializer totalSerializer = new TotalSerializer(config);
```

In the code example above the generic argument can actually be a type or an
interface. All classes deriving from the type or implementing the interface
will be recreated using the given factory.

> **Warning:** You need to use the same configuration for serialization and
> deserialization!

## Performance

Currently deserialization still takes 70.15% longer than serialization. Needs
to be checked how this can be improved.

## License

MIT — see [LICENSE](LICENSE).
