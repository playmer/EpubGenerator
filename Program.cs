using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EbookHelper
{
    class Program
    {
        ////////////////////////////////////////
        // Footnotes
        ////////////////////////////////////////
        static readonly string cFootnoteContentBegin = "title='";
        static readonly string cFootnoteContentEnd = "'><sup>";

        static readonly string cFootnoteBegin = "<span id='easy-footnote";
        static readonly string cFootnoteEnd = "</sup></a></span>";

        static string parseLineForFootnotes(string aLine, List<string> aFootnotes)
        {
            int start = 0;
            int end = 0;

            while (aLine.Contains(cFootnoteBegin) && aLine.Contains(cFootnoteEnd))
            {
                start = aLine.IndexOf(cFootnoteBegin, 0);
                end = aLine.IndexOf(cFootnoteEnd, start) + cFootnoteEnd.Length;

                var footnote = aLine.Substring(start, end - start);

                // We haven't added the footnote yet, and they count up from 1.
                int footnoteNumber = aFootnotes.Count + 1;

                var linkTofootnote = $"<a href=\"index.html#footnote{footnoteNumber}\"><sup>{footnoteNumber}</sup></a>";
                var footnoteLinkBack = $"<a id=\"backFromFootnote{footnoteNumber}\"></a>";

                aFootnotes.Add(generateFootnoteLine(footnote, footnoteNumber));

                aLine = aLine.Replace(footnote, $"{linkTofootnote}{footnoteLinkBack}");
            }

            return aLine;
        }

        static string generateFootnoteLine(string aLine, int aFootnoteNumber)
        {
            var linkFromFootnote = $"<a href=\"index.html#backFromFootnote{aFootnoteNumber}\"><sup>{aFootnoteNumber}</sup></a>";
            var linkToFootnote = $"<a id=\"footnote{aFootnoteNumber}\"></a>";

            int start = aLine.IndexOf(cFootnoteContentBegin, 0) + cFootnoteContentBegin.Length;
            int end = aLine.IndexOf(cFootnoteContentEnd, start);

            var content = aLine.Substring(start, end - start);

            return $"{aFootnoteNumber}{linkToFootnote}. {content}{linkFromFootnote}";
        }

        ////////////////////////////////////////
        // Paragraphs
        ////////////////////////////////////////
        /// Parses all lines and places them into the StringBuilder as well as generating a list of footnotes.
        static void parseParagraphs(List<string> aLines, StringBuilder aBuilder)
        {
            var footnotes = new List<string>();

            foreach (var line in aLines)
            {
                parseParagraph(line, footnotes, aBuilder);
            }

            foreach (var footnote in footnotes)
            {
                aBuilder.Append("<p class=\"calibre1\">");
                aBuilder.Append(footnote);
                aBuilder.Append("</p>");
                aBuilder.Append("\n");
            }
        }

        static void parseParagraph(string aLine, List<string> aFootnotes, StringBuilder aBuilder)
        {
            var lineWithoutNewLines = aLine.Replace("\r\n", " ");

            lineWithoutNewLines = parseLineForFootnotes(lineWithoutNewLines, aFootnotes);


            aBuilder.Append("<p class=\"calibre1\">");
            aBuilder.Append(lineWithoutNewLines);
            aBuilder.Append("</p>");
            aBuilder.Append("\n");
        }

        static List<string> parseTextForParagraphs(string aText)
        {
            var paragraphs = new List<string>();

            int start = 0;
            int end = 0;

            string paragraphBegin = "<p>";
            string paragraphEnd = "</p>";

            while (end < aText.Length)
            {
                start = aText.IndexOf("<p>", end) + paragraphBegin.Length;
                end = aText.IndexOf("</p>", start);

                var line = aText.Substring(start, end - start);

                paragraphs.Add(line);

                end += paragraphEnd.Length;
            }

            return paragraphs;
        }


        static void Main(string[] args)
        {
            var builder = new StringBuilder();
            var text = File.ReadAllText("C:/Users/playm/Documents/ebooks/HaruhiTempoLoss.txt");

            var paragraphs = parseTextForParagraphs(text);

            parseParagraphs(paragraphs, builder);

            File.WriteAllText("C:/Users/playm/Documents/ebooks/HaruhiTempoLossProcessed.html", builder.ToString());
        }
    }
}
