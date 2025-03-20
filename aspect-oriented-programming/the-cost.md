---
icon: '6'
---

# The cost

### Interception

<figure><img src="../.gitbook/assets/image (1).png" alt=""><figcaption><p>NBomber load test (50 parallel runs with ramping and warm-up)</p></figcaption></figure>

```markdown
BenchmarkDotNet v0.14.0, macOS Sequoia 15.3.2 (24D81) [Darwin 24.3.0]
Apple M3 Max, 1 CPU, 14 logical and 14 physical cores
.NET SDK 8.0.404
  [Host]     : .NET 8.0.13 (8.0.1325.6609), Arm64 RyuJIT AdvSIMD
  Job-MJSVBB : .NET 8.0.13 (8.0.1325.6609), Arm64 RyuJIT AdvSIMD

InvocationCount=1  UnrollFactor=1  

| Method          | Mean        | Error      | StdDev     | Allocated |
|---------------- |------------:|-----------:|-----------:|----------:|
| InterceptedCall | 1,612.17 μs | 157.311 μs | 451.356 μs | 568.69 KB |
| PlainCall       |    11.47 μs |   0.339 μs |   0.983 μs |  17.91 KB |
```

### IL Weaving

* The "Aspect" is not visible until you compile/ship the code
* Tools for design-time support are sparse

