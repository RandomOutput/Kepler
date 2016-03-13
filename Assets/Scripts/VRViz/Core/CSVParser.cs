using System;
using System.Collections;
using System.Collections.Generic;

namespace VRViz {
  namespace Core {
    public enum CSVErrorCode {
      ERR_OK,
      ERR_NOT_ENOUGH_FIELDS,
      ERR_COULD_NOT_CAST_FIELD,
    }

    public class CSVParser<T> {
      public class ParserOutput {
        public readonly T[] parsedObjects;
        public readonly List<string>[] uniqueValuesInRows;

        public ParserOutput(T[] objects, List<string>[] uniqueValues) {
          parsedObjects = objects;
          uniqueValuesInRows = uniqueValues;
        }
      }

      public delegate void ErrorOutDelegate(string errorString);
      public delegate CSVErrorCode RowToObjectFactory(string[] fields, out T parsedObject);

      public static ParserOutput ParseCSVRowsToType(string rawCSV, RowToObjectFactory factory, ErrorOutDelegate errorOutput = null) {
        bool initializedUniqueValuesStore = false;
        List<string>[] uniqueValues = null;

        if (errorOutput == null)
          errorOutput = (string err) => { };

        List<T> parsedObjects = new List<T>();

        string[] rows = rawCSV.Split('\n');

        for (int i = 0; i < rows.Length; i++) {
          string row = rows[i];

          if (row.Length == 0)
            continue;

          if (row[0] == '#')
            continue;

          string[] fields = Split(row, ',', '"');

          if (!initializedUniqueValuesStore) {
            uniqueValues = new List<string>[fields.Length];
            for (int j = 0; j < fields.Length; j++) {
              uniqueValues[j] = new List<string>();
            }
            initializedUniqueValuesStore = true;
          }

          for (int j = 0; j < fields.Length; j++) {
            if (fields[j] == "")
              continue;
            if (!uniqueValues[j].Contains(fields[j].ToLower())) {
              uniqueValues[j].Add(fields[j].ToLower());
            }
          }

          T parsedObject;
          CSVErrorCode result = factory(fields, out parsedObject);
          if (result != CSVErrorCode.ERR_OK) {
            errorOutput("Could not parse row #" + (i + 1) + " error: " + result.ToString());
            continue;
          }

          parsedObjects.Add(parsedObject);
        }

        ParserOutput output = new ParserOutput(parsedObjects.ToArray(), uniqueValues);
        return output;
      }

      private static string[] Split(string toSplit, char splitChar, params char[] pairedExclusionChars) {
        List<string> splitStrings = new List<string>();
        Dictionary<char, int> pairedCharacterState = new Dictionary<char, int>();

        // intialize paired chars
        foreach (char exlusionChar in pairedExclusionChars) {
          pairedCharacterState.Add(exlusionChar, 0);
        }

        int currentStringStartIndex = 0;

        for (int i = 0; i < toSplit.Length; i++) {
          char currentChar = toSplit[i];

          if (currentChar == splitChar && !IsInsidePairedBlock(pairedCharacterState)) {
            if (currentStringStartIndex == i) {
              splitStrings.Add("");
            }
            else {
              splitStrings.Add(toSplit.Substring(currentStringStartIndex, i - currentStringStartIndex));
            }

            currentStringStartIndex = i + 1;
            continue;
          }

          if (pairedCharacterState.ContainsKey(currentChar)) {
            pairedCharacterState[currentChar] = pairedCharacterState[currentChar] == 1 ? 0 : 1;
          }
        }

        return splitStrings.ToArray();
      }

      private static bool IsInsidePairedBlock(Dictionary<char, int> pairedCharState) {
        foreach (int val in pairedCharState.Values) {
          if (val > 0)
            return true;
        }

        return false;
      }
    }
  }
}