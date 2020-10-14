I've had an idea to recreate `Entities.Forech`-like syntax for physics calculations in Unity. In other words, to enable systax like this:

```csharp
Trace
    .Sphere( (0,1,0) , 1 )
    .Ray( (-1,-1,-1) , (1,1,1) )
    .Cast( out bool didHit , out var raycastHit );
```
.. where `Sphere` and `Ray` can be replaced with any other primitive ones want to test against.
Needs extending^2 for any practical use ofc. But as a proof of concept - it seems to work.

# Requirements
`"com.unity.physics": "0.5.0-preview.1"`
