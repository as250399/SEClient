using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Lucene.Net.Analysis.Standard;
using SEClient.Rabbit;
using SEClient.Contract;
using SEClient.Dao;
using SEClient.Helper;
using Lucene.Net.Search.Grouping;

namespace SEClient.Models
{
    public class SerachEngineControler
    {

        // Ensures index backward compatibility
        const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        //Update progressbar event
        public static event EventHandler UpdateProgressBar;
        public static Query SelectedQuery { get; set; }
        public static string indexPath { get; set; }
        public static SearchDefintion _SearchDefintion { get; set; }
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
        
        public static void GetDataFromRabbitToSerachEngine()
        {
            TlogFromRabbit.GetMessagesFromRabbit();
            TlogFromRabbit.UpdateRabbitMessage += TlogFromRabbit_UpdateRabbitMessage;
        }

        private static void TlogFromRabbit_UpdateRabbitMessage(object sender, EventArgs e)
        {
            var UpdateRabbitMessageEvent = (UpdateMessageEventArgs)e;
            var RabbitMessage = UpdateRabbitMessageEvent.RabbitMessage;
            if (RabbitMessage != "")
                SetDataToSerachEngine(new List<string> { RabbitMessage }, 1, 0);
        }

        public static void StartIndexingFromDB()
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

        private static void SetDataToSerachEngine(List<string> ListOfTlogs, int NumberOfLists, int ProgresBarCounter)
        {
            using var dir = FSDirectory.Open(indexPath);
            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);
            //for Tlog parsing
            XmlDocument Xmldoc = new XmlDocument();
            foreach (var Tlog in ListOfTlogs)
            {
                Xmldoc.LoadXml(Tlog);
                Document doc = new Document();
                AddTlogToDoc(Xmldoc, doc, Tlog, writer);
                ProgresBarCounter++;
                //OnUpdateProgressBar(new UpdateProgressBarEventArgs() { ProgresBarCurrentValue = ProgresBarCounter, ProgresBarMax = ListOfTlogs.Count * NumberOfLists });
            }
            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

