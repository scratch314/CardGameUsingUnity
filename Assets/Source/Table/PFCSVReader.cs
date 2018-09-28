using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class PFCSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
            return list;

        int lineIdx = 0;

        //헤더: 데이터 멤버와 이름이 같음
        var header = Regex.Split(lines[lineIdx++], SPLIT_RE);

        //설명 line
        Regex.Split(lines[lineIdx++], SPLIT_RE);

        //설명 line
        Regex.Split(lines[lineIdx++], SPLIT_RE);

        for (var i = lineIdx; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];

                value = value.TrimStart(TRIM_CHARS);
                value = value.TrimEnd(TRIM_CHARS);

                //기존: \\n => n 되버려서, 줄바꿈이 안됐다 - 18.04.22.kkw
                //줄바꿈 동작되게 수정함
                value = value.Replace("\\n", "\n");
                //value = value.Replace("\\", "");

                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }
}
