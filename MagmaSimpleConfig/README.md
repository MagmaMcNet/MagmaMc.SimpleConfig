# MagmaMc.MagmaSimpleConfig

Fully Compatible with INI And TOML Files
Converts basic Values Back Into its dedicated object

Just Simple And Fast

## Example

```cs
using MagmaMc.JEF
using MagmaMc.MagmaSimpleConfig

namespace AProgram
{
	public void Main(string[] ConsoleArgs)
	{
		// Load Config File and fail if file does not exist
		SimpleConfig MSC = new SimpleConfig("config.msc", false); 

		Console.Write("Enter Value For Key Test1:"); // I Enter: "ImportantValue1"
		string Value1 = Console.ReadLine();
		MSC.SetValue("Test1", Value1, "SectionName");
		
		Console.WriteLine("\n Value=" + MSC.GetValue("Test1", "DefualtValue", "SectionName")); 
		// Output:  "ImportantValue1"
	}
}
```

```ini
Test1 => ImportantValue1
```
