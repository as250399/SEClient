using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEClient.Helper
{
    public enum SearchTypeEnum
    {
        SearchByGroup,
        SearchRegular,
        SearchByJoint,
        SearchRegularSumTrx,
        SearchRegularCheckTotalAmount,
    }

    public enum TlogFieldsEnum
    {
        TareQuantity,
        BusinessUnitID,
        OperatorID,
        TransactionID,
        WorkstationID,
        EndDateTime,
        TransactionType,
        Product,
        ProductDescription,
        TransactionStatus,
        OperatorLock,
        TransactionNetAmount,
        TransactionTaxAmount,
        TenderCash,
        TenderVisa,
        TenderAmex,
        TenderDebit,
        TenderGiftCard,
        TenderEbtFoodStamp,
        TenderEwic,
        PromotionId,
        PromotionDescription,
        PromotionAmount,
    }

    public enum TransactionTypeEnum
    {
        ControlTransactionLog,
        CustomerOrderTransactionLog,
        RetailTransactionLog,
        FundTransferTransactionLog,
        Item,
        Promotion,
    }
}