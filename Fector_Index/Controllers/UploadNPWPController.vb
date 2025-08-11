Imports System.IO

Public Class UploadNPWPController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /UploadNPWP
    Function Index() As ActionResult
        Dim model As New MsUploadNPWPView
        Return View()
    End Function

    <HttpPost> _
    Function Index(model As MsUploadNPWPView) As ActionResult
        If model.File.ContentLength > 0 Then
            Dim filenamesplit As String() = model.File.FileName.Split(".")
            Dim filename As String = "NPWP - " & model.accno & "." & filenamesplit(filenamesplit.Length - 1)

            'Dim filename As String = model.File.FileName
            'Dim Extension As String = Right(filename, 3)
            'filename = "NPWP - " & model.accno & "." & Extension

            If model.LastFileDirectory IsNot Nothing Then
                Dim FilePath As String = model.LastFileDirectory

                FilePath = UrlHelper.GenerateContentUrl(FilePath, Me.HttpContext)
                Dim GetFile As New FileInfo(Server.MapPath(FilePath))
                GetFile.Delete()
            End If

            Dim Path As String = Server.MapPath("~/Uploads/NPWP/")

            If Not IO.Directory.Exists(Path) Then
                IO.Directory.CreateDirectory(Path)
            End If

            Dim NewNPWP As New MsUploadNPWP
            Dim db As New FectorEntities

            Dim DeleteNPWP = db.UploadNPWP.Where(Function(f) f.AccNum = model.accno).FirstOrDefault
            If DeleteNPWP IsNot Nothing Then
                db.UploadNPWP.Remove(DeleteNPWP)
                db.SaveChanges()
            End If

            With NewNPWP
                .AccNum = model.accno
                .FileDirectory = "~/Uploads/NPWP/" & filename
            End With
            db.UploadNPWP.Add(NewNPWP)
            db.SaveChanges()
            LogTransaction.WriteLog(User.Identity.Name, "UPLOAD NPPWP", filename, db)

            model.File.SaveAs(Path & filename)
        End If

        Return RedirectToAction("Index")
    End Function

    <HttpPost> _
    Sub ViewFile(FilePath As String)
        System.IO.File.ReadAllText(FilePath)
    End Sub

    Private Class AccMustUploadNPWP
        Public AccNo As String
        Public CIF As String
        Public AccName As String
        Public Status As String
        Public FileDirectory As String
    End Class

    Private Class AccInTrans
        Public AccNo As String
    End Class

    <Authorize> _
    <HttpGet> _
    Public Function DTAccountNonNPWPAjaxHandler(param As jQueryDataTableParamModel) As JsonResult
        Dim db As New FectorEntities()

        Dim AccInTransaction = (From t In db.ExchangeTransactionHead
                               Select New AccInTrans With {.AccNo = t.AccNum}).ToList

        Dim notNPWP As String = ("0").ToString.PadLeft(15, "0")

        'Dim AccNoHaveNPWP = From t In db.ExchangeTransactionHead
        '                    Group Join q In db.UploadNPWP On t.AccNum Equals q.AccNum Into Group From q In Group.DefaultIfEmpty
        '                    Join u In db.Accounts On t.AccNum Equals u.AccNo
        '                    Where ((t.DocumentStatementOverlimitLink IsNot Nothing And t.DocumentStatementOverlimitLink <> ""))
        '                    Select New AccMustUploadNPWP With {.AccNo = t.AccNum, _
        '                                                       .AccName = t.AccName, _
        '                                                       .CIF = u.CIF, _
        '                                                       .FileDirectory = q.FileDirectory, _
        '                                                       .Status = If(q.FileDirectory Is Nothing, _
        '                                                                    "NOT UPLOADED", "UPLOADED")}

        Dim HeadBranch = db.Settings.Where(Function(f) f.Key1 = "Bank" And f.Key2 = "BranchId").FirstOrDefault.Value1
        Dim UserBranchID As Decimal = Session("BranchId")

        If UserBranchID = HeadBranch Then
            Dim AccNonNPWP = (From a In db.Accounts
                         Join b In db.ExchangeTransactionHead On a.AccNo Equals b.AccNum
                         Group Join c In db.UploadNPWP On a.AccNo Equals c.AccNum Into ac = Group
                         Where b.DocumentStatementOverlimitLink IsNot Nothing And b.DocumentStatementOverlimitLink <> "" And a.Status = "ACTIVE"
                         Select New AccMustUploadNPWP With {.AccNo = a.AccNo, _
                                                            .AccName = a.Name, _
                                                            .CIF = a.CIF, _
                                                            .FileDirectory = ac.FirstOrDefault.FileDirectory, _
                                                            .Status = If(ac.FirstOrDefault.FileDirectory Is Nothing, "NOT UPLOADED", "UPLOADED")}).Distinct

            Return ReturnAccountNonNPWPDataTable(param, AccNonNPWP)
        Else
            Dim AccNonNPWP = (From a In db.Accounts
                         Join b In db.ExchangeTransactionHead On a.AccNo Equals b.AccNum
                         Group Join c In db.UploadNPWP On a.AccNo Equals c.AccNum Into ac = Group
                         Where b.DocumentStatementOverlimitLink IsNot Nothing And b.DocumentStatementOverlimitLink <> "" And a.Status = "ACTIVE" And b.BranchId = UserBranchID
                         Select New AccMustUploadNPWP With {.AccNo = a.AccNo, _
                                                            .AccName = a.Name, _
                                                            .CIF = a.CIF, _
                                                            .FileDirectory = ac.FirstOrDefault.FileDirectory, _
                                                            .Status = If(ac.FirstOrDefault.FileDirectory Is Nothing, "NOT UPLOADED", "UPLOADED")}).Distinct

            Return ReturnAccountNonNPWPDataTable(param, AccNonNPWP)
        End If
    End Function

    Private Function ReturnAccountNonNPWPDataTable(param As jQueryDataTableParamModel, accounts As IQueryable(Of AccMustUploadNPWP)) As JsonResult
        Dim totalRecords As Integer = accounts.Count
        Dim displayRecord As Integer = 0

        Dim iDisplayLength = Integer.Parse(param.iDisplayLength)
        Dim iDisplayStart = Integer.Parse(param.iDisplayStart)

        Dim accnoSearch As String = If(IsNothing(Request("sSearch_0")), "", Request("sSearch_0"))
        Dim cifsearch As String = If(IsNothing(Request("sSearch_1")), "", Request("sSearch_1"))
        Dim accNameSearch As String = If(IsNothing(Request("sSearch_2")), "", Request("sSearch_2"))
        Dim statusSearch As String = If(IsNothing(Request("sSearch_3")), "", Request("sSearch_3"))
        accounts = accounts.Where(Function(f) f.AccNo.Contains(accnoSearch))
        accounts = accounts.Where(Function(f) f.CIF.Contains(cifsearch))
        accounts = accounts.Where(Function(f) f.AccName.Contains(accNameSearch))

        If statusSearch <> "" Then
            accounts = accounts.Where(Function(f) f.Status.StartsWith(statusSearch))
        Else
            accounts = accounts.Where(Function(f) f.Status = "NOT UPLOADED")
        End If

        'displayRecord = accounts.Count

        'Detection of sorted column
        Dim sortOrder As String = Request("sSortDir_0")
        Select Case Request("iSortCol_0")
            Case 0
                If sortOrder = "asc" Then
                    accounts = accounts.OrderBy(Function(f) f.AccNo)
                Else
                    accounts = accounts.OrderByDescending(Function(f) f.AccNo)
                End If
            Case 1
                If sortOrder = "asc" Then
                    accounts = accounts.OrderBy(Function(f) f.AccName)
                Else
                    accounts = accounts.OrderByDescending(Function(f) f.AccName)
                End If
        End Select

        Dim result As New List(Of String())
        For Each acc As AccMustUploadNPWP In accounts
            result.Add({acc.AccNo, acc.CIF, acc.AccName, acc.Status, If(acc.FileDirectory Is Nothing Or acc.FileDirectory = "", "NONE", UrlHelper.GenerateContentUrl(acc.FileDirectory, Me.HttpContext)), ""})
        Next

        Return Json(New With {.sEcho = param.sEcho,
                      .iTotalRecords = totalRecords,
                      .iTotalDisplayRecords = result.Count(),
                      .aaData = result.Skip(iDisplayStart).Take(iDisplayLength)},
                      JsonRequestBehavior.AllowGet)
    End Function

End Class