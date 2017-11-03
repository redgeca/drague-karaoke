using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaraokeClient.lucene.analyzers
{
    public class ASCIIFoldingAnalyzer : StandardAnalyzer
    {
        Lucene.Net.Util.Version matchVersion;

        public ASCIIFoldingAnalyzer(Lucene.Net.Util.Version p_matchVersion)
            : base(p_matchVersion)
        {
            matchVersion = p_matchVersion;
        }

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            TokenStream result = new StandardTokenizer(matchVersion, reader);
            result = new StandardFilter(result);
            result = new LowerCaseFilter(result);
            result = new ASCIIFoldingFilter(result);
            return result;
        }

    }
 
}
