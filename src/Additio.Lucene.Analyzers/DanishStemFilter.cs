using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using SF.Snowball.Ext;

namespace Additio.Lucene.Analyzers
{
    public class DanishStemFilter : TokenFilter
    {
        private DanishStemmer Stemmer { get; set; }
        private ISet<string> Exclusions { get; set; }
        private ITermAttribute TermAttribute { get; }

        public DanishStemFilter(TokenStream tokenStream)
            : base(tokenStream)
        {
            Stemmer = new DanishStemmer();
            TermAttribute = AddAttribute<ITermAttribute>();
        }

        public DanishStemFilter(TokenStream tokenStream, ISet<string> exclusiontable)
            : this(tokenStream)
        {
            Exclusions = exclusiontable;
        }

        public override bool IncrementToken()
        {
            if (!input.IncrementToken())
                return false;

            var term = TermAttribute.Term;
            if (Exclusions?.Contains(term) == true)
                return true;

            Stemmer.SetCurrent(term);
            Stemmer.Stem();
            var buffer = Stemmer.GetCurrent();

            if (buffer != null && !buffer.Equals(term))
                TermAttribute.SetTermBuffer(buffer);

            return true;
        }

        public void SetStemmer(DanishStemmer stemmer)
        {
            if (stemmer == null)
                return;

            Stemmer = stemmer;
        }

        public void SetExclusionTable(ISet<string> exclusiontable)
        {
            Exclusions = exclusiontable;
        }
    }
}
