Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema

Public Class MsMenu
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property MenuId As Integer

    Public Property Description As String
    Public Property Ordering As Integer
    Public Property FlagActive As Boolean

    Public Shared Function GetYetReconsile(ByVal HOBranch As String, ByVal UserBranch As String, ByVal db As FectorEntities) As Integer
        If UserBranch = HOBranch Then
            Return (From a In db.ExchangeTransactionDetail
                   Join b In db.ExchangeTransactionHead On a.TransNum Equals b.TransNum
                   Join c In db.TransactionDeal On a.DealNumber Equals c.DealNumber
                   Where a.FlagReconcile = 0 And b.Status = "ACTIVE" And c.Status <> "CLOSED"
                   Select New With {.TransNum = a.TransNum, _
                                                 .DealNumber = a.DealNumber, _
                                                 .AccNum = b.AccNum, _
                                                 .AccName = b.AccName, _
                                                 .RateType = b.RateType, _
                                                 .TransactionType = b.TransactionType, _
                                                 .TransactionCurrency = a.TransactionCurrency, _
                                                 .TransactionRate = a.TransactionRate, _
                                                 .TransactionNominal = a.TransactionNominal, _
                                                 .CustomerCurrency = a.CustomerCurrency, _
                                                 .CustomerNominal = a.CustomerNominal, _
                                                 .ValuePeriod = a.ValuePeriod, _
                                                 .ValueDate = a.ValueDate, _
                                                 .FlagReconcile = a.FlagReconcile, _
                                                 .SourceFunds = b.SourceFunds, _
                                                 .SourceAccNum = b.SourceAccNum, _
                                                 .SourceAccName = b.SourceAccName, _
                                                 .SourceNominal = b.SourceNominal}).Count
        Else
            Return (From a In db.ExchangeTransactionDetail
                   Join b In db.ExchangeTransactionHead On a.TransNum Equals b.TransNum
                   Join c In db.TransactionDeal On a.DealNumber Equals c.DealNumber
                   Where a.FlagReconcile = 0 And b.BranchId = UserBranch And b.Status = "ACTIVE" And c.Status <> "CLOSED"
                   Select New With {.TransNum = a.TransNum, _
                                                 .DealNumber = a.DealNumber, _
                                                 .AccNum = b.AccNum, _
                                                 .AccName = b.AccName, _
                                                 .RateType = b.RateType, _
                                                 .TransactionType = b.TransactionType, _
                                                 .TransactionCurrency = a.TransactionCurrency, _
                                                 .TransactionRate = a.TransactionRate, _
                                                 .TransactionNominal = a.TransactionNominal, _
                                                 .CustomerCurrency = a.CustomerCurrency, _
                                                 .CustomerNominal = a.CustomerNominal, _
                                                 .ValuePeriod = a.ValuePeriod, _
                                                 .ValueDate = a.ValueDate, _
                                                 .FlagReconcile = a.FlagReconcile, _
                                                 .SourceFunds = b.SourceFunds, _
                                                 .SourceAccNum = b.SourceAccNum, _
                                                 .SourceAccName = b.SourceAccName, _
                                                 .SourceNominal = b.SourceNominal}).Count
        End If
    End Function

    Public Shared Function GetYetReconsile(ByVal UserBranch As String, ByVal db As FectorEntities) As Integer
        Return ReconcileViewModel.GetTodayPendingTransaction(db, UserBranch).Count
    End Function

    Public Shared Function GetUndoneTransaction(ByVal HOBranch As String, ByVal UserBranch As String, ByVal db As FectorEntities) As Integer
        Dim CountUndoneTransaction As Integer = 0

        If UserBranch = HOBranch Then
            Return (From s In db.ExchangeTransactionDetail
                    Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                    Join u In db.TransactionDeal On s.DealNumber Equals u.DealNumber
                    Where s.FlagReconcile = 0 And t.Status <> "INACTIVE" And u.Status <> "CLOSED"
                    Select New With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                        .AccNum = t.AccNum, .AccName = t.AccName, _
                                                        .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranch}).Count
        Else
            Return (From s In db.ExchangeTransactionDetail
                    Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                    Join u In db.TransactionDeal On s.DealNumber Equals u.DealNumber
                    Where t.BranchId = UserBranch And s.FlagReconcile = 0 And t.Status <> "INACTIVE" And u.Status <> "CLOSED"
                    Select New With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                        .AccNum = t.AccNum, .AccName = t.AccName, _
                                                        .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranch}).Count
        End If
    End Function

    Public Shared Function GetNonNPWP(ByVal HOBranch As String, ByVal UserBranch As String, ByVal db As FectorEntities) As Integer
        Dim CountNonNPWP As Integer = 0

        Dim notNPWP As String = ("0").ToString.PadLeft(15, "0")

        If UserBranch = HOBranch Then
            Return ((From a In db.Accounts
                         Join b In db.ExchangeTransactionHead On a.AccNo Equals b.AccNum
                         Group Join c In db.UploadNPWP On a.AccNo Equals c.AccNum Into ac = Group
                         Where b.DocumentStatementOverlimitLink IsNot Nothing And b.DocumentStatementOverlimitLink <> "" And a.Status = "ACTIVE" And ac.FirstOrDefault.FileDirectory Is Nothing
                         Select New With {.AccNo = a.AccNo, _
                                            .AccName = a.Name, _
                                            .CIF = a.CIF, _
                                            .FileDirectory = ac.FirstOrDefault.FileDirectory, _
                                            .Status = If(ac.FirstOrDefault.FileDirectory Is Nothing, "NOT UPLOADED", "UPLOADED")}).Distinct).count
        Else
            Return ((From a In db.Accounts
                        Join b In db.ExchangeTransactionHead On a.AccNo Equals b.AccNum
                        Group Join c In db.UploadNPWP On a.AccNo Equals c.AccNum Into ac = Group
                        Where b.DocumentStatementOverlimitLink IsNot Nothing And b.DocumentStatementOverlimitLink <> "" And a.Status = "ACTIVE" And ac.FirstOrDefault.FileDirectory Is Nothing And b.BranchId = UserBranch
                        Select New With {.AccNo = a.AccNo, _
                                            .AccName = a.Name, _
                                            .CIF = a.CIF, _
                                            .FileDirectory = ac.FirstOrDefault.FileDirectory, _
                                            .Status = If(ac.FirstOrDefault.FileDirectory Is Nothing, "NOT UPLOADED", "UPLOADED")}).Distinct).count
        End If
    End Function

    Public Shared Function GetUnapprovalTransaction(ByVal HOBranch As String, ByVal UserBranch As String, ByVal Username As String, ByVal db As FectorEntities) As Integer
        If UserBranch = HOBranch Then
            Return (From s In db.ExchangeTransactionDetail
                            Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                            Join u In db.TransactionDeal On s.DealNumber Equals u.DealNumber
                            Where t.Status.Contains("PENDING") And t.EditBy <> Username And s.FlagReconcile = 0 And u.Status <> "CLOSED"
                            Select New With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                            .AccNum = t.AccNum, .AccName = t.AccName, _
                                            .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranch}).count
        Else
            Return (From s In db.ExchangeTransactionDetail
                    Group Join t In db.ExchangeTransactionHead On s.TransNum Equals t.TransNum Into Group From t In Group.DefaultIfEmpty
                    Join u In db.TransactionDeal On s.DealNumber Equals u.DealNumber
                    Where t.Status.Contains("PENDING") And t.EditBy <> Username And t.BranchId = UserBranch And s.FlagReconcile = 0 And u.Status <> "CLOSED"
                    Select New With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                    .AccNum = t.AccNum, .AccName = t.AccName, _
                                    .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .Status = t.Status, .BranchId = UserBranch}).count
        End If
    End Function

    Public Shared Function GetUnreviewTransaction(ByVal HOBranch As String, ByVal UserBranch As String, ByVal db As FectorEntities) As Integer
        Dim tmp = From s In db.ExchangeTransactionDetail.AsNoTracking
                    Join t In db.ExchangeTransactionHead.AsNoTracking On s.TransNum Equals t.TransNum
                    Group Join v In db.ExchangeTransactionReview.AsNoTracking On s.TransNum Equals v.TransNum Into sv = Group
                    Group Join u In db.ExchangeTransactionClose.AsNoTracking On s.TransNum Equals u.TransNum Into su = Group
                    Join w In db.TransactionDeal.AsNoTracking On s.DealNumber Equals w.DealNumber
                    Where su.FirstOrDefault.DealNumber Is Nothing And w.Status <> "CLOSED"
                    Order By s.DealNumber
                    Select New With {.TransNum = s.TransNum, .DealNumber = s.DealNumber, .TDate = t.TDate, _
                                                .AccNum = t.AccNum, .AccName = t.AccName, _
                                                .TransactionType = t.TransactionType, .RateType = t.RateType, .SourceFunds = t.SourceFunds, .FlagReconsile = s.FlagReconcile, _
                                                .Status = If(CStr(sv.FirstOrDefault.FlagReview) Is Nothing, 0, sv.FirstOrDefault.FlagReview), .BranchId = UserBranch}


        If UserBranch = HOBranch Then
            Return (From s In tmp
                    Where s.Status = 0
                    Select s).Count
        Else
            Return (From s In tmp
                    Where s.Status = 0 And s.BranchId = UserBranch
                    Select s).Count
        End If
    End Function
