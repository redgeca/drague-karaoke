using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using KaraokeClient.Models;
using Lucene.Net.Analysis;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using KaraokeClient.lucene.analyzers;
using Lucene.Net.QueryParsers;
using Lucene.Net.Index;
using KaraokeClient.contexts;
using KaraokeCoreObjects;
using KaraokeCoreObjects.misc;

namespace KaraokeClient.Controllers
{
    public class HomeController : Controller
    {
        private static readonly String SINGERNAME_COOKIE = "singerName";

        public IActionResult Index()
        {
            // Check if Karaoke is accepting requests
            using (var db = new SongDBContext())
            {
                try
                {
                    KaraokeConfiguration state = db.KaraokeConfiguration.Where(
                        s => s.key == Constants.KARAOKE_STATE_FLAG).First();
                    if (state.value == Constants.STOPPED_FLAG)
                    {
                        return View("NotStarted");
                    }
                }
                catch (InvalidOperationException)
                {
                    return View("NotStarted");
                }
                catch (IndexOutOfRangeException)
                {
                    return View("NotStarted");
                }
            }

            String singerNameCookie = Request.Cookies[SINGERNAME_COOKIE];

            SongRequest songRequest = new SongRequest();
            songRequest.singerName = singerNameCookie;
            songRequest.songId = 1;

            return View("Index", songRequest);
        }

        [HttpGet]
        public ActionResult KeywordSearch(string term)
        {
            Analyzer analyzer = new ASCIIFoldingAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            // Perform a search
            var searcher = getSearcher();
            var hits_limit = 10;

            BooleanQuery finalQuery = new BooleanQuery();

            finalQuery.Add(getPrefixQuery(term, analyzer), Occur.SHOULD);
            searcher.SetDefaultFieldSortScoring(true, true);

            ScoreDoc[] hits = searcher.Search(finalQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
            ScoreDoc[] fuzzyHits = searcher.Search(getFuzzyQuery(term, analyzer), null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            List<ScoreDoc> scoreDocs = new List<ScoreDoc>();
            scoreDocs.AddRange(hits);
            if (hits.Length < 10)
            {
                scoreDocs.AddRange(fuzzyHits);
            }

            List<String> searchResults = new List<String>();
            foreach (ScoreDoc hit in scoreDocs)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                searchResults.Add(document.Get(Constants.TITLE_FIELD) + " par " + document.Get(Constants.ARTIST_FIELD));
            }
            return Json(searchResults.ToArray().Take(10));
        }

        private IndexSearcher getSearcher()
        {
            String indexLocation = Constants.INDEX_FOLDER;
            Directory indexDirectory = FSDirectory.Open(indexLocation);

            // Perform a search
            return new IndexSearcher(indexDirectory, false);
        }

        [HttpGet]
        public ActionResult ArtistSearch(string term)
        {
            Analyzer analyzer = new ASCIIFoldingAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var hits_limit = 5000;

            BooleanQuery finalQuery = getPrefixQuery(Constants.ARTIST_FIELD, 3, term, analyzer);
            IndexSearcher searcher = getSearcher();

            searcher.SetDefaultFieldSortScoring(true, true);
            ScoreDoc[] hits = searcher.Search(finalQuery, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
            ScoreDoc[] fuzzyHits = searcher.Search(getFuzzyQuery(Constants.ARTIST_FIELD, term, analyzer), 
                null, hits_limit, Sort.RELEVANCE).ScoreDocs;

            List<String> searchResults = new List<String>();
            foreach (ScoreDoc hit in hits)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                string artist = document.Get(Constants.ARTIST_FIELD);
                if (!searchResults.Contains(artist))
                {
                    searchResults.Add(document.Get(Constants.ARTIST_FIELD));
                }
            }

            foreach (ScoreDoc hit in fuzzyHits)
            {
                var document = searcher.IndexReader.Document(hit.Doc);
                string artist = document.Get(Constants.ARTIST_FIELD);
                if (!searchResults.Contains(artist))
                {
                    searchResults.Add(document.Get(Constants.ARTIST_FIELD));
                }
            }

            searcher.Dispose();
            return Json(searchResults.ToArray().Take(10));
        }


        [HttpGet]
        public ActionResult CompleteSelect()
        {
            return View();
        }

        // Define the list which you have to show in Drop down List
        public List<SelectListItem> getCategories()
        {
            List<SelectListItem> myList = new List<SelectListItem>();
            var data = new[]{
                 new SelectListItem{ Value="1",Text="Monday"},
                 new SelectListItem{ Value="2",Text="Tuesday"},
                 new SelectListItem{ Value="3",Text="Wednesday"},
                 new SelectListItem{ Value="4",Text="Thrusday"},
                 new SelectListItem{ Value="5",Text="Friday"},
                 new SelectListItem{ Value="6",Text="Saturday"},
                 new SelectListItem{ Value="7",Text="Sunday"},
             };
            myList = data.ToList();
            return myList;
        }

        [HttpPost]
        public ActionResult SubmitBtn(SongRequest songRequest)
        {
            if (ModelState.IsValid)
            {
                string singerName = songRequest.singerName;
                using (var db = new SongDBContext())
                {
                    try
                    {
                        string[] fields = songRequest.title.Split(" par ");
                        String query = fields[0] + fields[1];
                        Artist artist = (Artist)db.Artists.Where(a => a.Name == fields[1]).First();
                        Song song = (Song) db.Songs.Where(s => s.Title == fields[0] && s.ArtistId == artist.Id).First();
                        if (song != null) {
                            Request request = new KaraokeCoreObjects.Request();
                            request.SingerName = singerName;
                            request.SongId = song.Id;
                            request.Notes = songRequest.notes;
                            request.RequestTime = DateTime.Now;
                            db.Requests.Add(request);
                            db.SaveChanges();
                        }

                    }
                    catch (IndexOutOfRangeException)
                    {
                        ViewData["SubmitSong"] = "La chanson demandée n'existe pas";
                        return View("Index", songRequest);
                    }
                    catch (InvalidOperationException)
                    {
                        ViewData["SubmitSong"] = "La chanson demandée n'existe pas";
                        return View("Index", songRequest);
                    }
                    catch (ArgumentNullException)
                    {
                        ViewData["SubmitSong"] = "La chanson demandée n'existe pas";
                        return View("Index", songRequest);
                    }

                    ModelState.Clear();

                    ViewData["SubmitSong"] = "Demande effectuée avec succès à " + String.Format("{0:HH:mm:ss}", DateTime.Now);
                    CookieOptions cookieOption = new CookieOptions();

                    cookieOption.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Append(SINGERNAME_COOKIE, singerName, cookieOption);
                }

                SongRequest newRequest = new SongRequest();
                newRequest.singerName = singerName;

                return View("Index", newRequest);
            }

            return View("Index");
        }

        private BooleanQuery getPrefixQuery(string pField, float pBoost, string pSearchQuery, Analyzer pAnalyzer)
        {
            var escapedTerm = QueryParser.Escape(pSearchQuery);
            var prefixedTerm = String.Concat("\"", escapedTerm, "\"");

            var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, pField, pAnalyzer);
            Query query = queryParser.Parse(prefixedTerm);

            ISet<Term> terms = new HashSet<Term>();
            query.ExtractTerms(terms);

            BooleanQuery termQuery = new BooleanQuery();
            termQuery.Add(getQueryFromTerms(terms), Occur.SHOULD);
            termQuery.Boost = pBoost;
            return termQuery;
        }

