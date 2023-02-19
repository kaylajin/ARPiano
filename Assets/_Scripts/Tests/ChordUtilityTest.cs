using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class ChordUtilityTest
{
    [Test]
    public void TestTriads()
    {
        List<string> keysPressed = new List<string>() { "C", "E", "G" };
        string result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("C", result);

        keysPressed = new List<string>() { "C", "Eb", "G" };
        result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("C-", result);
    }

    [Test]
    public void Test7thChords()
    {
        List<string> keysPressed = new List<string>() { "C", "E", "G", "B" };
        string result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("Cma7", result);

        keysPressed = new List<string>() { "C", "Eb", "G", "Bb" };
        result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("C-7", result);
    }

    [Test]
    public void TestInverted()
    {
        List<string> keysPressed = new List<string>() { "E", "G", "C2" };
        string result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("C", result);

        keysPressed = new List<string>() { "E", "G", "A", "C2" };
        result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("A-7", result);
    }

    [Test]
    public void TestUnknown()
    {
        List<string> keysPressed = new List<string>() { "E", "G", "C2", "A", "B" };
        string result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("", result);

        keysPressed = new List<string>() { "C", "Eb" };
        result = ChordUtility.GetChord(keysPressed);
        Assert.AreEqual("", result);
    }
}
