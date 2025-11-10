

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class StringUtils {
    /// <summary>
    /// 智能版本：根据上下文处理标点符号
    /// </summary>
    /// <param name="sentence">英文句子</param>
    /// <returns>智能处理的单词列表</returns>
    public static List<string> WordsSmart(string sentence) {
        List<string> words = new();
        if (string.IsNullOrWhiteSpace(sentence)) {
            return words;
        }

        // 使用更复杂的正则表达式处理各种情况
        // 匹配：单词 + 标点符号（但不包括引号等特殊符号）
        Regex smartRegex = new Regex(@"[\w'-]+(?:[.,!?;:]|(?=\s|$))");
        MatchCollection matches = smartRegex.Matches(sentence);

        foreach (Match match in matches.Cast<Match>()) {
            if (!string.IsNullOrWhiteSpace(match.Value)) {
                words.Add(match.Value);
            }
        }

        return words;
    }
}