End Class

Public Class MsSubMenu
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property SubMenuId As Integer

    <ForeignKey("Menu")> _
    Public Property MenuId As Integer
    Public Overridable Property Menu As MsMenu

    Public Property Description As String
    Public Property Link As String
    Public Property Ordering As Integer
    Public Property FlagActive As Boolean
End Class

Public Class MsUserAccess
    <Key> _
    <Column(order:=1)> _
    <ForeignKey("UserLevel")> _
    Public Property UserLevelId As Integer
    Public Overridable Property UserLevel As MsUserlevel

    <Key> _
    <Column(order:=2)> _
    <ForeignKey("SubMenu")> _
    Public Property SubMenuId As Integer
    Public Overridable Property SubMenu As MsMenu

    Public Class MenuName
        Public Property SubMenuId As String
        Public Property MenuId As String
        Public Property MenuName As String
    End Class

    Public Shared Function GetMenuName(RequestURL As String) As IQueryable(Of MenuName)
        Dim db As New FectorEntities()
        Dim menu = From m In db.Menus
                   Join s In db.SubMenus On m.MenuId Equals s.MenuId
                   Where m.FlagActive = True And RequestURL.Contains("/" & s.Link)
                   Select New MenuName With {.SubMenuId = s.SubMenuId, .MenuId = m.MenuId, .MenuName = m.Description}

        Return menu
    End Function

    Public Shared Function GetMenus(UserLevelId As Integer) As IQueryable(Of MsMenu)
        Dim db As New FectorEntities
        Dim menu = (From m In db.Menus
                   Join s In db.SubMenus On m.MenuId Equals s.MenuId
                   Join uac In db.UserAccesses On uac.SubMenuId Equals s.SubMenuId
                   Where m.FlagActive = True And uac.UserLevelId = UserLevelId
                   Order By m.Ordering
                   Select m).Distinct
        Return menu
    End Function

    Public Shared Function GetSubMenus(UserLevelId As Integer, MenuId As Integer) As IQueryable(Of MsSubMenu)
        Dim db As New FectorEntities()
        Dim submenu = From s In db.SubMenus
                   Join uac In db.UserAccesses On uac.SubMenuId Equals s.SubMenuId
                   Where s.FlagActive = True And uac.UserLevelId = UserLevelId And s.MenuId = MenuId
                   Order By s.Ordering
                   Select s
        submenu.OrderBy(Function(f) f.Ordering)
        Return submenu
    End Function

    Public Shared Function GetUAC(ByVal UserLevelId As String, ByVal db As FectorEntities) As List(Of MsUserAccess)
        Return db.UserAccesses.Where(Function(f) f.UserLevelId = UserLevelId).ToList
    End Function
End Class

