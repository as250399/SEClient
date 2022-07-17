using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SEClient.Indexing
{
    public class IndexFromDB
    {
        public async void StartIndexing()
        {
            await Task.Factory.StartNew(() =>
                StartIndexingFromDB());
 
        }

        private static void StartIndexingFromDB()
        {
            ClearIndex(indexPath);
            List<List<string>> ListOfListOfTlogs = TlogsDao.GetAllTlogsFromDB();
            var NumberOfLists = ListOfListOfTlogs.Count;
            var CurrentListNumber = 0;
            foreach (var _ListOfTlogs in ListOfListOfTlogs)
            {
                if (_ListOfTlogs != null && _ListOfTlogs.Count > 0)
                    SetDataToSerachEngine(_ListOfTlogs, NumberOfLists, CurrentListNumber * _ListOfTlogs.Count);
                CurrentListNumber++;
            }
        }

        private static void ClearIndex(string indexPath)
        {
            using var dir = FSDirectory.Open(indexPath);
            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);
            writer.DeleteAll();

        }
    }
}