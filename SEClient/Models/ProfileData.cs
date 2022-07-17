using Lucene.Net.Index;
using Lucene.Net.Search;
using SEClient.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Models
{
    public static class ProfileData
    {
        public static void SetProfileData(string index, string _businessUnitId, string _FromDate, string _EndDate, string _FreeText)
        {
            var BooleanQuerySelectedQuery = new BooleanQuery();
            var ExpectedbusinessUnitId = _businessUnitId;
            if (ExpectedbusinessUnitId == "0")
                ExpectedbusinessUnitId = "*";


            switch (index)
            {
                case "0":
                    {
                        Query query1 = NumericRangeQuery.NewDoubleRange(TlogFieldsEnum.TareQuantity.ToString(), 0, double.Parse(_FreeText), true, true);
                        Query query2 = new TermQuery(new Term(TlogFieldsEnum.OperatorID.ToString(), "null"));
                        Query query3 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query4 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        Query query5 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST_NOT);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST_NOT);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query4, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query5, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = "OperatorID" };
                        break;
                    }
                case "1":
                    {
                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = new TermQuery(new Term(TlogFieldsEnum.TransactionStatus.ToString(), "Returned".ToLower()));
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.OperatorID.ToString(), "null"));
                        Query query4 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        Query query5 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST_NOT);
                        BooleanQuerySelectedQuery.Add(query4, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query5, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.OperatorID.ToString() };
                        break;
                    }
                case "2":
                    {
                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.Product.ToString(), _FreeText));

                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.Product.ToString() };
                        break;
                    }
                case "3":
                    {
                        Query query1 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.FundTransferTransactionLog.ToString().ToLower().ToLower()));
                        Query query2 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query3 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchRegular };
                        break;
                    }
                case "4":
                    {
                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.BusinessUnitID.ToString() };
                        break;
                    }
                case "5":
                    {
                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.OperatorID.ToString() };
                        break;
                    }
                case "6":
                    {
                        Query query1 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.ControlTransactionLog.ToString().ToLower()));
                        Query query2 = new TermQuery(new Term(TlogFieldsEnum.OperatorLock.ToString(), TlogFieldsEnum.OperatorLock.ToString().ToLower()));
                        Query query3 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query4 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query4, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.OperatorID.ToString() };
                        break;
                    }
                case "7":
                    {

                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchRegularSumTrx, GroupFiledId = TlogFieldsEnum.BusinessUnitID.ToString() };
                        break;
                    }
                case "8":
                    {

                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.Item.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.Product.ToString() };
                        break;
                    }
                case "9":
                    {

                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.Promotion.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchByGroup, GroupFiledId = TlogFieldsEnum.PromotionId.ToString() };
                        break;
                    }
                case "10":
                    {

                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchRegularCheckTotalAmount, GroupFiledId = "HighestAmount" };
                        break;
                    }
                case "11":
                    {

                        Query query1 = new WildcardQuery(new Term(TlogFieldsEnum.BusinessUnitID.ToString(), ExpectedbusinessUnitId));
                        Query query2 = NumericRangeQuery.NewInt32Range(TlogFieldsEnum.EndDateTime.ToString(), int.Parse(_FromDate), int.Parse(_EndDate), true, true);
                        Query query3 = new TermQuery(new Term(TlogFieldsEnum.TransactionType.ToString(), TransactionTypeEnum.RetailTransactionLog.ToString().ToLower()));
                        BooleanQuerySelectedQuery.Add(query1, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query2, Occur.MUST);
                        BooleanQuerySelectedQuery.Add(query3, Occur.MUST);
                        SerachEngineControler.SelectedQuery = BooleanQuerySelectedQuery;
                        SerachEngineControler._SearchDefintion = new SearchDefintion { SearchType = SearchTypeEnum.SearchRegularCheckTotalAmount, GroupFiledId = "LowestAmount" };
                        break;
                    }
                default:
                    // code block
                    break;
            }
        }
    }
}