        private static void AddTlogToDoc(XmlDocument Xmldoc, Document doc, string Tlog, IndexWriter writer)
        {
            var TransactionType = GetTransactionType(Xmldoc);
            var OperatorLock = GetValueFromXmlElment(Xmldoc, TlogFieldsEnum.OperatorLock.ToString(), "");
            if (OperatorLock != "null")
                OperatorLock = TlogFieldsEnum.OperatorLock.ToString().ToLower();

            var TareQuantityFromTlog = GetValueFromXmlElment(Xmldoc, "TareQuantity", "Units");
            double TareQuantity = 0.00;
            if (TareQuantityFromTlog != "null")
                TareQuantity = double.Parse(TareQuantityFromTlog);
            var UnitID = GetValueFromXmlElment(Xmldoc, "UnitID", "");
            var OperatorID = GetValueFromXmlElment(Xmldoc, TlogFieldsEnum.OperatorID.ToString(), "");
            var WorkstationID = GetValueFromXmlElment(Xmldoc, TlogFieldsEnum.WorkstationID.ToString(), "");
            var EndDateTime = Int32.Parse(GetValueFromXmlElment(Xmldoc, TlogFieldsEnum.EndDateTime.ToString(), "").Split('T')[0].Replace("-", ""));
            var TransactionStatus = GetValueFromXmlElment(Xmldoc, "RetailTransaction", "TransactionStatus").ToLower();
            var TransactionID = GetValueFromXmlElment(Xmldoc, TlogFieldsEnum.TransactionID.ToString(), "");

            doc.Add(new DoubleField(TlogFieldsEnum.TareQuantity.ToString(), TareQuantity, Field.Store.YES));
            doc.Add(new Field(TlogFieldsEnum.BusinessUnitID.ToString(), UnitID, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.OperatorID.ToString(), OperatorID, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TransactionID.ToString(), TransactionID, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.WorkstationID.ToString(), WorkstationID, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Int32Field(TlogFieldsEnum.EndDateTime.ToString(), EndDateTime, Field.Store.YES));
            doc.Add(new Field(TlogFieldsEnum.TransactionStatus.ToString(), TransactionStatus, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TransactionType.ToString(), TransactionType.ToLower(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.OperatorLock.ToString(), OperatorLock, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TransactionNetAmount.ToString(), GetValueFromXmlElment(Xmldoc, "Total", "TransactionNetAmount"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TransactionTaxAmount.ToString(), GetValueFromXmlElment(Xmldoc, "Total", "X:TransactionTaxSurcharge"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderCash.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "1"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderVisa.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "31"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderGiftCard.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "20"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderEwic.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "965"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderDebit.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "35"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderAmex.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "33"), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(TlogFieldsEnum.TenderEbtFoodStamp.ToString(), GetTenderAmountFromXmlElment(Xmldoc, "37"), Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
            if (TransactionType == TransactionTypeEnum.RetailTransactionLog.ToString() && TransactionStatus == "null")
            {
                AddSellItemsToNewDoc(Xmldoc, UnitID, OperatorID, WorkstationID, EndDateTime, writer);
                AddSellPromotionToNewDoc(Xmldoc, UnitID, OperatorID, WorkstationID, EndDateTime, writer);
            }
        }
        public static List<ListViewRow> GetDataToSerachEngine(string indexPath)
        {
            using var dir = FSDirectory.Open(indexPath);
            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);
            // Re-use the writer to get real-time updates
            using var reader = writer.GetReader(applyAllDeletes: true);
            var searcher = new IndexSearcher(reader);
            var results = new List<ListViewRow>();

            if (_SearchDefintion.SearchType == SearchTypeEnum.SearchByGroup)
            {
                var result = GetResultsByGroupBy(_SearchDefintion.GroupFiledId, searcher);
                if (result.Groups.Length > 0)
                {
                    foreach (var Group in result.Groups)
                    {
                        var ExtraData = "";
                        if (_SearchDefintion.GroupFiledId == TlogFieldsEnum.PromotionId.ToString())
                            ExtraData = searcher.Doc(Group.ScoreDocs[0].Doc).Get(TlogFieldsEnum.PromotionDescription.ToString());

                        if (_SearchDefintion.GroupFiledId == TlogFieldsEnum.Product.ToString())
                            ExtraData = searcher.Doc(Group.ScoreDocs[0].Doc).Get(TlogFieldsEnum.ProductDescription.ToString());

                        if (Group.GroupValue != null)
                            results.Add(SetListViewRowSearchByGroup(_SearchDefintion, Group, ExtraData));
                    }
                }
                else
                    return null;
            }

            if (_SearchDefintion.SearchType == SearchTypeEnum.SearchRegular)
            {
                var hits = searcher.Search(SelectedQuery, ConstantsVariables.MaxResults).ScoreDocs;
                //only if we have results goto ShowTheResultsOnListView
                if (hits.Length > 0)
                {
                    foreach (var hit in hits)
                    {
                        results.Add(SetListViewRowSearchRegular(_SearchDefintion.SearchType, searcher.Doc(hit.Doc)));
                    }
                    results[0].TotalHits = hits.Length.ToString();
                }
                else
                    return null;
            }

            if (_SearchDefintion.SearchType == SearchTypeEnum.SearchRegularSumTrx)
            {
                var hits = searcher.Search(SelectedQuery, 200000).ScoreDocs;

                if (hits.Length > 0)
                {
                    var BusinessUnit = searcher.Doc(hits[0].Doc).Get(TlogFieldsEnum.BusinessUnitID.ToString());
                    var result = GetResultsByGroupBy(_SearchDefintion.GroupFiledId, searcher);
                    if (result.Groups.Length > 1)
                        BusinessUnit = "0";
                    results.Add(SetListViewRowSumTrxNetAmount(_SearchDefintion, BusinessUnit, "BusinessUnitId", GetTotalTransactions(hits, searcher)));
                }
                else
                    return null;
            }

            if (_SearchDefintion.SearchType == SearchTypeEnum.SearchRegularCheckTotalAmount)
            {
                var hits = searcher.Search(SelectedQuery, ConstantsVariables.MaxResults).ScoreDocs;
                //only if we have results goto ShowTheResultsOnListView
                if (hits.Length > 0)
                {
                    int ExpectedScoreDoc = 0;
                    double ExpectedTransactionNetAmount = 0;
                    if (_SearchDefintion.GroupFiledId == "LowestAmount")
                        ExpectedTransactionNetAmount = 100000000;
                    foreach (var hit in hits)
                    {
                        var TransactionNetAmount = double.Parse(searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TransactionNetAmount.ToString()));

                        if (_SearchDefintion.GroupFiledId == "HighestAmount")
                        {
                            if (TransactionNetAmount > ExpectedTransactionNetAmount)
                            {
                                ExpectedScoreDoc = hit.Doc;
                                ExpectedTransactionNetAmount = TransactionNetAmount;
                            }
                        }

                        if (_SearchDefintion.GroupFiledId == "LowestAmount")
                        {
                            if (TransactionNetAmount < ExpectedTransactionNetAmount)
                            {
                                ExpectedScoreDoc = hit.Doc;
                                ExpectedTransactionNetAmount = TransactionNetAmount;
                            }
                        }
                    }
                    results.Add(SetListViewRowSearchRegular(_SearchDefintion.SearchType, searcher.Doc(ExpectedScoreDoc)));
                    results[0].TotalHits = "1";
                }
                else
                    return null;
            }

            return results;
        }

        private static TotalTransactions GetTotalTransactions(ScoreDoc[] hits, IndexSearcher searcher)
        {
            TotalTransactions _TotalTransactions = new TotalTransactions();

            foreach (var hit in hits)
            {
                var TransactionNetAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TransactionNetAmount.ToString());
                var TransactionTaxAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TransactionTaxAmount.ToString());
                var CashTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderCash.ToString());
                var VisaTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderVisa.ToString());
                var DebitTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderDebit.ToString());
                var AmexTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderAmex.ToString());
                var GiftCardTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderGiftCard.ToString());
                var EbtFoodStampTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderEbtFoodStamp.ToString());
                var EwicTenderAmount = searcher.Doc(hit.Doc).Get(TlogFieldsEnum.TenderEwic.ToString());
                if (TransactionNetAmount != "null")
                {
                    _TotalTransactions.TotalTransactionsNetAmount = _TotalTransactions.TotalTransactionsNetAmount + double.Parse(TransactionNetAmount);
                    _TotalTransactions.TotalTransactionsTaxAmount = _TotalTransactions.TotalTransactionsTaxAmount + double.Parse(TransactionTaxAmount);
                    if (CashTenderAmount != "null")
                    {
                        _TotalTransactions.TotalCashTenderAmount = _TotalTransactions.TotalCashTenderAmount + double.Parse(CashTenderAmount);
                        _TotalTransactions.TotalEwicTenderAmount = _TotalTransactions.TotalEwicTenderAmount + double.Parse(EwicTenderAmount);
                        _TotalTransactions.TotalVisaTenderAmount = _TotalTransactions.TotalVisaTenderAmount + double.Parse(VisaTenderAmount);
                        _TotalTransactions.TotalDebitTenderAmount = _TotalTransactions.TotalDebitTenderAmount + double.Parse(DebitTenderAmount);
                        _TotalTransactions.TotalAmexTenderAmount = _TotalTransactions.TotalAmexTenderAmount + double.Parse(AmexTenderAmount);
                        _TotalTransactions.TotalGiftCardTenderAmount = _TotalTransactions.TotalGiftCardTenderAmount + double.Parse(GiftCardTenderAmount);
                        _TotalTransactions.TotalEbtFoodStampTenderAmount = _TotalTransactions.TotalEbtFoodStampTenderAmount + double.Parse(EbtFoodStampTenderAmount);
                    }
                }
            }
            return _TotalTransactions;

        }
        private static ListViewRow SetListViewRowSearchRegular(SearchTypeEnum SearchType, Document foundDoc)
        {

            var ListViewRow = new ListViewRow();
            ListViewRow.SearchType = SearchTypeEnum.SearchRegular;
            ListViewRow.TransactionId = foundDoc.Get(TlogFieldsEnum.TransactionID.ToString()); ;
            ListViewRow.BusinessUnit = foundDoc.Get(TlogFieldsEnum.BusinessUnitID.ToString());
            ListViewRow.DateTime = DateTime.ParseExact(foundDoc.Get(TlogFieldsEnum.EndDateTime.ToString()), "yyyyMMdd", null).ToString("yyyy-MM-dd");
            ListViewRow.POS = foundDoc.Get(TlogFieldsEnum.WorkstationID.ToString());
            ListViewRow.Product = foundDoc.Get(TlogFieldsEnum.Product.ToString());
            var TransactionNetAmount = foundDoc.Get(TlogFieldsEnum.TransactionNetAmount.ToString());
            if (TransactionNetAmount != "null")
                ListViewRow.Totaltransactions = new TotalTransactions() { TotalTransactionsNetAmount = double.Parse(TransactionNetAmount) };
            return ListViewRow;
        }

        private static ListViewRow SetListViewRowSearchByGroup(SearchDefintion _SearchDefintion, IGroupDocs<object> Group, string ExtraData)
        {
            if (Group.GroupValue == null)
                return null;

            var ListViewRow = new ListViewRow();
            ListViewRow.SearchType = _SearchDefintion.SearchType;
            BytesRef bytes = (BytesRef)Group.GroupValue;
            ListViewRow.GroupValue = bytes.Utf8ToString();
            ListViewRow.GroupFiledId = _SearchDefintion.GroupFiledId;
            ListViewRow.TotalHits = Group.TotalHits.ToString();
            if (ExtraData != "")
                ListViewRow.Description = ExtraData;
            return ListViewRow;
        }

        private static ListViewRow SetListViewRowSumTrxNetAmount(SearchDefintion _SearchDefintion, string GroupValue, string GroupFiledId, TotalTransactions _TotalTransaction)
        {
            var ListViewRow = new ListViewRow();
            ListViewRow.SearchType = _SearchDefintion.SearchType;
            ListViewRow.GroupValue = GroupValue;
            ListViewRow.GroupFiledId = GroupFiledId;
            ListViewRow.Totaltransactions = _TotalTransaction;
            return ListViewRow;
        }

        // GetResultsByGroupBy    
        private static ITopGroups<object> GetResultsByGroupBy(string groupFiledId, IndexSearcher searcher)
        {
            Sort groupSort = new Sort();
            groupSort.SetSort(SortField.FIELD_SCORE);

            int groupOffset = 0;
            int groupLimit = 10;

            GroupingSearch groupingSerach = new GroupingSearch(groupFiledId);
            groupingSerach.SetGroupSort(groupSort);

            return groupingSerach.Search(searcher, SelectedQuery, groupOffset, groupLimit);
        }

        //get element value or attribute from tlog
        private static string GetValueFromXmlElment(XmlDocument doc, string TagName, string ExpectedAttribute)
        {
            XmlNodeList lst = doc.GetElementsByTagName(TagName);

            foreach (XmlElement elem in lst)
            {

                if (ExpectedAttribute == "")
                    return elem.InnerText;
                else
                {
                    if (elem.Attributes.Count > 0)
                    {

                        var LstAttribute = elem.Attributes;
                        foreach (XmlAttribute Attr in LstAttribute)
                        {
                            if (Attr.Name == ExpectedAttribute || Attr.Value == ExpectedAttribute)
                            {
                                if (Attr.InnerText != ExpectedAttribute)
                                    return Attr.InnerText;
                                else
                                    return elem.InnerText;
                            }
                        }
                    }
                }
            }


            return "null";
        }

        private static string GetTenderAmountFromXmlElment(XmlDocument doc, string ExpectedTenderId)
        {
            XmlNodeList lst = doc.GetElementsByTagName("Tender");
            foreach (XmlElement elem in lst)
            {
                var ChildNodesElem = elem.ChildNodes;
                foreach (XmlElement ChildNode in ChildNodesElem)
                {
                    if (ChildNode.Name == "TenderID")
                    {
                        if (ChildNode.InnerText == ExpectedTenderId)
                        {
                            return elem.FirstChild.InnerText;
                        }
                    }
                }

            }
            return "0";
        }
        private static string GetTransactionType(XmlDocument tlog)
        {
            if (GetValueFromXmlElment(tlog, "ControlTransaction", "") != "null")
                return TransactionTypeEnum.ControlTransactionLog.ToString();

            if (GetValueFromXmlElment(tlog, "CustomerOrderTransactio", "") != "null")
                return TransactionTypeEnum.CustomerOrderTransactionLog.ToString();

            if (GetValueFromXmlElment(tlog, "RetailTransaction", "") != "null")
                return TransactionTypeEnum.RetailTransactionLog.ToString();

            if (GetValueFromXmlElment(tlog, "TenderControlTransaction", "") != "null")
                return TransactionTypeEnum.FundTransferTransactionLog.ToString();

            return "null";
        }
        private static void AddSellItemsToNewDoc(XmlDocument tlog, string UnitID, string OperatorID, string WorkstationID, int EndDateTime, IndexWriter writer)
        {
            XmlNodeList Sales = tlog.GetElementsByTagName("Sale");
            XmlDocument Xmldoc = new XmlDocument();
            foreach (XmlElement elem in Sales)
            {
                Xmldoc.LoadXml("<Sale>" + elem.InnerXml + "</Sale>");
                var ItemID = GetValueFromXmlElment(Xmldoc, "ItemID", "");
                var Description = GetValueFromXmlElment(Xmldoc, "Description", "");
                Document Itemdoc = new Document();
                if (ItemID != "" && Description != "")
                {
                    Itemdoc.Add(new Field(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.Item.ToString().ToLower(), Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.Product.ToString(), ItemID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.ProductDescription.ToString(), Description, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.BusinessUnitID.ToString(), UnitID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.OperatorID.ToString(), OperatorID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.WorkstationID.ToString(), WorkstationID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Int32Field(TlogFieldsEnum.EndDateTime.ToString(), EndDateTime, Field.Store.YES));
                    writer.AddDocument(Itemdoc);
                }
            }
        }

        private static void AddSellPromotionToNewDoc(XmlDocument tlog, string UnitID, string OperatorID, string WorkstationID, int EndDateTime, IndexWriter writer)
        {
            XmlNodeList Sales = tlog.GetElementsByTagName("PromotionsSummary");
            XmlDocument Xmldoc = new XmlDocument();
            foreach (XmlElement elem in Sales)
            {
                Xmldoc.LoadXml("<PromotionsSummary>" + elem.InnerXml + "</PromotionsSummary>");
                var PromotionId = GetValueFromXmlElment(Xmldoc, "PromotionID", "");
                var PromotionDescription = GetValueFromXmlElment(Xmldoc, "PromotionDescription", "");
                var PromotionAmount = GetValueFromXmlElment(Xmldoc, "TotalRewardAmount", "");
                Document Itemdoc = new Document();
                if (PromotionId != "" && PromotionDescription != "" && PromotionDescription != "null")
                {
                    Itemdoc.Add(new Field(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.Promotion.ToString().ToLower(), Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.PromotionId.ToString(), PromotionId, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.PromotionDescription.ToString(), PromotionDescription, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.PromotionAmount.ToString(), PromotionAmount, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.BusinessUnitID.ToString(), UnitID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.OperatorID.ToString(), OperatorID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Field(TlogFieldsEnum.WorkstationID.ToString(), WorkstationID, Field.Store.YES, Field.Index.ANALYZED));
                    Itemdoc.Add(new Int32Field(TlogFieldsEnum.EndDateTime.ToString(), EndDateTime, Field.Store.YES));
                    writer.AddDocument(Itemdoc);
                }
            }
        }

    }
}