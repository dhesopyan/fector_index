Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Security.Cryptography

Public Class LogUser
    <Key> _
    <DatabaseGenerated(DatabaseGeneratedOption.Identity)> _
    Public Property LogId As Long

    <ForeignKey("User")> _
    Public Property UserId As String
    Public Property User As MsUser

    Public Property LoginTime As DateTime
    Public Property ExpiredTime As DateTime
    Public Property LogoutTime As DateTime
    Public Property SessionId As String
    Public Property Status As Boolean

    Public Shared Function isLoginOK(UserId As String, SessionId As String) As Boolean
        Dim db As New FectorEntities
        Dim loguser = db.LogUsers.Where(Function(l) l.UserId = UserId And l.Status = True).FirstOrDefault
        If loguser Is Nothing Then
            Return True
        Else
            If loguser.SessionId = SessionId Then
                Return True
            Else
                If loguser.ExpiredTime < Now Then
                    Call LogoutExpiredUser(loguser)
                    Return True
                Else
                    Return False
                End If
            End If
        End If
    End Function

    Public Shared Sub LoginUser(UserId As String, SessionId As String)
        Dim SessionExpired As Integer = CInt(MsSetting.GetSetting("LoginTimeout", "", "", 1))
        Dim LoginTime As Date = Now
        Dim ExpiredTime As Date = Now.AddMinutes(SessionExpired)

        Dim db As New FectorEntities
        Dim user = MsUser.GetUser(UserId, db)
        Dim newlog As New LogUser() With {.UserId = UserId, .LoginTime = LoginTime, .ExpiredTime = ExpiredTime, .LogoutTime = New Date(1900, 1, 1), .SessionId = SessionId, .Status = 1}
        db.LogUsers.Add(newlog)
        user.PasswordTry = 0
        db.Entry(user).State = EntityState.Modified
        db.SaveChanges()
    End Sub

    Public Shared Sub LogoutExpiredUser(loguser As LogUser)
        Dim db As New FectorEntities
        If Not IsNothing(loguser) Then
            loguser.LogoutTime = Now
            loguser.Status = False
            db.Entry(loguser).State = EntityState.Modified
            db.SaveChanges()
        End If
    End Sub

    Public Shared Sub LogoutUser(UserId As String)
        Dim db As New FectorEntities
        Dim LogUser = db.LogUsers.Where(Function(l) l.UserId = UserId And l.Status = True).ToList

        If Not IsNothing(LogUser) Then
            For i As Integer = 0 To LogUser.Count - 1
                LogUser.Item(i).LogoutTime = Now
                LogUser.Item(i).Status = False
                db.Entry(LogUser(i)).State = EntityState.Modified
                db.SaveChanges()
            Next
        End If
    End Sub

    Public Shared Function createSessionId() As String
        Return Guid.NewGuid.ToString()
    End Function

    Public Shared Function checkSession(UserId As String, SessionId As String) As Boolean
        Dim db As New FectorEntities
        Dim LogUser = db.LogUsers.Where(Function(l) l.UserId = UserId And l.Status = True And l.SessionId = SessionId).FirstOrDefault

        If IsNothing(LogUser) Then
            Return False
        Else
            If LogUser.ExpiredTime <= Now Then
                Return False
            Else
                Return True
            End If
        End If
    End Function

    Public Shared Sub openPage(UserId As String, SessionId As String)
        Dim db As New FectorEntities
        Dim LogUser = db.LogUsers.Where(Function(l) l.UserId = UserId And l.Status = True And l.SessionId = SessionId).FirstOrDefault

        If Not IsNothing(LogUser) Then
            Dim SessionExpired As Integer = CInt(MsSetting.GetSetting("LoginTimeout", "", "", 1))
            Dim ExpiredTime As Date = Now.AddMinutes(SessionExpired)

            LogUser.ExpiredTime = ExpiredTime
            db.Entry(LogUser).State = EntityState.Modified
            db.SaveChanges()
        End If
    End Sub

End Class
