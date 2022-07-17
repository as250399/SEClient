using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Contract
{
    public class TotalTransactions
    {
        public double TotalTransactionsNetAmount { get; set; } = 0.00;
        public double TotalTransactionsTaxAmount { get; set; } = 0.00;
        public double TotalCashTenderAmount { get; set; } = 0.00;
        public double TotalEwicTenderAmount { get; set; } = 0.00;
        public double TotalVisaTenderAmount { get; set; } = 0.00;
        public double TotalDebitTenderAmount { get; set; } = 0.00;
        public double TotalAmexTenderAmount { get; set; } = 0.00;
        public double TotalGiftCardTenderAmount { get; set; } = 0.00;
        public double TotalEbtFoodStampTenderAmount { get; set; } = 0.00;
    }
}