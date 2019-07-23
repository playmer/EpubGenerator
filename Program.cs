using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EbookHelper
{
    class Program
    {
        //<span id='easy-footnote-1-322' class='easy-footnote-margin-adjust'></span><span class='easy-footnote'><a href='#easy-footnote-bottom-1-322' title='A kind of bright pink mollusc.'><sup>1</sup></a></span>

        static string cFootnoteStart = "title='";
        static string cFootnoteEnd = "'><sup>";

        static string parseFootnoteLine(string aLine, int aFootnoteNumber)
        {
            var linkFromFootnote = $"<a href=\"index.html#backFromFootnote{aFootnoteNumber}\"><sup>{aFootnoteNumber}</sup></a>";
            var linkToFootnote = $"<a id=\"footnote{aFootnoteNumber}\"></a>";

            int start = aLine.IndexOf(cFootnoteStart, 0) + cFootnoteStart.Length;
            int end = aLine.IndexOf(cFootnoteEnd, start);

            var content = aLine.Substring(start, end - start);

            return $"{aFootnoteNumber}{linkToFootnote}. {content}{linkFromFootnote}";
        }

        static void Main(string[] args)
        {
            var builder = new StringBuilder();
            var text = File.ReadAllText("C:/Users/playm/Documents/ebooks/HaruhiTempoLoss.txt");

            var lines = new List<string>();

            {
                int start = 0;
                int end = 0;

                string paragraphBegin = "<p>";
                string paragraphEnd = "</p>";

                while (end < text.Length)
                {
                    start = text.IndexOf("<p>", end) + paragraphBegin.Length;
                    end = text.IndexOf("</p>", start);

                    var line = text.Substring(start, end - start);

                    lines.Add(line);

                    end += paragraphEnd.Length;
                }
            }

            var footnotes = new List<string>();

            {
                var footNoteBegin = "<span id='easy-footnote";
                var footNoteEnd = "</sup></a></span>";

                foreach (var line in lines)
                {
                    var lineWithoutNewLines = line.Replace("\r\n", " ");

                    int start = 0;
                    int end = 0;

                    while (lineWithoutNewLines.Contains(footNoteBegin) && lineWithoutNewLines.Contains(footNoteEnd))
                    {
                        start = lineWithoutNewLines.IndexOf(footNoteBegin, 0);
                        end = lineWithoutNewLines.IndexOf(footNoteEnd, start) + footNoteEnd.Length;

                        var footnote = lineWithoutNewLines.Substring(start, end - start);

                        // We haven't added the footnote yet, and they count up from 1.
                        int footnoteNumber = footnotes.Count + 1;

                        var linkTofootnote = $"<a href=\"index.html#footnote{footnoteNumber}\"><sup>{footnoteNumber}</sup></a>";
                        var footnoteLinkBack = $"<a id=\"backFromFootnote{footnoteNumber}\"></a>";

                        footnotes.Add(parseFootnoteLine(footnote, footnoteNumber));

                        lineWithoutNewLines = lineWithoutNewLines.Replace(footnote, $"{linkTofootnote}{footnoteLinkBack}");
                    }

                    builder.Append("<p class=\"calibre1\">");
                    builder.Append(lineWithoutNewLines);
                    builder.Append("</p>");
                    builder.Append("\n");
                }
            }

            foreach (var footnote in footnotes)
            {
                builder.Append("<p class=\"calibre1\">");
                builder.Append(footnote);
                builder.Append("</p>");
                builder.Append("\n");
            }

            File.WriteAllText("C:/Users/playm/Documents/ebooks/HaruhiTempoLossProcessed.html", builder.ToString());
        }
    }
}
