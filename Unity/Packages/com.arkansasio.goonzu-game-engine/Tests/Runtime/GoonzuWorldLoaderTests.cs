using System;
using NUnit.Framework;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine.Tests;

public sealed class GoonzuWorldLoaderTests
{
    [Test]
    public void NullTextAssetThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => GoonzuWorldLoader.LoadJson(null));
    }

    [Test]
    public void ValidJsonLoadsSeedWorld()
    {
        var json = new GameObject("SeedWorldJson");
        var asset = ScriptableObject.CreateInstance<TextAsset>();
        try
        {
            // TextAsset's text is constructor-backed, so construct an asset with explicit JSON.
            Object.DestroyImmediate(asset);
            asset = new TextAsset(
                "{\"version\":\"1\",\"style\":\"isometric\"," +
                "\"regions\":[{\"id\":\"r1\",\"name\":\"Region\",\"biome\":\"forest\",\"towns\":[\"t1\"]}]," +
                "\"towns\":[{\"id\":\"t1\",\"name\":\"Town\",\"taxRate\":0.05,\"services\":[]}]," +
                "\"resources\":[{\"id\":\"ore\",\"tier\":1,\"gathering\":\"mining\",\"recipe\":\"\"}]," +
                "\"starterSkills\":[],\"starterAssets\":{\"character\":\"hero\",\"town\":\"town\",\"tileset\":\"tiles\",\"ui\":\"ui\"}}");

            var world = GoonzuWorldLoader.LoadJson(asset);
            try
            {
                Assert.That(world.version, Is.EqualTo("1"));
                Assert.That(world.towns.Length, Is.EqualTo(1));
                Assert.That(world.regions[0].towns[0], Is.EqualTo("t1"));
            }
            finally
            {
                Object.DestroyImmediate(world);
            }
        }
        finally
        {
            Object.DestroyImmediate(asset);
            Object.DestroyImmediate(json);
        }
    }

    [Test]
    public void BrokenTownReferenceIsRejected()
    {
        var asset = new TextAsset(
            "{\"version\":\"1\",\"style\":\"isometric\"," +
            "\"regions\":[{\"id\":\"r1\",\"name\":\"Region\",\"biome\":\"forest\",\"towns\":[\"missing\"]}]," +
            "\"towns\":[],\"resources\":[],\"starterSkills\":[]}");

        try
        {
            var exception = Assert.Throws<FormatException>(() => GoonzuWorldLoader.LoadJson(asset));
            Assert.That(exception.Message, Does.Contain("unknown town"));
        }
        finally
        {
            Object.DestroyImmediate(asset);
        }
    }
}
