Public Class SettingController
    Inherits System.Web.Mvc.Controller

    Private Class GetCurrency_View
        Public Property CurrId As String
        Public Property CurrDisplay As String
    End Class

    '
    ' Get: /Setting/Edit
    <Authorize> _
    Public Function Edit() As ActionResult
        Dim model As New MsSettingViewModel
        Dim db As New FectorEntities

        model.BankCountryID = MsSetting.GetSetting("Bank", "CountryID", "", "1")
        model.BankSubjectStatusID = MsSetting.GetSetting("Bank", "SubjectStatusId", "", "1")
        model.BankBICode = MsSetting.GetSetting("Bank", "BICode", "", "1")
        model.BankBusinessTypeID = MsSetting.GetSetting("Bank", "BusinessTypeID", "", "1")
        model.TransactionLimit = MsSetting.GetSetting("Transaction", "Limit", "", "1")
        model.LimitCurrency = MsSetting.GetSetting("Transaction", "Limit", "", "2")
        model.BankName = MsSetting.GetSetting("Bank", "Name", "", "1")

        Dim ExchRate = (From a In db.ExchangeRate
                        Select a).Distinct.ToList

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.LimitCurrency, .CurrDisplay = model.LimitCurrency})

        For i As Integer = 0 To ExchRate.Count - 1
            ListCurr.Add(New GetCurrency_View With {.CurrId = ExchRate.Item(i).CurrId, .CurrDisplay = ExchRate.Item(i).CurrencyDisplay})
        Next

        ViewData("Title") = "Edit Setting"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")
        ViewBag.Currency = New SelectList(ListCurr, "CurrId", "CurrDisplay")
        ViewBag.Country = New SelectList(db.Countries, "CountryId", "Description")
        ViewBag.Breadcrumb = "Home > Setting > Edit"

        Return View("Detail", model)
    End Function

    '
    ' Post: /Setting/Edit
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Edit(model As MsSettingViewModel) As ActionResult
        Dim db As New FectorEntities

        Dim ExchRate = (From a In db.ExchangeRate
                       Select a).Distinct.ToList

        Dim ListCurr As New List(Of GetCurrency_View)
        ListCurr.Add(New GetCurrency_View With {.CurrId = model.LimitCurrency, .CurrDisplay = model.LimitCurrency})

        For i As Integer = 0 To ExchRate.Count - 1
            ListCurr.Add(New GetCurrency_View With {.CurrId = ExchRate.Item(i).CurrId, .CurrDisplay = ExchRate.Item(i).CurrencyDisplay})
        Next

        ViewData("Title") = "Edit Setting"
        ViewBag.Breadcrumb = "Home > Setting > Edit"
        ViewBag.BType = New SelectList(db.BusinessType, "BusinessTypeId", "DisplayValue")
        ViewBag.Status = New SelectList(db.SubjectStatus, "StatusId", "Description")
        ViewBag.Country = New SelectList(db.Countries, "CountryId", "Description")
        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim CountryId As String = model.BankCountryID
            Dim SubjectStatusId As String = model.BankSubjectStatusID
            Dim BICode As String = model.BankBICode
            Dim BusinessTypeId As String = model.BankBusinessTypeID
            Dim LimitCurrency As String = model.LimitCurrency
            Dim TransactionLimit As String = model.TransactionLimit
            Dim BankName As String = model.BankName

            Dim editedSettingCountryID As MsSetting = MsSettingViewModel.GetMsSetting("Bank", "CountryID", "", "1", db)
            With editedSettingCountryID
                .Value1 = CountryId
            End With

            Dim editedSubjectStatusID As MsSetting = MsSettingViewModel.GetMsSetting("Bank", "SubjectStatusId", "", "1", db)
            With editedSubjectStatusID
                .Value1 = SubjectStatusId
            End With

            Dim editedBICode As MsSetting = MsSettingViewModel.GetMsSetting("Bank", "BICode", "", "1", db)
            With editedBICode
                .Value1 = BICode
            End With

            Dim editedBusinessTypeID As MsSetting = MsSettingViewModel.GetMsSetting("Bank", "BusinessTypeId", "", "1", db)
            With editedBusinessTypeID
                .Value1 = BusinessTypeId
            End With

            Dim editedLimitTransaction As MsSetting = MsSettingViewModel.GetMsSetting("Transaction", "Limit", "", "1", db)
            With editedLimitTransaction
                .Value1 = TransactionLimit.Substring(0, TransactionLimit.Length - 3).Replace(",", "").Replace(".", "")
            End With

            Dim editedLimitCurrency As MsSetting = MsSettingViewModel.GetMsSetting("Transaction", "Limit", "", "2", db)
            With editedLimitCurrency
                .Value2 = LimitCurrency
            End With

            Dim editedBankName As MsSetting = MsSettingViewModel.GetMsSetting("Bank", "Name", "", "1", db)
            With editedBankName
                .Value1 = BankName
            End With


            db.Entry(editedSettingCountryID).State = EntityState.Modified
            db.Entry(editedSubjectStatusID).State = EntityState.Modified
            db.Entry(editedBICode).State = EntityState.Modified
            db.Entry(editedBusinessTypeID).State = EntityState.Modified
            db.Entry(editedLimitTransaction).State = EntityState.Modified
            db.Entry(editedLimitCurrency).State = EntityState.Modified
            db.Entry(editedBankName).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT SETTING", "BANK", db)

            Return RedirectToAction("Index", "Home")
        Else
            Return View("Detail", model)
        End If
    End Function

    '
    ' Get: /Setting/Security
    <Authorize> _
    Public Function Security() As ActionResult
        Dim model As New MsSettingSecurityViewModel
        Dim db As New FectorEntities

        model.LoginTimeout = MsSetting.GetSetting("LoginTimeout", "", "", "1")
        model.MinPassword = MsSetting.GetSetting("Password", "Min", "Char", "1")
        model.MaxPassword = MsSetting.GetSetting("Password", "Max", "Char", "1")
        model.MaxIncorrect = MsSetting.GetSetting("Password", "Incorrect", "", "1")
        model.PasswordExpired = MsSetting.GetSetting("Password", "ChangeTime", "Days", "1")

        ViewData("Title") = "Edit Security Setting"
        ViewBag.Breadcrumb = "Home > Setting > Security Edit"

        Return View("Security", model)
    End Function

    '
    ' Post: /Setting/Security
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Public Function Security(model As MsSettingSecurityViewModel) As ActionResult
        Dim db As New FectorEntities

        ViewBag.ErrorMessage = ""

        If ModelState.IsValid Then
            Dim LoginTimeout As Integer = model.LoginTimeout
            Dim MinPassword As Integer = model.MinPassword
            Dim MaxPassword As Integer = model.MaxPassword
            Dim MaxIncorrect As Integer = model.MaxIncorrect
            Dim PasswordExpired As Integer = model.PasswordExpired

            Dim editedSettingLoginTimeout As MsSetting = MsSettingViewModel.GetMsSetting("LoginTimeout", "", "", "1", db)
            With editedSettingLoginTimeout
                .Value1 = LoginTimeout
            End With

            Dim editedSettingMinPassword As MsSetting = MsSettingViewModel.GetMsSetting("Password", "Min", "Char", "1", db)
            With editedSettingMinPassword
                .Value1 = MinPassword
            End With

            Dim editedSettingMaxPassword As MsSetting = MsSettingViewModel.GetMsSetting("Password", "Max", "Char", "1", db)
            With editedSettingMaxPassword
                .Value1 = MaxPassword
            End With

            Dim editedSettingMaxIncorrect As MsSetting = MsSettingViewModel.GetMsSetting("Password", "Incorrect", "", "1", db)
            With editedSettingMaxIncorrect
                .Value1 = MaxIncorrect
            End With

            Dim editedSettingPasswordExpired As MsSetting = MsSettingViewModel.GetMsSetting("Password", "ChangeTime", "Days", "1", db)
            With editedSettingPasswordExpired
                .Value1 = PasswordExpired
            End With


            db.Entry(editedSettingLoginTimeout).State = EntityState.Modified
            db.Entry(editedSettingMinPassword).State = EntityState.Modified
            db.Entry(editedSettingMaxPassword).State = EntityState.Modified
            db.Entry(editedSettingMaxIncorrect).State = EntityState.Modified
            db.Entry(editedSettingPasswordExpired).State = EntityState.Modified
            db.SaveChanges()

            LogTransaction.WriteLog(User.Identity.Name, "EDIT SETTING", "SECURITY", db)

            Return RedirectToAction("Index", "Home")
        Else
            Return View("Security", model)
        End If
    End Function
End Class