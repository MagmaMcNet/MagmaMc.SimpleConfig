# MagmaMc.MagmaSimpleConfig

Fully Compatible with INI And TOML Files
Converts basic Values Back Into its dedicated object

Just Simple And Fast

## Example

```cs
using MagmaMc.MagmaSimpleConfig

public class Program
{
	public static void Main(string[] ConsoleArgs)
	{
		// Load Config File and fail if file does not exist
		SimpleConfig MSC = new SimpleConfig("example.msc", false); 

		Console.Write("Enter Value For Key Test1:"); // I Enter: "ImportantValue1"
		string Value1 = Console.ReadLine();
		MSC.SetValue("Test1", Value1, ".ENV");
		
		Console.WriteLine("\n Value=" + MSC.GetValue("Test1", null, ".ENV")); 
		// Output:  "ImportantValue1"
		MSC.SetValue("array1", new string[] {"Value1", "Value2" }, ".ENV");
	}
}
```

example.msc
```ini
[.ENV]
Test1 => ImportantValue1
array1 => ["value1", "value2"]
```
