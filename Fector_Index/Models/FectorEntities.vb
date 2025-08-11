Imports System.Data.Entity
Imports System.Data.Entity.ModelConfiguration.Conventions
Imports System.Data.Entity.Infrastructure

Public Class FectorEntities
    Inherits DbContext

    Public Overridable Property UserLevels As DbSet(Of MsUserlevel)
    Public Overridable Property Branches As DbSet(Of MsBranch)
    Public Overridable Property Users As DbSet(Of MsUser)
    Public Overridable Property LogUsers As DbSet(Of LogUser)
    Public Overridable Property LogTransactions As DbSet(Of LogTransaction)
    Public Overridable Property Settings As DbSet(Of MsSetting)
    Public Overridable Property Menus As DbSet(Of MsMenu)
    Public Overridable Property SubMenus As DbSet(Of MsSubMenu)
    Public Overridable Property UserAccesses As DbSet(Of MsUserAccess)
    Public Overridable Property Accounts As DbSet(Of MsAccount)
    Public Overridable Property AccountsExtension As DbSet(Of MsAccountExtension)
    Public Overridable Property Currencies As DbSet(Of MsCurrency)
    Public Overridable Property BusinessType As DbSet(Of MsBusinessType)
    Public Overridable Property DocumentTransaction As DbSet(Of MsDocumentTransaction)
    Public Overridable Property DocumentLHBU As DbSet(Of MsDocumentLHBU)
    Public Overridable Property Countries As DbSet(Of MsCountry)
    Public Overridable Property SubjectStatus As DbSet(Of MsSubjectStatus)
    Public Overridable Property Purposes As DbSet(Of MsPurpose)
    Public Overridable Property AccountsLimit As DbSet(Of MsAccountLimit)
    Public Overridable Property OtherAccountsLimit As DbSet(Of MsOtherAccountLimit)
    Public Overridable Property OtherAccounts As DbSet(Of MsOtherAccount)
    Public Overridable Property OtherAccountsExtension As DbSet(Of MsOtherAccountExtension)
    Public Overridable Property BICodes As DbSet(Of MsBICode)
    Public Overridable Property MappingDocument As DbSet(Of MsMappingDocument)
    Public Overridable Property MappingDocumentPurpose As DbSet(Of MsMappingDocumentPurpose)
    Public Overridable Property ExtractSettingTransaction As DbSet(Of MsExtractSettingTransaction)
    Public Overridable Property ExtractSettingNonTransaction As DbSet(Of MsExtractSettingNonTransaction)
    Public Overridable Property TransactionDeal As DbSet(Of TransactionDeal)
    Public Overridable Property ExchangeTransactionHead As DbSet(Of ExchangeTransactionHead)
    Public Overridable Property ExchangeTransactionDetail As DbSet(Of ExchangeTransactionDetail)
    Public Overridable Property ExchangeRate As DbSet(Of MsRTExchangeRate)
    Public Overridable Property ExchangeRateMaster As DbSet(Of MsExchangeRate)
    Public Overridable Property Holidays As DbSet(Of MsHoliday)
    Public Overridable Property UploadNPWP As DbSet(Of MsUploadNPWP)
    Public Overridable Property AvailableMenu As DbSet(Of MsAvailableMenu)
    Public Overridable Property ExceptionUrl As DbSet(Of MsExceptionUrl)
    Public Overridable Property CoreTrx As DbSet(Of CoreTransaction)
    Public Overridable Property CoreNonTrx As DbSet(Of CoreNonTransaction)
    Public Overridable Property DocUnderlying As DbSet(Of MsDocUnderlying)
    Public Overridable Property Nostro As DbSet(Of MsNostro)
    Public Overridable Property MappingNostro As DbSet(Of MsMappingNostro)
    Public Overridable Property ExchangeTransactionDoc As DbSet(Of ExchangeTransactionDoc)
    Public Overridable Property DailyProfit As DbSet(Of DailyProfit)
    Public Overridable Property LKDerivativeType As DbSet(Of LK_DerivativeType)
    Public Overridable Property LKNettingType As DbSet(Of LK_NettingType)
    Public Overridable Property ExchangeTransactionReview As DbSet(Of ExchangeTransactionReview)
    Public Overridable Property ExchangeTransactionClose As DbSet(Of ExchangeTransactionClose)
    'Public Property ManualReconcile As DbSet(Of ManualReconcile)


    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        MyBase.OnModelCreating(modelBuilder)
        modelBuilder.Conventions.Remove(Of PluralizingTableNameConvention)()

        'modelBuilder.Entity(Of ManualReconcile).ToTable("CoreTransaction")
    End Sub

    Public Sub New()
        Database.Connection.ConnectionString = AppHelper.CS
        Dim objectContext = TryCast(Me, IObjectContextAdapter).ObjectContext
        objectContext.CommandTimeout = 600
    End Sub

End Class
