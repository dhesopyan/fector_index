Imports System.Data.SqlClient

Public Class ExtractDataManualController
    Inherits System.Web.Mvc.Controller

    '
    ' GET: /ExtractDataManual

    Function Index() As ActionResult
        Dim model As New ExtractDataManualViewModel
        model.TDate = Now.Date

        ViewBag.Breadcrumb = String.Format("Home > Extract Data Manual")
        Return View("Index", model)
    End Function

    Private Class JobStatus
        Public JobName As String
    End Class

    '
    ' POST: /ExtractDataManual
    <HttpPost> _
    <ValidateAntiForgeryToken> _
    <Authorize> _
    Function Index(model As ExtractDataManualViewModel) As ActionResult
        Dim db As New FectorEntities

        Try
            Dim CheckStatus = db.Database.SqlQuery(Of JobStatus)("exec msdb.dbo.sp_help_job @execution_status=1")

            If CheckStatus.Count > 0 Then
                For i As Integer = 0 To CheckStatus.Count - 1
                    If CheckStatus(i).JobName = "Fector Download Transaction" Then
                        If db.Database.ExecuteSqlCommand("EXEC msdb.dbo.sp_stop_job @job_name = 'Fector Download Transaction'") Then
                            If db.Database.ExecuteSqlCommand("EXEC msdb.dbo.sp_start_job @job_name = 'Fector Download Transaction'") Then
                                ModelState.AddModelError("TDate", "The extract data is success")
                            Else
                                ModelState.AddModelError("TDate", "The extract data is failed, extract data on progress. Please Wait for a moment")
                            End If
                        Else
                            ModelState.AddModelError("TDate", "The extract data is failed to stop")
                        End If
                        Exit For
                    End If
                Next
            Else
                If db.Database.ExecuteSqlCommand("EXEC msdb.dbo.sp_start_job @job_name = 'Fector Download Transaction'") Then
                    ModelState.AddModelError("TDate", "The extract data is success")
                Else
                    ModelState.AddModelError("TDate", "The extract data is failed, extract data on progress. Please Wait for a moment")
                End If
            End If
        Catch ex As Exception
            ModelState.AddModelError("TDate", ex.Message)
        End Try

        
        'Try
        'db.Database.ExecuteSqlCommand("Insert into lk_nettingType values('99','Test')")


        'Dim tmp = db.Database.SqlQuery(Of String)("EXEC msdb.dbo.sp_start_job N'Fector Download Transaction'", Nothing).ToList

        'Dim param = New SqlParameter("jobName", "Fector Download Transaction")

        'ModelState.AddModelError("TDate", db.Database.ExecuteSqlCommand("EXEC msdb.dbo.sp_start_job  N'Fector Download Transaction'"))



        'db.Database.ExecuteSqlCommand("EXEC msdb.dbo.sp_start_job N'Fector Download Transaction'")

        'If db.Database.SqlQuery(Of Boolean)("EXEC msdb.dbo.sp_stop_job 'Fector Download Transaction'").SingleOrDefault() Then
        '    db = New FectorEntities
        '    If db.Database.SqlQuery(Of Boolean)("EXEC msdb.dbo.sp_start_job 'Fector Download Transaction'").SingleOrDefault = True Then
        '        LogTransaction.WriteLog(User.Identity.Name, "SUCCESS EXTRACT DATA MANUALLY", model.TDate, db)

        '        ModelState.AddModelError("TDate", "The extract data is success")
        '    Else
        '        LogTransaction.WriteLog(User.Identity.Name, "FAILED EXTRACT DATA MANUALLY", model.TDate, db)

        '        ModelState.AddModelError("TDate", "The extract data is fail")
        '    End If
        'End If

        ''If db.Database.ExecuteSqlCommand("EXEC msdb.dbo.sp_start_job 'Fector Download Transaction'") = 1 Then
        ''    LogTransaction.WriteLog(User.Identity.Name, "SUCCESS EXTRACT DATA MANUALLY", model.TDate, db)

        ''    ModelState.AddModelError("TDate", "The extract data is success")
        ''Else
        ''    LogTransaction.WriteLog(User.Identity.Name, "FAILED EXTRACT DATA MANUALLY", model.TDate, db)

        ''    ModelState.AddModelError("TDate", "The extract data is fail")
        ''End If
        'Catch ex As Exception
        'ModelState.AddModelError("TDate", ex.Message)
        'End Try

        Return View("Index", model)
    End Function
End Class