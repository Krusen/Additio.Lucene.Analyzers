using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace Additio.Lucene.Analyzers
{
    public class DanishAnalyzer : Analyzer
    {
        #region Stop words
        public static readonly string[] DanishStopWords =
        {
            "og",
            "i",
            "jeg",
            "det",
            "at",
            "en",
            "den",
            "til",
            "er",
            "som",
            "på",
            "de",
            "med",
            "han",
            "af",
            "for",
            "ikke",
            "der",
            "var",
            "mig",
            "sig",
            "men",
            "et",
            "har",
            "om",
            "vi",
            "min",
            "havde",
            "ham",
            "hun",
            "nu",
            "over",
            "da",
            "fra",
            "du",
            "ud",
            "sin",
            "dem",
            "os",
            "op",
            "man",
            "hans",
            "hvor",
            "eller",
            "hvad",
            "skal",
            "selv",
            "her",
            "alle",
            "vil",
            "blev",
            "kunne",
            "ind",
            "når",
            "være",
            "dog",
            "noget",
            "ville",
            "jo",
            "deres",
            "efter",
            "ned",
            "skulle",
            "denne",
            "end",
            "dette",
            "mit",
            "også",
            "under",
            "have",
            "dig",
            "anden",
            "hende",
            "mine",
            "alt",
            "meget",
            "sit",
            "sine",
            "vor",
            "mod",
            "disse",
            "hvis",
            "din",
            "nogle",
            "hos",
            "blive",
            "mange",
            "ad",
            "bliver",
            "hendes",
            "været",
            "thi",
            "jer",
            "sådan"
        };
        #endregion

        private ISet<string> ExclusionTable { get; set; }

        private ISet<string> StopTable { get; }

        private Version MatchVersion { get; }

        public DanishAnalyzer(Version matchVersion)
            : this(matchVersion, DefaultSetHolder.DefaultStopSet)
        {
        }

        public DanishAnalyzer(Version matchVersion, ISet<string> stopwords)
            : this(matchVersion, stopwords, CharArraySet.EMPTY_SET)
        {
        }

        public DanishAnalyzer(Version matchVersion, ISet<string> stopwords, ISet<string> stemExclusionTable)
        {
            StopTable = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stopwords));
            ExclusionTable = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stemExclusionTable));
            MatchVersion = matchVersion;
        }

        public DanishAnalyzer(Version matchVersion, params string[] stopwords)
            : this(matchVersion, StopFilter.MakeStopSet(stopwords))
        {
        }

        public DanishAnalyzer(Version matchVersion, HashSet<string> stopwords)
            : this(matchVersion, (ISet<string>) stopwords)
        {
        }

        public DanishAnalyzer(Version matchVersion, FileInfo stopwordsFile)
        {
            StopTable = WordlistLoader.GetWordSet(stopwordsFile);
            MatchVersion = matchVersion;
        }

        public static ISet<string> GetDefaultStopSet()
        {
            return DefaultSetHolder.DefaultStopSet;
        }

        public void SetStemExclusionTable(params string[] exclusionlist)
        {
            ExclusionTable = StopFilter.MakeStopSet(exclusionlist);
            PreviousTokenStream = null;
        }

        public void SetStemExclusionTable(ISet<string> exclusionlist)
        {
            ExclusionTable = exclusionlist;
            PreviousTokenStream = null;
        }

        public void SetStemExclusionTable(FileInfo exclusionlist)
        {
            try
            {
                ExclusionTable = WordlistLoader.GetWordSet(exclusionlist);
                PreviousTokenStream = null;
            }
            catch (IOException ex)
            {
                throw new Exception("", ex);
            }
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return
                new DanishStemFilter(
                    new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(MatchVersion),
                        new LowerCaseFilter(
                            new StandardFilter(
                                new StandardTokenizer(MatchVersion, reader))),
                        StopTable),
                    ExclusionTable);
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            if (overridesTokenStreamMethod)
                return TokenStream(fieldName, reader);

            var savedStreams = (SavedStreams) PreviousTokenStream;
            if (savedStreams == null)
            {
                savedStreams = new SavedStreams {Source = new StandardTokenizer(MatchVersion, reader)};
                savedStreams.Result = new StandardFilter(savedStreams.Source);
                savedStreams.Result = new LowerCaseFilter(savedStreams.Result);
                // TODO: Lucene.Net.Analysis.Compound.HyphenationCompoundWordTokenFilter
                savedStreams.Result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(MatchVersion), savedStreams.Result, StopTable);
                savedStreams.Result = new DanishStemFilter(savedStreams.Result, ExclusionTable);

                PreviousTokenStream = savedStreams;
            }
            else
            {
                savedStreams.Source.Reset(reader);
            }

            return savedStreams.Result;
        }

        private static class DefaultSetHolder
        {
            internal static readonly ISet<string> DefaultStopSet =
                CharArraySet.UnmodifiableSet(new CharArraySet(DanishStopWords, false));
        }

        private class SavedStreams
        {
            protected internal Tokenizer Source;
            protected internal TokenStream Result;
        }
    }
}