        private BooleanQuery getFuzzyQuery(string pField, string searchTerms, Analyzer pAnalyzer)
        {
            BooleanQuery resultQuery = new BooleanQuery();

            String[] terms = searchTerms.Split(" ");
            foreach (string term in terms)
            {
                resultQuery.Add(new FuzzyQuery(new Term(pField, term.ToLower())), Occur.SHOULD);
            }
            return resultQuery;
        }

        private BooleanQuery getPrefixQuery(string searchQuery, Analyzer analyzer)
        {
            BooleanQuery termQuery = new BooleanQuery();
            termQuery.Add(getPrefixQuery(Constants.TITLE_FIELD, 15, searchQuery, analyzer), Occur.SHOULD);
            termQuery.Add(getPrefixQuery(Constants.ARTIST_FIELD, 5, searchQuery, analyzer), Occur.SHOULD);
            termQuery.Add(getPrefixQuery(Constants.CATEGORY_FIELD, 1, searchQuery, analyzer), Occur.SHOULD);

            return termQuery;
        }

        private BooleanQuery getFuzzyQuery(string searchTerms, Analyzer pAnalyzer)
        {
            BooleanQuery resultQuery = new BooleanQuery();

            resultQuery.Add(getFuzzyQuery(Constants.ARTIST_FIELD, searchTerms, pAnalyzer), Occur.SHOULD);
            resultQuery.Add(getFuzzyQuery(Constants.TITLE_FIELD, searchTerms, pAnalyzer), Occur.SHOULD);
            resultQuery.Add(getFuzzyQuery(Constants.CATEGORY_FIELD, searchTerms, pAnalyzer), Occur.SHOULD);

            return resultQuery;
        }


        public BooleanQuery getQueryFromTerms(ISet<Term> pTerms)
        {

            BooleanQuery query = new BooleanQuery();
            foreach (Term term in pTerms)
            {
                query.Add(new PrefixQuery(term), Occur.SHOULD);
            }

            return query;
        }

    }
}
