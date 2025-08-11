Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.ComponentModel

Public Class MsUser
    <Key> _
    <DisplayName("Username")> _
    <StringLength(20)> _
    <Required(ErrorMessage:="Please fill username")> _
    Public Property UserId As String

    Public Property Salt As String
    Public Property Password As String

    <DisplayName("Full Name")> _
    <StringLength(100)> _
    <Required(ErrorMessage:="Please fill full name")> _
    Public Property Fullname As String

    <DisplayName("Branch")> _
    <ForeignKey("Branch")> _
    <Required(ErrorMessage:="Please select branch")> _
    Public Property BranchId As String
    Public Overridable Property Branch As MsBranch

    <DisplayName("User Level")> _
    <ForeignKey("Userlevel")> _
    <Required(ErrorMessage:="Please select user level")> _
    Public Property UserlevelId As Integer
    Public Overridable Property Userlevel As MsUserlevel

    <DisplayName("Transaction Limit")> _
    <ForeignKey("Currency")> _
    Public Property TransactionLimitCurrency As String
    Public Overridable Property Currency As MsCurrency

    <Required(ErrorMessage:="Please fill transaction limit")> _
    <DisplayFormat(DataFormatString:="{0:N2}", ApplyFormatInEditMode:=True)> _
    Public Property TransactionLimit As Decimal

    Public Property PasswordLastChange As DateTime
    Public Property PasswordTry As Integer
    Public Property EditBy As String
    Public Property EditDate As DateTime
    Public Property ApproveBy As String
    Public Property ApproveDate As DateTime
    Public Property Status As String

    Public ReadOnly Property LimitDisplay As String
        Get
            Return String.Format("{0} - {1:N2}", TransactionLimitCurrency, TransactionLimit)
        End Get
    End Property

    Public Shared Function Authenticate(UserId As String, password As String) As Boolean
        Dim db As New FectorEntities()
        Dim user = GetUser(UserId, db)
        If user Is Nothing Then
            Return False
        End If

        Dim salt = user.Salt
        If (getMD5Hash(password + salt) = user.Password) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function getMD5Hash(ByVal strToHash As String) As String
        Dim md5Obj As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)

        bytesToHash = md5Obj.ComputeHash(bytesToHash)

        Dim strResult As String = ""
        Dim b As Byte

        For Each b In bytesToHash
            strResult += b.ToString("x2")
        Next

        Return strResult
    End Function

    Public Shared Function GetUser(ByVal UserId As String, ByVal db As FectorEntities) As MsUser
        Return db.Users.Where(Function(f) f.UserId = UserId And f.Status = "ACTIVE").SingleOrDefault
    End Function

    Public Shared Function GetAllUser(ByVal UserId As String, ByVal db As FectorEntities) As MsUser
        Return db.Users.Where(Function(f) f.UserId = UserId).SingleOrDefault
    End Function

    Public Shared Sub CaptureIncorrectPassword(UserId As String)
        Dim db As New FectorEntities()
        Dim user = GetUser(UserId, db)
        If Not IsNothing(user) Then
            user.PasswordTry = user.PasswordTry + 1
            db.Entry(user).State = EntityState.Modified
            db.SaveChanges()
        End If
    End Sub

    Public Shared Function isUserBlock(ByVal userid As String) As Boolean
        Dim db As New FectorEntities()
        Dim user = GetAllUser(userid, db)
        Dim maxAttempt As String = MsSetting.GetSetting("Password", "Incorrect", "", 1)
        If Not IsNothing(user) Then
            If user.PasswordTry >= maxAttempt Then
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Public Shared Function isPasswordExpired(ByVal userid As String) As Boolean
        Dim db As New FectorEntities()
        Dim user = GetUser(userid, db)
        Dim Period As Integer = CInt(MsSetting.GetSetting("Password", "ChangeTime", "Days", 1))
        If Not IsNothing(user) Then
            If DateAdd(DateInterval.Day, Period, user.PasswordLastChange) <= Now Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Shared Function isPasswordPolicyOk(ByVal password As String, ByRef message As String) As Boolean
        Dim MinLength As Integer = CInt(MsSetting.GetSetting("Password", "Min", "Char", 1))
        Dim MaxLength As Integer = CInt(MsSetting.GetSetting("Password", "Max", "Char", 1))

        If password.Length < MinLength Then
            message = "Minimum password length: " & MinLength
            Return False
        End If
        If password.Length > MaxLength Then
            message = "Maximum password length: " & MaxLength
            Return False
        End If

        If Not Regex.IsMatch(password, "^(?=.*\d)(?=.*[a-zA-Z])", RegexOptions.Singleline) Then
            message = "Password Policy: must contain at least 1 character and 1 number"
            Return False
        End If
        Return True
    End Function

    Public Shared Sub ChangePassword(UserId As String, password As String)
        Dim db As New FectorEntities()
        Dim user = GetUser(UserId, db)
        If user Is Nothing Then
            Exit Sub
        End If

        Dim salt = user.Salt
        user.Password = getMD5Hash(password + salt)
        user.PasswordLastChange = Now
        db.Entry(user).State = EntityState.Modified
        db.SaveChanges()
    End Sub

End Class
