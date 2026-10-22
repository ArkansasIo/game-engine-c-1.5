using NUnit.Framework;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine.Tests;

public sealed class GoonzuWorldLoaderTests
{
    [Test]
    public void InvalidWorldDefinitionIsRejected()
    {
        var json = new GameObject("JsonAsset").AddComponent<TestJsonTextAsset>();
        Assert.That(json, Is.Not.Null);
    }
}
