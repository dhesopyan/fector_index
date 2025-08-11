Imports System.Net
Imports System.Data.Objects.SqlClient
Imports System.IO
Imports System.Data.Entity.Validation
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Data.Objects


Public Class ReconcileController
    Inherits System.Web.Mvc.Controller

    ' GET: /Reconcile/Index
    <Authorize> _
    Function Index() As ActionResult
        Dim model As New ReconcileViewModel
        Dim db As New FectorEntities

        model.PendingTransaction = ReconcileViewModel.GetTodayPendingTransaction(db, Session("BranchId"))
        model.CoreUnmapped = ReconcileViewModel.GetCoreTransaction(db, Session("BranchId"))

        ViewData("Title") = "Transaction Reconcile"
        ViewBag.Breadcrumb = "Home > Transaction Reconcile"

        Return View("Index", model)
    End Function

    '
    ' POST: /Reconcile/DeleteCore
    <Authorize> _
    <HttpPost> _
    Public Function DeleteCore() As ActionResult
        Dim db As New FectorEntities()
        Dim id As String = Request.Form("DelCoreRefno")
        If IsNothing(id) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedcoretrx = db.CoreTrx.Find(id)

        If IsNothing(editedcoretrx) Then
            Return New HttpNotFoundResult
        End If

        editedcoretrx.Status = 0
        db.Entry(editedcoretrx).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "DELETE CORE TRX", editedcoretrx.Refno, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /Reconcile/ReconcileTrx
    <Authorize> _
    <HttpPost> _
    Public Function ReconcileTrx() As ActionResult
        Dim db As New FectorEntities()
        Dim corerefno As String = Request.Form("RecCoreRefno")
        Dim fectortransnum As String = Request.Form("FectorTransNum")
        Dim fectordealnum As String = Request.Form("FectorDealNum")
        If IsNothing(corerefno) OrElse IsNothing(fectortransnum) OrElse IsNothing(fectordealnum) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedcoretrx = db.CoreTrx.Find(corerefno)
        Dim editedexchangedetail = db.ExchangeTransactionDetail.Where(Function(f) f.TransNum = fectortransnum And f.DealNumber = fectordealnum).SingleOrDefault

        If IsNothing(editedcoretrx) Or IsNothing(editedexchangedetail) Then
            Return New HttpNotFoundResult
        End If

        editedcoretrx.ExchangeTransactionNumber = fectortransnum
        editedcoretrx.DealNumber = fectordealnum
        db.Entry(editedcoretrx).State = EntityState.Modified
        db.SaveChanges()

        editedexchangedetail.FlagReconcile = 1
        db.Entry(editedexchangedetail).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "RECONCILE CORE TRX", editedcoretrx.Refno, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /Reconcile/UnreconcileTrx
    <Authorize> _
    <HttpPost> _
    Public Function UnreconcileTrx() As ActionResult
        Dim db As New FectorEntities()
        Dim corerefno As String = Request.Form("UnmCoreRefno")
        If IsNothing(corerefno) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedcoretrx = db.CoreTrx.Where(Function(f) f.Refno = corerefno And f.TDate = EntityFunctions.TruncateTime(DateTime.Now) And f.ExchangeTransactionNumber.HasValue).SingleOrDefault
        
        If IsNothing(editedcoretrx) Then
            ViewBag.ErrorMessage = "Ref # cannot be found in today reconciled transaction"

            Dim model As New ReconcileViewModel
            model.PendingTransaction = ReconcileViewModel.GetTodayPendingTransaction(db, Session("BranchId"))
            model.CoreUnmapped = ReconcileViewModel.GetCoreTransaction(db, Session("BranchId"))

            ViewData("Title") = "Transaction Reconcile"
            ViewBag.Breadcrumb = "Home > Transaction Reconcile"

            Return View("Index", model)
        End If

        Dim editedexchangedetail = db.ExchangeTransactionDetail.Where(Function(f) f.TransNum = editedcoretrx.ExchangeTransactionNumber And f.DealNumber = editedcoretrx.DealNumber).SingleOrDefault

        editedcoretrx.ExchangeTransactionNumber = Nothing
        editedcoretrx.DealNumber = Nothing
        db.Entry(editedcoretrx).State = EntityState.Modified
        db.SaveChanges()

        editedexchangedetail.FlagReconcile = 0
        db.Entry(editedexchangedetail).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "UNRECONCILE CORE TRX", editedcoretrx.Refno, db)

        Return RedirectToAction("Index")
    End Function

    '
    ' POST: /Reconcile/Reactivate
    <Authorize> _
    <HttpPost> _
    Public Function Reactivate() As ActionResult
        Dim db As New FectorEntities()
        Dim corerefno As String = Request.Form("ReaCoreRefno")
        If IsNothing(corerefno) Then
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

        Dim editedcoretrx = db.CoreTrx.Find(corerefno)

        If IsNothing(editedcoretrx) Then
            ViewBag.ErrorMessage = "Ref # cannot be found"

            Dim model As New ReconcileViewModel
            model.PendingTransaction = ReconcileViewModel.GetTodayPendingTransaction(db, Session("BranchId"))
            model.CoreUnmapped = ReconcileViewModel.GetCoreTransaction(db, Session("BranchId"))

            ViewData("Title") = "Transaction Reconcile"
            ViewBag.Breadcrumb = "Home > Transaction Reconcile"

            Return View("Index", model)
        End If

        editedcoretrx.Status = 1
        db.Entry(editedcoretrx).State = EntityState.Modified
        db.SaveChanges()

        LogTransaction.WriteLog(User.Identity.Name, "REACTIVATE CORE TRX", editedcoretrx.Refno, db)

        Return RedirectToAction("Index")
    End Function
End Class