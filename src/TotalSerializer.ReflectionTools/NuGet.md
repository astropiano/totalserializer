# TotalSerializer.ReflectionTools

Efficient CLR reflection tools.


Usage example:

```
using TotalSerializer.ReflectionTools.Extensions;


var obj = new MyExampleClass();
obj.SetValue("MyField", 2);

var value = obj.GetValue<int>("MyField")

```


Issues are tracked on the repository's issue tracker.
