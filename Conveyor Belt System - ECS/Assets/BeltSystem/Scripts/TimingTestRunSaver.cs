using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TimingTestRunSaver : MonoBehaviour {

    public static void WriteTestRunDataToFile (List<string> results) {

        string versionTextFileNameAndPath = "version.txt";
        string testResultsTextFileNameAndPath = "ExecutionTimeTestResults.txt";

        string versionText = CommonUtils.ReadTextFile(versionTextFileNameAndPath);
        string testResultsText = CommonUtils.ReadTextFile(testResultsTextFileNameAndPath);

        if (versionText != null) {
            versionText = versionText.Trim();

        }

        testResultsText += "\n";
        testResultsText += "\n" + "--------------------------------------------------";
        testResultsText += "\n";

        foreach (string line in results) {
            testResultsText += "\n" + line;
        }

        testResultsText += "\n";
        testResultsText += "\n" + "--------------------------------------------------";

        CommonUtils.WriteTextFile(testResultsTextFileNameAndPath, testResultsText);
    }
}
