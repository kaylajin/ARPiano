using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChordUtility
{
    private static Dictionary<string, double> keysToFrequencies = new Dictionary<string, double> {
        {"C3", 261.63}, //actually C4
        {"Db3", 277.18},
        {"D3", 293.66},
        {"Eb3", 311.13},
        {"E3", 329.63},
        {"F3", 349.23},
        {"F#3", 369.99},
        {"G3", 392.0},
        {"Ab3", 415.30},
        {"A3", 440.0},
        {"Bb3", 466.16},
        {"B3", 493.88},
        {"C4", 523.25} //C5
    };
    private static Dictionary<double, string> frequenciesToKeys =
        keysToFrequencies.ToDictionary(x => x.Value, x => x.Key == "C4" ? "C" : x.Key.Replace("3", ""));

    public static string GetChord(List<string> keysPressed)
    {
        if (keysPressed.Count < 3 || keysPressed.Count > 4)
        {
            return "";
        }

        List<double> frequencies = new List<double>();

        // Convert keys to frequencies
        foreach (string key in keysPressed)
        {
            frequencies.Add(keysToFrequencies[key]);
        }
        frequencies.Sort();

        // TODO rootless voicings/tensions? - actually need the root in context
        // TODO sus4 aug7 tension support - more than one notes with intervals < 3

        // Find the root first
        int rootIndex = 0;
        for (int i = 1; i < frequencies.Count; i++)
        {
            double ratio = frequencies[i] / frequencies[i - 1];
            int interval = (int)Math.Round(12 * Math.Log(ratio, 2));
            if (interval < 3 || interval > 4)
            {
                rootIndex = i;
            }
        }

        // Get intervals
        // num semitones = 12* log_2(f2/f1)
        List<int> intervals = new List<int>();
        for (int i = 0; i < frequencies.Count-1; i++)
        {
            // index starts from rootIndex+1, then to the beginning of array until rootIndex-1
            int index = (i + rootIndex + 1) % frequencies.Count;
            
            double ratio;
            if (frequencies[index] > frequencies[rootIndex])
            {
                ratio = frequencies[index] / frequencies[rootIndex];
            }
            else
            {
                // "place" the note in the next octave
                ratio = (frequencies[index] * 2) / frequencies[rootIndex];
            }
            int interval = (int)Math.Round(12 * Math.Log(ratio, 2));
            intervals.Add(interval);
        }

        string chord = frequenciesToKeys[frequencies[rootIndex]];
        Debug.Log($"root: {chord} and intervals: {string.Join(", ", intervals)}");

        // Now, intervals are in order and in root position
        switch (intervals.Count)
        {
            case 2:
                switch ((intervals[0], intervals[1]))
                {
                    case (4, 7):
                        // nothing to append
                        break;
                    case (3, 7):
                        chord += "-";
                        break;
                    case (3, 6):
                        chord += "dim";
                        break;
                    case (4, 8):
                        chord += "aug";
                        break;
                    default:
                        chord = "";
                        break;
                }
                break;
            case 3:
                switch ((intervals[0], intervals[1], intervals[2]))
                {
                    case (4, 7, 11):
                        chord += "ma7";
                        break;
                    case (4, 7, 10):
                        chord += "7";
                        break;
                    case (3, 7, 10):
                        chord += "-7";
                        break;
                    case (3, 6, 10):
                        chord += "-7(b5)";
                        break;
                    case (3, 6, 9):
                        chord += "dim7";
                        break;
                    case (3, 7, 11):
                        chord += "-ma7";
                        break;
                    default:
                        chord = "";
                        break;
                }
                break;
            default:
                chord = "";
                break;
        }

        return chord;
    }
}
